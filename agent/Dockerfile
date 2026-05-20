FROM python:3.12-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    && rm -rf /var/lib/apt/lists/*

COPY pyproject.toml ./
RUN pip install --no-cache-dir --upgrade pip && \
    pip install --no-cache-dir \
      livekit-agents>=1.0.0 \
      livekit-plugins-deepgram>=1.0.0 \
      livekit-plugins-openai>=1.0.0 \
      livekit-plugins-elevenlabs>=1.0.0 \
      livekit-plugins-silero>=1.0.0 \
      httpx>=0.27.0 \
      pydantic-settings>=2.5.0 \
      python-dotenv>=1.0.1

COPY . .

CMD ["python", "main.py", "start"]
