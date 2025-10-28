import { Group, GroupInfo, InviteLinkResponse } from "@/types/chat.types";
import { apiService } from "./api";

export const groupService = {
  getUserGroups: () => apiService.get<Group[]>('/api/groups'),
  
  createGroup: (data: { name: string; description?: string }) =>
    apiService.post<Group>('/api/groups', data),

  addMember: (groupId: string, userName: string) =>
    apiService.post(`/api/groups/${groupId}/members`, { userName }),

  generateInviteLink: (groupId: string) =>
    apiService.post<InviteLinkResponse>(`/api/groups/${groupId}/invite`),

  joinByInvite: (inviteCode: string) =>
    apiService.post(`/api/groups/join/${inviteCode}`),

  getGroupInfo: (groupId: string) =>
    apiService.get<GroupInfo>(`/api/groups/${groupId}/info`),

  leaveGroup: (groupId: string) =>
    apiService.post(`/api/groups/${groupId}/leave`),

  removeMember: (groupId: string, userId: string) =>
    apiService.delete(`/api/groups/${groupId}/members`, { userId })
};