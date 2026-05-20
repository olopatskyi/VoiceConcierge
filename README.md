# Voice Concierge — The Meridian Casino & Resort

A 24/7 voice-based concierge for a luxury Las Vegas property. Guests speak
through a web playground; an AI agent looks up answers in a Meridian-specific
knowledge base and captures any question it can't answer for the concierge
team to review.

Take-home assessment.

---

## Stack

| Layer | Technology |
|---|---|
| Backend | .NET 10 + ASP.NET Core + EF Core 10 + Postgres 16 + pgvector |
| Voice agent | Python 3.12 + `livekit-agents` (Deepgram STT, OpenAI LLM, ElevenLabs TTS) |
| Admin panel (bonus) | React 19 + Vite + TypeScript (skeleton only — see *Status* below) |
| Orchestration | Docker Compose |

See [`ARCHITECTURE.md`](./ARCHITECTURE.md) for design decisions and trade-offs.

---

## Quick start

### 1. Prerequisites

- **Docker** with Compose v2 (`docker compose ...`)
- API keys for: **LiveKit Cloud**, **OpenAI**, **Deepgram**, **ElevenLabs**
- ~5 minutes for first build (NuGet restore + Python deps + npm install)

### 2. Configure environment

```bash
cp .env.example .env
```

Open `.env` and fill in real values. A few notes:

**Generate a strong agent shared secret:**
```bash
# overwrite the placeholder with a 32-byte random hex string
sed -i.bak "s|^AGENT_SHARED_SECRET=.*|AGENT_SHARED_SECRET=$(openssl rand -hex 32)|" .env && rm .env.bak
```

**ElevenLabs voice IDs** — open https://elevenlabs.io/app/voice-library and
pick four voices matching the PRD personalities (mature British male,
friendly European female, confident American male, calm American female).
Paste the voice ID strings into `ElevenLabs__VoiceJames`, `VoiceSofia`,
`VoiceMarcus`, `VoiceElena`.

**LiveKit** — create a project at https://cloud.livekit.io. Copy the URL
(`wss://...livekit.cloud`), API key, and API secret.

### 3. Launch

```bash
docker compose up --build
```

First start: ~2 minutes (build) + ~30 seconds (Postgres ready + migrations +
FAQ seed via OpenAI embedding batch call). Watch for:

```
backend  | Migration applied: 20260519180523_Initial
backend  | Migration applied: 20260519204309_UnansweredQueueCompositeIndex
backend  | Seeded 65 FAQ items.
backend  | Now listening on: http://[::]:8080
agent    | Worker registered with LiveKit
```

If you see `FAQ seed failed — embeddings could not be generated`, OpenAI is
unreachable or your `OpenAI__ApiKey` is wrong. The app still starts; you can
fix the key and restart, and seed will retry.

### 4. Verify

| URL | Purpose |
|---|---|
| `http://localhost:8080/health` | Liveness probe — should return `{"status":"ok"}` |
| `http://localhost:8080/scalar` | Interactive OpenAPI UI (dev only) — explore every endpoint |
| `http://localhost:8080/openapi/v1.json` | Raw OpenAPI document |

---

## Trying the voice concierge

The PRD allows using the official LiveKit Agents Playground for testing.

1. Open https://agents-playground.livekit.io
2. Choose **"Manual"** connection mode
3. Get a fresh token from your local backend:
   ```bash
   curl -X POST http://localhost:8080/api/livekit-token \
     -H "Content-Type: application/json" \
     -d '{"participantName":"Test Guest"}'
   ```
   The response body is:
   ```json
   {
     "data": {
       "token": "eyJhbG...",
       "url": "wss://your-project.livekit.cloud",
       "roomName": "concierge-...",
       "participantIdentity": "guest-...",
       "participantDisplayName": "Test Guest"
     }
   }
   ```
4. Paste **url** and **token** into the playground, click *Connect*
5. The agent greets you — speak normally

Sample conversations (also seeded as FAQs — see `agent/concierge_persona.md`
for the full persona prompt):

| You ask | The agent will… |
|---|---|
| "Is the poker room open right now?" | Look up gaming hours, confirm 24/7 |
| "What's your best restaurant?" | Recommend Aurelia (the seeded signature) |
| "Can I bring my dog to the hotel?" | Apologize, record it in unanswered queue |
| "Any good restaurants nearby?" | Mention Carbone + the partner discount |

---

## Endpoints quick reference

All success responses are `{"data": T}`. Errors are RFC 7807 `ProblemDetails`.
See `/scalar` for full OpenAPI.

