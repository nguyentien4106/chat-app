import { Group, GroupInfo, InviteLinkResponse, Message } from "@/types/chat.types";
import { PaginatedResponse, PaginationRequest } from "@/types";
import { apiService } from "./api";

export const groupService = {
  getUserGroups: (params?: PaginationRequest) => {
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
    return apiService.get<PaginatedResponse<Group>>(`/api/groups${queryString}`);
  },

  getGroupMessages: (groupId: string, params?: PaginationRequest) => {
    const queryParams = new URLSearchParams();
    if (params?.pageNumber !== undefined) {
      queryParams.append('pageNumber', params.pageNumber.toString());
    }
    queryParams.append('pageSize', "20");
    queryParams.append('sortOrder', 'desc');
    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : '';
    return apiService.get<PaginatedResponse<Message>>(
      `/api/groups/${groupId}/messages${queryString}`
    );
  },
  
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
    apiService.delete(`/api/groups/${groupId}/members`, { userId }),

  deleteGroup: (groupId: string) =>
    apiService.delete(`/api/groups/${groupId}`)
};