import client, { getData, putData } from './client';
import type { VoiceCatalogEntry, VoiceConfigModel, VoiceName } from './types';

export const voiceConfigApi = {
  get: () => getData<VoiceConfigModel>('/voice-config'),

  setActive: (activeVoiceId: VoiceName) =>
    putData<VoiceConfigModel>('/voice-config', { activeVoiceId }),

  catalog: () => getData<VoiceCatalogEntry[]>('/voices'),

  /** Returns a Blob (audio/mpeg) playable via <audio src={URL.createObjectURL(blob)}>. */
  preview: async (id: VoiceName): Promise<Blob> => {
    const resp = await client.post(`/voices/${id}/preview`, null, {
      responseType: 'blob',
    });
    return resp.data as Blob;
  },
};
