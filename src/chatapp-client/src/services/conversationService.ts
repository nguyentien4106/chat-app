import { Conversation } from '@/types/chat.types';
import { apiService } from './api';

export const conversationService = {
  getUserConversations: () => apiService.get<Conversation[]>('/api/conversations'),
  createConversation: (userId: string) =>
    apiService.post<Conversation>('/api/conversations', { userId }),
  markAsRead: (conversationId: string, senderId: string) =>
    apiService.post<number>(`/api/conversations/${conversationId}/mark-read/${senderId}`, {}),
};