import logging

from livekit.agents import JobProcess, WorkerOptions, cli
from livekit.plugins import silero

from session import entrypoint


def prewarm(proc: JobProcess) -> None:
    """Load heavy ML assets once per worker process so each session starts fast."""
    proc.userdata["vad"] = silero.VAD.load()


if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    )

    cli.run_app(WorkerOptions(entrypoint_fnc=entrypoint, prewarm_fnc=prewarm))
