import { postData } from './client';
import type { LiveKitTokenModel } from './types';

export interface IssueTokenBody {
  participantName?: string;
  roomName?: string;
}

export const livekitTokenApi = {
  issue: (body?: IssueTokenBody) => postData<LiveKitTokenModel>('/livekit-token', body ?? {}),
};
