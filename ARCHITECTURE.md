# Architecture — Voice Concierge

Design decisions and trade-offs for the Meridian Casino & Resort voice concierge.
Focuses on the *why*; code is the source of *what*.

---

## Service topology

```
┌──────────────────┐   REST /api/livekit-token
│   Browser        │ ─────────────────────────────►  Backend (.NET 10)
│   (Playground)   │                                     │
└────────┬─────────┘                                     ▼
         │ WebRTC                              Postgres 16 + pgvector
         │
         ▼
   LiveKit Cloud (audio room)                            ▲
         │                                               │
         │ WebSocket                                     │
         ▼                                               │
┌──────────────────┐   REST (X-Agent-Secret)             │
│  Voice Agent     │ ───────────────────────────────────►┘
│  (Python 3.12)   │
└────────┬─────────┘
         │
         ├─► Deepgram (STT)
         ├─► OpenAI    (LLM)
         └─► ElevenLabs (TTS)
```

Four docker-compose services: `db`, `backend`, `agent`, `admin`.
External: LiveKit Cloud, OpenAI, Deepgram, ElevenLabs.

---

## Stack — and why

| Layer | Choice | Rationale |
|---|---|---|
| Backend | **.NET 10** + ASP.NET Core + EF Core 10 | Strongest layer to demonstrate engineering depth. First-class pgvector via `Pgvector.EntityFrameworkCore`. Typed contracts, OpenAPI out-of-the-box, FluentValidation. |
| Agent | **Python 3.12** + `livekit-agents` 1.x | The LiveKit Agents SDK is most mature in Python — built-in turn detection, prewarm/pipeline plugins, comprehensive examples. |
| Admin | **React 19 + Vite + TS** | Required by PRD for bonus. Not implemented in this submission (skeleton only). |
| DB | **Postgres 16** + pgvector | Single store covers relational data and vector similarity search. `pgvector/pgvector:pg16` image. |

Three languages in one repo is intentional — each service is fully isolated by its Dockerfile. The agent service is ~500 LOC, not a serious cognitive tax.

---

## Per-request flow inside a voice session

```
User asks: "Is the poker room open right now?"
   │
   ▼
LiveKit Cloud routes audio → Agent container
   │
   ▼
livekit-agents pipeline:
   ① Turn detection (silero VAD, prewarmed on worker start)
   ② STT  (Deepgram nova-3, streaming)         → "Is the poker room open right now?"
   ③ LLM  (OpenAI gpt-4.1)
       │
       │ Decides: this is a property question. Call tool.
       │
       ▼
   ④ Function tool: search_knowledge_base(query="poker room hours")
       │
       └─► HTTPS POST /api/faq/search
            X-Agent-Secret: <shared>
            { "query": "...", "topK": 3 }
                │
                ▼
            Backend:
              · Embed query (OpenAI text-embedding-3-small, 1536 dims)
              · pgvector cosine search via HNSW index
              · Return top-3 with similarity scores
                │
                ▼
            { "data": [
                { "faq": {...}, "score": 0.94 },
                { "faq": {...}, "score": 0.72 },
                { "faq": {...}, "score": 0.61 }
              ]}
       │
   ⑤ LLM forms warm, concierge-toned response from result #1
   ⑥ TTS (ElevenLabs eleven_turbo_v2_5 with active voice)
   │
   ▼
Audio back to LiveKit Cloud → User hears reply
```

End-to-end latency budget: ~1.0–1.5s from end-of-user-speech to first agent words.

---

## Knowledge-base search — RAG strategy

LLM-driven retrieval via **function tools**, not fixed-threshold backend filtering.

**Why:**
- LLM rephrases ambiguous user phrasing into optimal search queries
  (e.g., "what's the best place to eat?" → "signature fine dining restaurant").
- Backend returns top-K hits with cosine similarity scores; the LLM decides
  whether any are relevant enough to answer from.
- Handles multi-turn context naturally: "and what about Italian?" can trigger
  a follow-up search without our code tracking conversation state.
- When the LLM judges no match → it calls `record_unanswered_question` and
  apologizes — the no-match logic is in the prompt, not in backend thresholds.

**Trade-off:** every property question = 1 LLM round trip + 1 backend search. We
accept ~150ms extra over a pure-vector approach for naturalness and tone control.

---

## Single source of truth for embeddings

All embeddings — for FAQ inserts, FAQ updates, search queries, unanswered-question
dedup — go through the **backend's** `IEmbeddingService` (OpenAI
`text-embedding-3-small`).

