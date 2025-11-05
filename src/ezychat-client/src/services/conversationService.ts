import { Conversation, Message } from '@/types/chat.types';
import { PaginatedResponse, PaginationRequest } from '@/types';
import { apiService } from './api';

export const conversationService = {
  getUserConversations: (params?: PaginationRequest) => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber !== undefined) {
      queryParams.append('pageNumber', params.pageNumber.toString());
    }
    if (params?.pageSize !== undefined) {
      queryParams.append('pageSize', params.pageSize.toString());
    }
    if (params?.sortBy) {
      queryParams.append('sortBy', params.sortBy);
    }
    if (params?.sortOrder) {
      queryParams.append('sortOrder', params.sortOrder);
    }
    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : '';
    return apiService.get<PaginatedResponse<Conversation>>(`/api/conversations${queryString}`);
  },
  createConversation: (userId: string) =>
    apiService.post<Conversation>('/api/conversations', { userId }),
  markAsRead: (conversationId: string) =>
    apiService.post<number>(`/api/conversations/${conversationId}/mark-read`, {}),
  getConversationMessages: (conversationId: string, params?: PaginationRequest) => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber !== undefined) {
        queryParams.append('pageNumber', params.pageNumber.toString());
    }
    queryParams.append('pageSize', "20");
    queryParams.append('sortOrder', 'desc');
    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : '';
    return apiService.get<PaginatedResponse<Message>>(
      `/api/conversations/${conversationId}/messages${queryString}`
    );
  },
};