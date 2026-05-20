namespace VoiceConcierge.Api.Options;

public class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;

    public string ChatModel { get; set; } = "gpt-4.1";

    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
