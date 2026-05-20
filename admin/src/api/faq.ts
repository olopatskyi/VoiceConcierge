import { deleteVoid, getData, postData, putData } from './client';
import type { FaqModel } from './types';

export interface CreateFaqBody {
  question: string;
  answer: string;
  category: string;
}

export type UpdateFaqBody = CreateFaqBody;

export const faqApi = {
  getMany: () => getData<FaqModel[]>('/faq'),
  getOne: (id: string) => getData<FaqModel>(`/faq/${id}`),
  create: (body: CreateFaqBody) => postData<FaqModel>('/faq', body),
  update: (id: string, body: UpdateFaqBody) => putData<FaqModel>(`/faq/${id}`, body),
  remove: (id: string) => deleteVoid(`/faq/${id}`),
};