| Method | Path | Auth | Purpose |
|---|---|---|---|
| GET | `/api/faq` | — | List all FAQs |
| GET | `/api/faq/{id}` | — | Get one |
| POST | `/api/faq` | — | Create (embeds) |
| PUT | `/api/faq/{id}` | — | Update (re-embeds) |
| DELETE | `/api/faq/{id}` | — | Delete |
| POST | `/api/faq/search` | **X-Agent-Secret** | Semantic search top-K (agent only) |
| GET | `/api/unanswered?status=Pending` | — | Admin queue (filtered+sorted) |
| POST | `/api/unanswered` | **X-Agent-Secret** | Record unanswered (agent only) |
| POST | `/api/unanswered/{id}/dismiss` | — | Admin: mark dismissed |
| POST | `/api/unanswered/{id}/convert` | — | Admin: build FAQ from question |
| GET | `/api/voice-config` | — | Get current active voice |
| PUT | `/api/voice-config` | — | Set active voice |
| GET | `/api/voices` | — | List catalog (4 voices with descriptions) |
| POST | `/api/voices/{id}/preview` | — | Returns `audio/mpeg` sample bytes |
| POST | `/api/livekit-token` | — | Issue browser room token |

---

## Inspecting the database

```bash
docker compose exec db psql -U concierge -d concierge
```

Useful queries:

```sql
-- count of seeded FAQs by category
SELECT "Category", COUNT(*) FROM faq_items GROUP BY "Category" ORDER BY 2 DESC;

-- queue listing (admin's view)
SELECT "Question", "Frequency", "LastAskedAt"
FROM unanswered_questions WHERE "Status" = 'Pending'
ORDER BY "Frequency" DESC, "LastAskedAt" DESC LIMIT 50;

-- confirm composite index is used for the queue query
EXPLAIN ANALYZE
SELECT * FROM unanswered_questions
WHERE "Status" = 'Pending'
ORDER BY "Frequency" DESC, "LastAskedAt" DESC LIMIT 50;
-- expect: Index Scan using IX_unanswered_questions_Status_Frequency_LastAskedAt
```

---

## Status

### ✅ Core (PRD section "Functional Requirements: Core")

- Voice agent with STT/LLM/TTS pipeline and two function tools
- Backend API for FAQ search + unanswered recording
- Postgres seeded with **65 FAQs** covering all PRD topics
- Single-command launch (`docker compose up`)
- This README + `ARCHITECTURE.md`
- Playground: LiveKit Agents Playground (external, per PRD allowance)

### ⏳ Bonus (PRD section "Functional Requirements: Bonus")

- All backend endpoints implemented for FAQ management, unanswered queue,
  voice config + preview, and LiveKit token issuance
- React admin UI: **not built** — only Vite skeleton. The four feature areas
  (FAQ CRUD, queue, voice picker, embedded playground) are straightforward
  on top of the existing API.

---

## Project layout

```
voice-concierge/
├── docker-compose.yml
├── .env.example
├── README.md
├── ARCHITECTURE.md
├── backend/                 .NET 10 — VoiceConcierge.Api + VoiceConcierge.Data
├── agent/                   Python LiveKit worker
└── admin/                   React + Vite + TS (skeleton)
```

---

## Database migrations

Migrations apply automatically on container startup
(`db.Database.MigrateAsync()`).

To add a new migration locally:

```bash
cd backend
dotnet ef migrations add <Name> \
  --project VoiceConcierge.Data/VoiceConcierge.Data.csproj \
  --startup-project VoiceConcierge.Api/VoiceConcierge.Api.csproj \
  --output-dir Migrations
```

The next `docker compose up` will apply it.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| Backend exits with `AgentAuth:SharedSecret is required` | `AGENT_SHARED_SECRET` empty or missing in `.env` | Set it (see *Quick start*) |
| `Seeded 0 FAQ items` and `FAQ seed failed` in logs | OpenAI key invalid or rate-limited at boot | Fix `OpenAI__ApiKey`, restart `backend` container |
| Agent silent on join, no greeting | Most likely `ELEVENLABS_VOICE_*` voice IDs empty | Set the four IDs in `.env`, restart `agent` |
| `401 Agent authentication required` from agent | Backend secret differs from agent's | Both read `AGENT_SHARED_SECRET` — confirm only one value in `.env` |
| Backend can't connect to db at startup | Postgres still booting | EnableRetryOnFailure handles transient errors; first start may take ~30s |
| Voice config endpoint 404 on first call | Seed runner didn't insert singleton | Check `backend` logs for migration/seed completion |

