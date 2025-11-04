import { Conversation, Message } from '@/types/chat.types';
import { PaginatedResponse, PaginationRequest } from '@/types';
import { apiService } from './api';

export const conversationService = {
  getUserConversations: () => apiService.get<Conversation[]>('/api/conversations'),
  createConversation: (userId: string) =>
    apiService.post<Conversation>('/api/conversations', { userId }),
  markAsRead: (conversationId: string, senderId: string) =>
    apiService.post<number>(`/api/conversations/${conversationId}/mark-read/${senderId}`, {}),
  getConversationMessages: (conversationId: string, params?: PaginationRequest) => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber !== undefined) {
        queryParams.append('pageNumber', params.pageNumber.toString());
    }
    queryParams.append('pageSize', "50");
    queryParams.append('sortOrder', 'desc');
    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : '';
    return apiService.get<PaginatedResponse<Message>>(
      `/api/conversations/${conversationId}/messages${queryString}`
    );
  },
};