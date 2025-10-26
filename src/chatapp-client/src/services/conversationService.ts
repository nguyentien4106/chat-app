import { Conversation } from '@/types/chat.types';
import { apiService } from './api';

export const conversationService = {
  getUserConversations: () => apiService.get<Conversation[]>('/api/conversations')
};