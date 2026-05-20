// Mirrors VoiceConcierge.Data.Enums.VoiceId (string serialization via JsonStringEnumConverter).
export type VoiceName = 'James' | 'Sofia' | 'Marcus' | 'Elena';

export type UnansweredStatus = 'Pending' | 'Dismissed' | 'Converted';

export interface FaqModel {
  id: string;
  question: string;
  answer: string;
  category: string;
  createdAt: string;
  updatedAt: string;
}

export interface FaqSearchResultModel {
  faq: FaqModel;
  score: number;
}

export interface UnansweredQuestionModel {
  id: string;
  question: string;
  frequency: number;
  status: UnansweredStatus;
  firstAskedAt: string;
  lastAskedAt: string;
  convertedToFaqId: string | null;
}

export interface VoiceConfigModel {
  activeVoiceId: VoiceName;
  updatedAt: string;
}

export interface VoiceCatalogEntry {
  id: VoiceName;
  name: string;
  description: string;
  isActive: boolean;
}

export interface LiveKitTokenModel {
  token: string;
  url: string;
  roomName: string;
  participantIdentity: string;
  participantDisplayName: string;
}