The agent never embeds. This prevents drift if the embedding model is swapped
later: change one config value in one place, regenerate, done.

---

## Voice configuration model

A singleton row (`voice_config` table, `Id = 1`, guarded by CHECK constraint).

| Event | Behavior |
|---|---|
| `GET /api/voice-config` | Returns current active voice |
| `PUT /api/voice-config` | Admin sets new voice; updates row |
| New voice session starts | Agent fetches voice config in `entrypoint`, instantiates ElevenLabs TTS with that voice id |
| Existing session in progress | Unchanged — voice is captured at session start |

Per PRD VX-3: "Voice change takes effect immediately for new conversations."
We achieve this **without hot-swapping TTS mid-session**, which keeps the agent
state machine simple and matches what the PRD asks for literally.

---

## Frequency dedup for unanswered questions

On `POST /api/unanswered`:

1. Backend embeds the incoming question.
2. Searches existing **Pending** unanswered rows for cosine ≥ 0.85.
3. If a similar pending question exists → bump its `Frequency`, update `LastAskedAt`.
4. Else → insert new row with `Frequency = 1`.

Threshold 0.85 was chosen empirically (≈ 1 − 0.15 cosine distance). Lower values
collapse distinct questions; higher misses near-duplicates.

**Known race:** between `FindSimilar` and `BumpFrequency`, a concurrent caller
may insert a parallel row → momentary duplication. Documented as acceptable at
PRD scale (one casino, ~tens of agent calls per day). Mitigations:
- Postgres advisory lock keyed on normalized question hash.
- Or a UNIQUE constraint on a normalized text column.
Neither implemented — premature for the assessment.

---

## Agent-to-backend authentication

A shared secret in the `X-Agent-Secret` header, set via env var (`AgentAuth__SharedSecret`
for backend, `AGENT_SHARED_SECRET` for agent). Both read from the same `.env`
entry through docker-compose.

**Validation flow:**

```
Agent request → Backend middleware (AgentSecretMiddleware)
   │
   ├─ endpoint has [RequireAgentSecret]?  → if no, pass through
   │
   ├─ read X-Agent-Secret header
   │
   ├─ compare with options.SharedSecret using
   │  CryptographicOperations.FixedTimeEquals (constant-time)
   │
   └─ mismatch or missing → 401 ProblemDetails
```

**Protected endpoints** (agent-only):
- `POST /api/faq/search`
- `POST /api/unanswered`

Other endpoints (admin CRUD, voice config, LiveKit token) are unauthenticated
because admin auth is **out of scope** per PRD.

**Startup-time validation:** `ValidateOnStart()` ensures the secret is non-empty
at backend startup. Missing → backend fails fast with clear message.

---

## Response shape

| Outcome | HTTP | Body |
|---|---|---|
| Success with value | 200 | `{ "data": T }` envelope |
| Success without value (DELETE) | 204 | empty |
| Validation failure | 400 | `ValidationProblemDetails` (RFC 7807 + `errors` dict) |
| Not found | 404 | `ProblemDetails` |
| Conflict (e.g. already Converted) | 409 | `ProblemDetails` |
| Agent auth failure | 401 | `ProblemDetails` |
| Unhandled exception | 500 | `ProblemDetails` (via `AddProblemDetails` + `UseExceptionHandler`) |

The success envelope is opinionated; client always knows where data lives.
Failures use the RFC 7807 standard for cross-stack tool compatibility.

---

## Repository / service / controller layering

```
Controller          Service                          Repository              DB
─────────           ───────────                      ────────────            ──
Request ─────────► .CreateAsync(Request) ─────►   .CreateAsync(Request, embedding) ─► Entity + SaveChanges
                       │                                  │
                       │ 1. ValidateAsync(req)            │ 1. Build Entity from Request
                       │ 2. Embed via OpenAI              │ 2. SaveChangesAsync
                       │ 3. Call repo.CreateAsync         │ 3. Map Entity → Model
                       │ 4. Return ServiceResponse<Model>  ◄┘
                       ▼
.ActionResult(serviceResponse) ◄── ServiceResponse<Model>
```

- **Controllers** are one-line per action; just forward to service and call
  `ActionResult(...)` (from `BaseController`).
- **Services** inherit `LogicalLayerElement` (provides typed `Success`/`Failure`
  factories). They orchestrate: validate → external calls → repo → return
  `ServiceResponse<Model>`.
- **Repositories** accept Request DTOs and return Models. Entity ↔ Model mapping
  is local to the repo (no AutoMapper).
