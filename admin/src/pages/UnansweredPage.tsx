import { FormEvent, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { describeApiError } from '../api/client';
import { ConvertToFaqBody, unansweredApi } from '../api/unanswered';
import type { UnansweredQuestionModel, UnansweredStatus } from '../api/types';

const statusOptions: UnansweredStatus[] = ['Pending', 'Dismissed', 'Converted'];

export function UnansweredPage() {
  const qc = useQueryClient();
  const [statusFilter, setStatusFilter] = useState<UnansweredStatus>('Pending');
  const [converting, setConverting] = useState<UnansweredQuestionModel | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  const list = useQuery({
    queryKey: ['unanswered', statusFilter],
    queryFn: () => unansweredApi.getMany(statusFilter),
  });

  const dismissMut = useMutation({
    mutationFn: unansweredApi.dismiss,
    onSuccess: () => {
      setActionError(null);
      qc.invalidateQueries({ queryKey: ['unanswered'] });
    },
    onError: (err) => setActionError(describeApiError(err)),
  });

  const handleDismiss = (item: UnansweredQuestionModel) => {
    if (window.confirm(`Dismiss "${item.question}"? This removes it from the queue.`)) {
      setActionError(null);
      dismissMut.mutate(item.id);
    }
  };

  return (
    <div>
      <div className="toolbar">
        <div>
          <h1>Unanswered Queue</h1>
          <p className="page-subtitle" style={{ marginBottom: 0 }}>
            Questions the concierge couldn't answer. Convert to FAQ or dismiss as irrelevant.
          </p>
        </div>
        <div className="spacer" />
        <label className="muted">Status:</label>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value as UnansweredStatus)}
          style={{ width: 'auto' }}
        >
          {statusOptions.map((s) => (
            <option key={s} value={s}>
              {s}
            </option>
          ))}
        </select>
      </div>

      {actionError && (
        <div className="error" style={{ marginBottom: 16 }}>{actionError}</div>
      )}
      {list.isLoading && <div className="muted">Loading…</div>}
      {list.error && <div className="error">{describeApiError(list.error)}</div>}

      {list.data && (
        <div className="card" style={{ padding: 0 }}>
          <table>
            <thead>
              <tr>
                <th style={{ width: 80 }}>Frequency</th>
                <th>Question</th>
                <th style={{ width: 160 }}>Last asked</th>
                <th style={{ width: 200 }}></th>
              </tr>
            </thead>
            <tbody>
              {list.data.length === 0 && (
                <tr>
                  <td colSpan={4} className="muted" style={{ textAlign: 'center', padding: 24 }}>
                    {statusFilter === 'Pending'
                      ? 'No pending questions — the concierge has answered everything so far.'
                      : `No ${statusFilter.toLowerCase()} questions.`}
                  </td>
                </tr>
              )}
              {list.data.map((q) => (
                <tr key={q.id}>
                  <td>
                    <span className="badge">{q.frequency}×</span>
                  </td>
                  <td>{q.question}</td>
                  <td className="muted">{new Date(q.lastAskedAt).toLocaleString()}</td>
                  <td>
                    {q.status === 'Pending' ? (
                      <div className="row-actions">
                        <button className="primary" onClick={() => setConverting(q)}>
                          Convert to FAQ
                        </button>
                        <button className="danger" onClick={() => handleDismiss(q)}>
                          Dismiss
                        </button>
                      </div>
                    ) : (
                      <span className="muted">— {q.status} —</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {converting && (
        <ConvertDialog
          key={converting.id}
          item={converting}
          onClose={() => setConverting(null)}
        />
      )}
    </div>
  );
}

interface ConvertDialogProps {
  item: UnansweredQuestionModel;
  onClose: () => void;
}

function ConvertDialog({ item, onClose }: ConvertDialogProps) {
  const qc = useQueryClient();
  const [form, setForm] = useState<ConvertToFaqBody>({ answer: '', category: '' });
  const [submitError, setSubmitError] = useState<string | null>(null);

  const mut = useMutation({
    mutationFn: (body: ConvertToFaqBody) => unansweredApi.convert(item.id, body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['unanswered'] });
      qc.invalidateQueries({ queryKey: ['faq'] });
      onClose();
    },
    onError: (err) => setSubmitError(describeApiError(err)),
  });

  const submit = (e: FormEvent) => {
    e.preventDefault();
    setSubmitError(null);
    mut.mutate(form);
  };

  return (
    <div className="overlay" onClick={onClose}>
      <form className="dialog" onClick={(e) => e.stopPropagation()} onSubmit={submit}>
        <h2>Convert to FAQ</h2>

        <div className="field">
          <label>Question (from guests)</label>
          <input value={item.question} disabled />
        </div>

        <div className="field">
          <label>Answer</label>
          <textarea
            value={form.answer}
            onChange={(e) => setForm({ ...form, answer: e.target.value })}
            required
            maxLength={4000}
            rows={6}
            placeholder="Write the concierge-tone answer that the agent will use."
          />
        </div>

        <div className="field">
          <label>Category</label>
          <input
            value={form.category}
            onChange={(e) => setForm({ ...form, category: e.target.value })}
            required
            maxLength={100}
            placeholder="e.g. gaming, dining, accommodations"
          />
        </div>

        {submitError && <div className="error">{submitError}</div>}

        <div className="dialog-actions">
          <button type="button" onClick={onClose}>
            Cancel
          </button>
          <button type="submit" className="primary" disabled={mut.isPending}>
            {mut.isPending ? 'Creating…' : 'Create FAQ + mark Converted'}
          </button>
        </div>
      </form>
    </div>
  );
}
