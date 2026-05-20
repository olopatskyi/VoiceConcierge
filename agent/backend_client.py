"""Thin async HTTP client to the .NET backend.

All protected endpoints (`/api/faq/search`, `/api/unanswered`) require the
`X-Agent-Secret` header. We attach it once at construction so every call
carries it. Auth-fail or 5xx propagate as `httpx.HTTPStatusError` for the
caller to handle.

Instantiate per session in the entrypoint and register `aclose` as a
shutdown callback on the JobContext.
"""
from typing import Any, TypedDict

import httpx

from config import settings


class VoiceConfigDto(TypedDict):
    activeVoiceId: str
    updatedAt: str


class FaqSearchHitDto(TypedDict):
    faq: dict[str, Any]
    score: float


class BackendClient:
    def __init__(self, client: httpx.AsyncClient | None = None) -> None:
        self._client = client or httpx.AsyncClient(
            base_url=settings.backend_url,
            headers={"X-Agent-Secret": settings.agent_shared_secret},
            timeout=10.0,
        )

    async def get_voice_config(self) -> VoiceConfigDto:
        """GET /api/voice-config → { activeVoiceId, updatedAt }."""
        resp = await self._client.get("/api/voice-config")
        resp.raise_for_status()
        return resp.json()["data"]

    async def search_knowledge_base(self, query: str, top_k: int = 3) -> list[FaqSearchHitDto]:
        """POST /api/faq/search → list of { faq: {...}, score: float }."""
        resp = await self._client.post(
            "/api/faq/search",
            json={"query": query, "topK": top_k},
        )
        resp.raise_for_status()
        return resp.json()["data"]

    async def record_unanswered_question(self, question: str) -> dict[str, Any]:
        """POST /api/unanswered → { id, question, frequency, status, ... }."""
        resp = await self._client.post(
            "/api/unanswered",
            json={"question": question},
        )
        resp.raise_for_status()
        return resp.json()["data"]

    async def aclose(self) -> None:
        await self._client.aclose()