- **Models** live in `Api/Models/` and are exposed as the response payload
  (wrapped in `{data: …}`). Contracts in `Api/Contracts/` are inputs (Requests)
  and their validators only.

This mirrors the pattern used in our other production services — predictable
for any reviewer familiar with .NET layering.

---

## Database indexes

| Table | Index | Purpose |
|---|---|---|
| `faq_items` | PK `Id` | CRUD by id |
| `faq_items` | `Category` | Future filtered listing |
| `faq_items` | `Embedding` HNSW (`vector_cosine_ops`) | Search top-K |
| `unanswered_questions` | PK `Id` | CRUD by id |
| `unanswered_questions` | composite `(Status, Frequency DESC, LastAskedAt DESC)` | Admin queue listing (status filter + sort) |
| `unanswered_questions` | `Embedding` HNSW (`vector_cosine_ops`) | Dedup similarity search |
| `voice_config` | PK + CHECK `Id = 1` | Singleton guard |

**HNSW + WHERE caveat:** pgvector HNSW indexes don't support filtered queries.
For `FindSimilarPendingAsync(WHERE Status = 'Pending')` Postgres falls back to
sequential scan. At PRD scale (~tens of pending) this is fine; a partial HNSW
index (`WHERE status = 'Pending'`) is the production fix.

---

## What is **not** done

### Out-of-scope per PRD (intentionally absent)
- Authentication for admin
- Reservation booking, payment processing, PMS integration
- Mobile app
- Multi-language support (Deepgram locked to `en`)
- Guest-facing standalone website

### In-scope but not yet built
- **React admin panel** (bonus): all backend endpoints are implemented and
  documented in OpenAPI, but the UI is just a Vite skeleton. The four feature
  areas — FAQ CRUD, unanswered queue, voice config + preview, embedded playground —
  are straightforward to build on top of the existing API.
- **Automated tests**: no `*.Tests` project. The patterns used (interfaces, DI,
  ServiceResponse) are deliberately testable; adding xUnit + NSubstitute + a
  Testcontainers Postgres fixture is a clean follow-up.

### Acknowledged limitations (scale-out)
- Unanswered queue index works to ~1k pending rows; beyond that we'd want a
  partial HNSW index by status.
- BumpFrequency uses tracked-entity update — a concurrent double-bump may
  understate frequency by one. Acceptable for one casino's traffic.
- Embedding generation happens on FAQ create/update synchronously — at 60–80
  items in seed this is one batch call (~3s). At thousands of FAQs we'd want
  a background job.
- Agent secret rotation requires restarting both containers (no zero-downtime
  rotation in this design).

---

## Repository layout

```
voice-concierge/
├── docker-compose.yml          # 4 services (db, backend, agent, admin)
├── .env.example                # all environment variables
├── README.md
├── ARCHITECTURE.md             # this file
│
├── backend/                    # .NET 10 solution
│   ├── VoiceConcierge.sln
│   ├── Dockerfile              # multi-stage build
│   ├── Api/                    # ASP.NET Core project
│   │   ├── Program.cs
│   │   ├── Controllers/        # 4 controllers
│   │   ├── Services/           # 6 services (FAQ, Unanswered, VoiceConfig,
│   │   │                       #   LiveKitToken, Embedding, VoicePreview)
│   │   ├── Repositories/       # 3 repositories
│   │   ├── Contracts/          # request DTOs + FluentValidation validators
│   │   ├── Models/             # response shapes
│   │   ├── Options/            # IOptions classes per external service
│   │   ├── Middleware/         # AgentSecretMiddleware + attribute
│   │   ├── Common/             # ServiceResponse, BaseController, LogicalLayerElement
│   │   └── Seed/               # SeedRunner + 65 Meridian FAQs
│   └── Data/                   # EF Core project (entities + migrations only)
│       ├── ConciergeDbContext.cs
│       ├── Entities/
│       ├── Enums/
│       ├── Configurations/
│       └── Migrations/
│
├── agent/                      # Python LiveKit worker
│   ├── Dockerfile
│   ├── main.py                 # WorkerOptions + prewarm
│   ├── session.py              # entrypoint(ctx)
│   ├── agent.py                # ConciergeAgent + @function_tool methods
│   ├── backend_client.py       # httpx client with X-Agent-Secret
│   ├── voice_mapping.py        # PRD voice name → ElevenLabs voice id
│   ├── config.py               # pydantic-settings
│   ├── prompts.py
│   └── concierge_persona.md    # LLM system prompt (markdown for ease of editing)
│
└── admin/                      # React + Vite + TS (skeleton only)
    └── src/
```
