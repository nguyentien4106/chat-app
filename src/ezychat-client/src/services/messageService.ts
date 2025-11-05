import { Message } from '@/types/chat.types';
import { apiService } from './api';

export const messageService = {
  getConversationMessages: (otherUserId: string, skip = 0, take = 50) =>
    apiService.get<Message[]>(`/api/messages/conversation/${otherUserId}`, { params: { skip, take } }),

  getGroupMessages: (groupId: string, skip = 0, take = 50) =>
    apiService.get<Message[]>(`/api/messages/group/${groupId}`, { params: { skip, take } }),

  sendMessage: (data: {
    receiverId?: string;
    groupId?: string; 
    content?: string;
    type: number;
    fileUrl?: string;
    fileName?: string;
    fileType?: string;
    fileSize?: number;
  }) => apiService.post<Message>('/api/messages', data)
};