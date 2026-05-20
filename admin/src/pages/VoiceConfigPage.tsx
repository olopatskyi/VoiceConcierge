import { useEffect, useRef, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { describeApiError } from '../api/client';
import type { VoiceCatalogEntry, VoiceName } from '../api/types';
import { voiceConfigApi } from '../api/voiceConfig';

export function VoiceConfigPage() {
  const qc = useQueryClient();
  const [previewError, setPreviewError] = useState<string | null>(null);
  const audioUrlRef = useRef<string | null>(null);
  const audioElRef = useRef<HTMLAudioElement | null>(null);

  const catalog = useQuery({ queryKey: ['voice-catalog'], queryFn: voiceConfigApi.catalog });

  const setActiveMut = useMutation({
    mutationFn: voiceConfigApi.setActive,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['voice-catalog'] }),
  });

  const previewMut = useMutation({
    mutationFn: voiceConfigApi.preview,
    onSuccess: (blob) => {
      // Revoke prior URL to avoid leaking object URLs.
      if (audioUrlRef.current) URL.revokeObjectURL(audioUrlRef.current);
      audioUrlRef.current = URL.createObjectURL(blob);
      const el = audioElRef.current ?? new Audio();
      el.src = audioUrlRef.current;
      el.play().catch((err) => setPreviewError(err.message));
      audioElRef.current = el;
    },
    onError: (err) => setPreviewError(describeApiError(err)),
  });

  // Clean up audio playback + object URL when leaving the page.
  useEffect(() => {
    return () => {
      audioElRef.current?.pause();
      if (audioUrlRef.current) URL.revokeObjectURL(audioUrlRef.current);
    };
  }, []);

  const handleSelect = (id: VoiceName) => setActiveMut.mutate(id);
  const handlePreview = (id: VoiceName) => {
    setPreviewError(null);
    previewMut.mutate(id);
  };

  return (
    <div>
      <h1>Voices</h1>
      <p className="page-subtitle">
        Pick the active concierge voice. Changes apply to new conversations — current sessions
        keep their voice. Preview each option before selecting.
      </p>

      {catalog.isLoading && <div className="muted">Loading…</div>}
      {catalog.error && <div className="error">{describeApiError(catalog.error)}</div>}
      {previewError && <div className="error" style={{ marginBottom: 16 }}>Preview failed: {previewError}</div>}

      {catalog.data && (
        <div className="voice-grid">
          {catalog.data.map((v) => (
            <VoiceCard
              key={v.id}
              entry={v}
              busy={setActiveMut.isPending || previewMut.isPending}
              previewingId={previewMut.isPending ? (previewMut.variables ?? null) : null}
              onSelect={() => handleSelect(v.id)}
              onPreview={() => handlePreview(v.id)}
            />
          ))}
        </div>
      )}
    </div>
  );
}

interface VoiceCardProps {
  entry: VoiceCatalogEntry;
  busy: boolean;
  previewingId: VoiceName | null;
  onSelect: () => void;
  onPreview: () => void;
}

function VoiceCard({ entry, busy, previewingId, onSelect, onPreview }: VoiceCardProps) {
  const isPreviewing = previewingId === entry.id;
  return (
    <div className={`voice-card${entry.isActive ? ' active' : ''}`}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div className="voice-name">{entry.name}</div>
        {entry.isActive && <span className="badge active">Active</span>}
      </div>
      <div className="voice-description">{entry.description}</div>
      <div className="voice-actions">
        <button onClick={onPreview} disabled={busy}>
          {isPreviewing ? 'Loading…' : '▶ Preview'}
        </button>
        {!entry.isActive && (
          <button className="primary" onClick={onSelect} disabled={busy}>
            Select
          </button>
        )}
      </div>
    </div>
  );
}
