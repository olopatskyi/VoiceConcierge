import { FormEvent, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { describeApiError } from '../api/client';
import { CreateFaqBody, faqApi } from '../api/faq';
import type { FaqModel } from '../api/types';

export function FaqPage() {
  const qc = useQueryClient();
  const [editing, setEditing] = useState<FaqModel | null>(null);
  const [creating, setCreating] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);

  const list = useQuery({ queryKey: ['faq'], queryFn: faqApi.getMany });

  const deleteMut = useMutation({
    mutationFn: faqApi.remove,
    onSuccess: () => {
      setActionError(null);
      qc.invalidateQueries({ queryKey: ['faq'] });
    },
    onError: (err) => setActionError(describeApiError(err)),
  });

  const handleDelete = (faq: FaqModel) => {
    if (window.confirm(`Delete FAQ "${faq.question}"?`)) {
      setActionError(null);
      deleteMut.mutate(faq.id);
    }
  };

  return (
    <div>
      <div className="toolbar">
        <div>
          <h1>FAQs</h1>
          <p className="page-subtitle" style={{ marginBottom: 0 }}>
            Knowledge base entries the concierge uses to answer guests.
          </p>
        </div>
        <div className="spacer" />
        <button className="primary" onClick={() => setCreating(true)}>
          + Add FAQ
        </button>
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
                <th>Question</th>
                <th>Answer</th>
                <th>Category</th>
                <th style={{ width: 140 }}></th>
              </tr>
            </thead>
            <tbody>
              {list.data.length === 0 && (
                <tr>
                  <td colSpan={4} className="muted" style={{ textAlign: 'center', padding: 24 }}>
                    No FAQs yet — add one to get started.
                  </td>
                </tr>
              )}
              {list.data.map((faq) => (
                <tr key={faq.id}>
                  <td style={{ maxWidth: 280 }}>{faq.question}</td>
                  <td className="muted" style={{ maxWidth: 420 }}>
                    {faq.answer.length > 140 ? `${faq.answer.slice(0, 140)}…` : faq.answer}
                  </td>
                  <td>
                    <span className="badge">{faq.category}</span>
                  </td>
                  <td>
                    <div className="row-actions">
                      <button onClick={() => setEditing(faq)}>Edit</button>
                      <button className="danger" onClick={() => handleDelete(faq)}>
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {(creating || editing) && (
        <FaqDialog
          key={editing?.id ?? 'new'}
          initial={editing}
          onClose={() => {
            setCreating(false);
            setEditing(null);
          }}
        />
      )}
    </div>
  );
}

interface FaqDialogProps {
  initial: FaqModel | null;
  onClose: () => void;
}

function FaqDialog({ initial, onClose }: FaqDialogProps) {
  const qc = useQueryClient();
  const [form, setForm] = useState<CreateFaqBody>({
    question: initial?.question ?? '',
    answer: initial?.answer ?? '',
    category: initial?.category ?? '',
  });
  const [submitError, setSubmitError] = useState<string | null>(null);

  const isEdit = initial !== null;

  const mut = useMutation({
    mutationFn: (body: CreateFaqBody) =>
      initial ? faqApi.update(initial.id, body) : faqApi.create(body),
    onSuccess: () => {
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
        <h2>{isEdit ? 'Edit FAQ' : 'Add FAQ'}</h2>

        <div className="field">
          <label>Question</label>
          <input
            value={form.question}
            onChange={(e) => setForm({ ...form, question: e.target.value })}
            required
            maxLength={1000}
          />
        </div>

        <div className="field">
          <label>Answer</label>
          <textarea
            value={form.answer}
            onChange={(e) => setForm({ ...form, answer: e.target.value })}
            required
            maxLength={4000}
            rows={6}
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
            {mut.isPending ? 'Saving…' : isEdit ? 'Save' : 'Create'}
          </button>
        </div>
      </form>
    </div>
  );
}
