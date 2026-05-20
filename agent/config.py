from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", case_sensitive=False, extra="ignore")

    # ── Backend ─────────────────────────────────────────────
    backend_url: str = "http://backend:8080"
    agent_shared_secret: str

    # ── LiveKit ─────────────────────────────────────────────
    livekit_url: str
    livekit_api_key: str
    livekit_api_secret: str

    # ── External AI / voice providers ───────────────────────
    openai_api_key: str
    openai_chat_model: str = "gpt-4.1"

    deepgram_api_key: str
    deepgram_stt_model: str = "nova-3"
    deepgram_language: str = "en"

    elevenlabs_api_key: str
    elevenlabs_tts_model: str = "eleven_turbo_v2_5"

    # ElevenLabs voice IDs — one per PRD voice (selected from ElevenLabs Voice Library).
    elevenlabs_voice_james: str = ""
    elevenlabs_voice_sofia: str = ""
    elevenlabs_voice_marcus: str = ""
    elevenlabs_voice_elena: str = ""


settings = Settings()
