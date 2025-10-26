import { UserDto } from "@/types/chat.types";
import { apiService } from "./api";

export const userService = {
  searchUsers: (searchTerm: string) =>
    apiService.get<UserDto[]>('/api/users/search?searchTerm=' + encodeURIComponent(searchTerm)),
  
  getUser: (userId: string) =>
    apiService.get(`/api/users/${userId}`)
};