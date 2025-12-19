import { apiService } from './api';
import { QuickMessage, CreateQuickMessageDto, UpdateQuickMessageDto } from '@/types';

export const quickMessageService = {
  getUserQuickMessages: async () => {
    return await apiService.get<QuickMessage[]>('/api/QuickMessages');
  },

  getQuickMessageByKey: async (key: string) => {
    return await apiService.get<QuickMessage>(`/api/QuickMessages/${key}`);
  },

  createQuickMessage: async (dto: CreateQuickMessageDto) => {
    return await apiService.post<QuickMessage>('/api/QuickMessages', dto);
  },

  updateQuickMessage: async (id: string, dto: UpdateQuickMessageDto) => {
    return await apiService.put<QuickMessage>(`/api/QuickMessages/${id}`, dto);
  },

  deleteQuickMessage: async (id: string) => {
    return await apiService.delete<boolean>(`/api/QuickMessages/${id}`);
  },
};
