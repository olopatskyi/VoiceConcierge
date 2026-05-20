"""Maps the active voice id reported by backend (`"James"`/`"Sofia"`/`"Marcus"`/`"Elena"`)
to the configured ElevenLabs voice id from settings.

Voice IDs are picked from the ElevenLabs Voice Library to match the personality
descriptions in PRD (VX-1): mature British male, friendly European female, etc.
"""
from config import settings


def get_elevenlabs_voice_id(active_voice_id: str) -> str:
    mapping = {
        "james": settings.elevenlabs_voice_james,
        "sofia": settings.elevenlabs_voice_sofia,
        "marcus": settings.elevenlabs_voice_marcus,
        "elena": settings.elevenlabs_voice_elena,
    }
    voice_id = mapping.get(active_voice_id.lower(), "")
    if not voice_id:
        raise ValueError(
            f"ElevenLabs voice id is not configured for '{active_voice_id}'. "
            f"Set ELEVENLABS_VOICE_{active_voice_id.upper()} in .env."
        )
    return voice_id
