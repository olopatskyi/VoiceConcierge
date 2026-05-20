"""Per-room session lifecycle.

LiveKit dispatches the worker to a freshly created room; this entrypoint runs
once per room. It:

1. Connects to the room.
2. Instantiates a per-session BackendClient and registers cleanup.
3. Fetches the active voice from backend (PRD VX-3: voice change applies to
   new conversations only). Falls back to Sofia if backend is unreachable.
4. Wires Deepgram STT → OpenAI LLM → ElevenLabs TTS pipeline (VAD prewarmed
   at worker startup).
5. Starts the AgentSession with our ConciergeAgent.
6. Greets the guest.
"""
import json
import logging

import httpx
from livekit.agents import AgentSession, JobContext
from livekit.plugins import deepgram, elevenlabs, openai, silero

from agent import ConciergeAgent
from backend_client import BackendClient
from config import settings
from prompts import GREETING
from voice_mapping import get_elevenlabs_voice_id

logger = logging.getLogger(__name__)

_DEFAULT_VOICE = "Sofia"


async def entrypoint(ctx: JobContext) -> None:
    await ctx.connect()

    backend = BackendClient()
    ctx.add_shutdown_callback(backend.aclose)

    try:
        voice_config = await backend.get_voice_config()
        active_voice = voice_config.get("activeVoiceId") or _DEFAULT_VOICE
    except (httpx.HTTPError, KeyError, json.JSONDecodeError) as exc:
        logger.error("voice_config fetch failed, falling back to %s: %s", _DEFAULT_VOICE, exc)
        active_voice = _DEFAULT_VOICE

    elevenlabs_voice_id = get_elevenlabs_voice_id(active_voice)
    logger.info("session_starting room=%s voice=%s", ctx.room.name, active_voice)

    vad = ctx.proc.userdata.get("vad") or silero.VAD.load()

    # Pass api_key explicitly so we don't depend on each plugin's preferred env-var name
    # (e.g., ElevenLabs plugin looks for ELEVEN_API_KEY, not ELEVENLABS_API_KEY).
    session = AgentSession(
        stt=deepgram.STT(
            model=settings.deepgram_stt_model,
            language=settings.deepgram_language,
            api_key=settings.deepgram_api_key,
        ),
        llm=openai.LLM(
            model=settings.openai_chat_model,
            api_key=settings.openai_api_key,
        ),
        tts=elevenlabs.TTS(
            voice_id=elevenlabs_voice_id,
            model=settings.elevenlabs_tts_model,
            api_key=settings.elevenlabs_api_key,
        ),
        vad=vad,
    )

    await session.start(room=ctx.room, agent=ConciergeAgent(backend))
    await session.say(GREETING, allow_interruptions=False)
