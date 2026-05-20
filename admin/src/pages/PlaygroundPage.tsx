import { useState } from 'react';
import {
  LiveKitRoom,
  RoomAudioRenderer,
  StartAudio,
  useConnectionState,
  useTracks,
  VoiceAssistantControlBar,
} from '@livekit/components-react';
import { ConnectionState, Track } from 'livekit-client';

import { describeApiError } from '../api/client';
import { livekitTokenApi } from '../api/livekitToken';
import type { LiveKitTokenModel } from '../api/types';

export function PlaygroundPage() {
  const [session, setSession] = useState<LiveKitTokenModel | null>(null);
  const [connecting, setConnecting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleConnect = async () => {
    setError(null);
    setConnecting(true);
    try {
      const token = await livekitTokenApi.issue({ participantName: 'Test Guest' });
      setSession(token);
    } catch (err) {
      setError(describeApiError(err));
    } finally {
      setConnecting(false);
    }
  };

  const handleDisconnect = () => {
    setSession(null);
  };

  return (
    <div className="playground-wrap">
      <div>
        <h1>Playground</h1>
        <p className="page-subtitle">
          Speak with the concierge to test the active voice and FAQ knowledge. The agent uses
          whichever voice is currently active (configured on the Voices page).
        </p>
      </div>

      <div className="test-mode-banner">Test Mode</div>

      {error && <div className="error">Failed to start session: {error}</div>}

      {!session ? (
        <div className="card">
          <p className="muted" style={{ marginTop: 0 }}>
            Click <strong>Start conversation</strong> to request a fresh LiveKit token and join
            the room. The concierge will greet you automatically.
          </p>
          <button className="primary" onClick={handleConnect} disabled={connecting}>
            {connecting ? 'Connecting…' : 'Start conversation'}
          </button>
        </div>
      ) : (
        <LiveKitRoom
          token={session.token}
          serverUrl={session.url}
          connect
          audio
          video={false}
          onDisconnected={handleDisconnect}
          onError={(e) => {
            setError(e.message);
            handleDisconnect();
          }}
        >
          <ActiveSession onEnd={handleDisconnect} />
        </LiveKitRoom>
      )}
    </div>
  );
}

interface ActiveSessionProps {
  onEnd: () => void;
}

function ActiveSession({ onEnd }: ActiveSessionProps) {
  const state = useConnectionState();
  const agentTracks = useTracks([Track.Source.Microphone], { onlySubscribed: true });

  return (
    <div className="card">
      <div className="toolbar">
        <StatusPill state={state} />
        <span className="muted">
          {agentTracks.length > 0 ? `Agent connected (${agentTracks.length} track)` : 'Waiting for agent…'}
        </span>
        <div className="spacer" />
        <button className="danger" onClick={onEnd}>
          End conversation
        </button>
      </div>

      <div style={{ marginTop: 16 }}>
        <VoiceAssistantControlBar />
      </div>

      <RoomAudioRenderer />
      <StartAudio label="Click to allow audio playback" />
    </div>
  );
}

function StatusPill({ state }: { state: ConnectionState }) {
  const config = {
    [ConnectionState.Disconnected]: { dot: 'disconnected', label: 'Disconnected' },
    [ConnectionState.Connecting]: { dot: 'connecting', label: 'Connecting…' },
    [ConnectionState.Connected]: { dot: 'connected', label: 'Connected' },
    [ConnectionState.Reconnecting]: { dot: 'connecting', label: 'Reconnecting…' },
    [ConnectionState.SignalReconnecting]: { dot: 'connecting', label: 'Reconnecting…' },
  }[state] ?? { dot: 'disconnected', label: state };

  return (
    <span className="status-pill">
      <span className={`status-dot ${config.dot}`} />
      {config.label}
    </span>
  );
}
