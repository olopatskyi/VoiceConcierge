using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VoiceConcierge.Api.Middleware;
using VoiceConcierge.Api.Options;
using VoiceConcierge.Api.Providers;
using VoiceConcierge.Api.Repositories;
using VoiceConcierge.Api.Seed;
using VoiceConcierge.Api.Services;
using VoiceConcierge.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ── Options (validated on startup, fail-fast if any credential missing) ──
builder.Services.AddOptions<OpenAIOptions>()
    .Bind(builder.Configuration.GetSection(OpenAIOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "OpenAI:ApiKey is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ChatModel), "OpenAI:ChatModel is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.EmbeddingModel), "OpenAI:EmbeddingModel is required")
    .ValidateOnStart();

builder.Services.AddOptions<LiveKitOptions>()
    .Bind(builder.Configuration.GetSection(LiveKitOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Url), "LiveKit:Url is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "LiveKit:ApiKey is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiSecret), "LiveKit:ApiSecret is required")
    .ValidateOnStart();

builder.Services.AddOptions<ElevenLabsOptions>()
    .Bind(builder.Configuration.GetSection(ElevenLabsOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "ElevenLabs:ApiKey is required")
    .ValidateOnStart();

builder.Services.AddOptions<AgentAuthOptions>()
    .Bind(builder.Configuration.GetSection(AgentAuthOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SharedSecret), "AgentAuth:SharedSecret is required")
    .ValidateOnStart();

// ── Database ──
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

builder.Services.AddDbContext<ConciergeDbContext>(opts =>
    opts.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.UseVector();
        npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    }));

builder.Services.AddScoped<SeedRunner>();

// ── External providers (typed HttpClient + standard resilience: retry + circuit breaker + timeout) ──
builder.Services
    .AddHttpClient<IOpenAIEmbeddingProvider, OpenAIEmbeddingProvider>(client =>
    {
        client.BaseAddress = new Uri("https://api.openai.com/");
    })
    .AddStandardResilienceHandler();

builder.Services
    .AddHttpClient<IElevenLabsTtsProvider, ElevenLabsTtsProvider>(client =>
    {
        client.BaseAddress = new Uri("https://api.elevenlabs.io/");
    })
    .AddStandardResilienceHandler();

// ── Services + Repositories ──
builder.Services.AddScoped<IFaqRepository, FaqRepository>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<IUnansweredRepository, UnansweredRepository>();
builder.Services.AddScoped<IUnansweredService, UnansweredService>();
builder.Services.AddScoped<IVoiceConfigRepository, VoiceConfigRepository>();
builder.Services.AddScoped<IVoiceConfigService, VoiceConfigService>();
builder.Services.AddScoped<IVoicePreviewService, VoicePreviewService>();
builder.Services.AddSingleton<ILiveKitTokenService, LiveKitTokenService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRouting();
app.UseMiddleware<AgentSecretMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ── Migrations + seed on startup ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConciergeDbContext>();
    await db.Database.MigrateAsync();

    var seedRunner = scope.ServiceProvider.GetRequiredService<SeedRunner>();
    await seedRunner.RunAsync();
}

app.Run();
