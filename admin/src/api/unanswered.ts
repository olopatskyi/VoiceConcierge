import { getData, postData } from './client';
import type { UnansweredQuestionModel, UnansweredStatus } from './types';

export interface ConvertToFaqBody {
  answer: string;
  category: string;
}

export const unansweredApi = {
  getMany: (status?: UnansweredStatus) =>
    getData<UnansweredQuestionModel[]>('/unanswered', status ? { status } : undefined),

  dismiss: (id: string) => postData<UnansweredQuestionModel>(`/unanswered/${id}/dismiss`),

  convert: (id: string, body: ConvertToFaqBody) =>
    postData<UnansweredQuestionModel>(`/unanswered/${id}/convert`, body),
};
