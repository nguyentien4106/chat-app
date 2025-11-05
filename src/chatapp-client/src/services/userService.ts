import { User } from "@/types/chat.types";
import { apiService } from "./api";

export const userService = {
  searchUsers: (searchTerm: string) =>
    apiService.get<User[]>('/api/users/search?searchTerm=' + encodeURIComponent(searchTerm)),
  
  getUser: (userId: string) =>
    apiService.get(`/api/users/${userId}`)
};