FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VoiceConcierge.sln ./
COPY VoiceConcierge.Api/VoiceConcierge.Api.csproj VoiceConcierge.Api/
COPY VoiceConcierge.Data/VoiceConcierge.Data.csproj VoiceConcierge.Data/
RUN dotnet restore VoiceConcierge.sln

COPY . .
RUN dotnet publish VoiceConcierge.Api/VoiceConcierge.Api.csproj -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VoiceConcierge.Api.dll"]
