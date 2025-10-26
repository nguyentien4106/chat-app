import { Group, InviteLinkResponse } from "@/types/chat.types";
import { apiService } from "./api";

export const groupService = {
  getUserGroups: () => apiService.get<Group[]>('/api/groups'),
  
  createGroup: (data: { name: string; description?: string }) =>
    apiService.post('/api/groups', data),

  addMember: (groupId: string, userId: string) =>
    apiService.post(`/api/groups/${groupId}/members`, { userId }),

  generateInviteLink: (groupId: string) =>
    apiService.post<InviteLinkResponse>(`/api/groups/${groupId}/invite`),

  joinByInvite: (inviteCode: string) =>
    apiService.post(`/api/groups/join/${inviteCode}`)
};