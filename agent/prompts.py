import logging
from pathlib import Path

logger = logging.getLogger(__name__)

_FALLBACK_PERSONA = (
    "You are a senior concierge at The Meridian Casino & Resort, a luxury Las Vegas property. "
    "Warm, professional, refined. Use search_knowledge_base for any property question. "
    "When you don't know, apologize and call record_unanswered_question."
)


def _load_persona() -> str:
    persona_file = Path(__file__).parent / "concierge_persona.md"
    try:
        return persona_file.read_text(encoding="utf-8")
    except OSError as exc:
        logger.error("Failed to load %s: %s — using fallback persona", persona_file, exc)
        return _FALLBACK_PERSONA


PERSONA_PROMPT = _load_persona()

GREETING = "Welcome to The Meridian Casino and Resort. How may I assist you this evening?"

FAREWELL = "Thank you for staying with us. Have a wonderful evening."
