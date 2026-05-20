import axios, { AxiosError } from 'axios';

const client = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
  timeout: 15_000,
});

export type QueryParams = Record<string, string | number | boolean | undefined>;

export type ApiSuccess<T> = { data: T };

export type ApiValidationError = {
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
  detail?: string;
};

/** Extract a human-friendly message from a ProblemDetails-shaped error response. */
export function describeApiError(err: unknown): string {
  if (err instanceof AxiosError && err.response) {
    const body = err.response.data as ApiValidationError | undefined;
    if (body?.errors) {
      const all = Object.values(body.errors).flat();
      if (all.length > 0) return all.join('; ');
    }
    return body?.detail || body?.title || err.message;
  }
  return err instanceof Error ? err.message : String(err);
}

/** GET that unwraps the {data: T} envelope. */
export async function getData<T>(url: string, params?: QueryParams): Promise<T> {
  const resp = await client.get<ApiSuccess<T>>(url, { params });
  return resp.data.data;
}

/** POST that unwraps the {data: T} envelope. */
export async function postData<T>(url: string, body?: unknown): Promise<T> {
  const resp = await client.post<ApiSuccess<T>>(url, body);
  return resp.data.data;
}

/** PUT that unwraps the {data: T} envelope. */
export async function putData<T>(url: string, body?: unknown): Promise<T> {
  const resp = await client.put<ApiSuccess<T>>(url, body);
  return resp.data.data;
}

/** DELETE returning nothing (204 NoContent). */
export async function deleteVoid(url: string): Promise<void> {
  await client.delete(url);
}

export default client;
