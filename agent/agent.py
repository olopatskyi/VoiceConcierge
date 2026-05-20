"""Concierge agent definition.

The `ConciergeAgent` extends `livekit.agents.Agent` with the persona prompt
and two function tools the LLM can call during a conversation.
"""
import logging
from typing import Any

from httpx import HTTPError
from livekit.agents import Agent, RunContext, function_tool

from backend_client import BackendClient, FaqSearchHitDto
from prompts import PERSONA_PROMPT

logger = logging.getLogger(__name__)


class ConciergeAgent(Agent):
    def __init__(self, backend: BackendClient) -> None:
        super().__init__(instructions=PERSONA_PROMPT)
        self._backend = backend

    @function_tool
    async def search_knowledge_base(
        self,
        context: RunContext,
        query: str,
    ) -> list[FaqSearchHitDto]:
        """Search The Meridian Casino & Resort knowledge base.

        Call this for ANY guest question about the property — gaming, dining,
        accommodations, amenities, partners, hours, policies. Returns up to 3
        FAQs with relevance scores; the higher the score, the closer the match.

        Args:
            query: The query to search for, optimally rephrased to maximize match
                   quality (e.g., "best restaurant" → "signature fine dining restaurant").
        """
        try:
            results = await self._backend.search_knowledge_base(query, top_k=3)
            logger.info("kb_search query=%r hits=%d", query, len(results))
            return results
        except HTTPError as exc:
            logger.error("kb_search failed query=%r error=%s", query, exc)
            return []

    @function_tool
    async def record_unanswered_question(
        self,
        context: RunContext,
        question: str,
    ) -> None:
        """Record a guest question that the knowledge base could not answer.

        Always call this after apologizing for not knowing something. The
        concierge team reviews these in the admin panel.

        Args:
            question: The user's question, normalized into a canonical form.
        """
        try:
            await self._backend.record_unanswered_question(question)
            logger.info("unanswered_recorded question=%r", question)
        except HTTPError as exc:
            logger.error("unanswered_record failed question=%r error=%s", question, exc)
