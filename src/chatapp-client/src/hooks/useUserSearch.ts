// src/hooks/useUserSearch.ts
import { useState, useCallback } from 'react';
import { userService } from '@/services/userService';
import type { UserDto } from '@/types/chat.types';

export const useUserSearch = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  const searchUsers = useCallback(async (searchTerm: string) => {
    if (!searchTerm.trim()) {
      setUsers([]);
      return;
    }

    setIsSearching(true);
    try {
      const result = await userService.searchUsers(searchTerm);
      console.log("User search response:", result);
      setUsers(result);
    } catch (error) {
      console.error('Error searching users:', error);
    } finally {
      setIsSearching(false);
    }
  }, []);

  const clearSearch = useCallback(() => {
    setUsers([]);
  }, []);

  return {
    users,
    isSearching,
    searchUsers,
    clearSearch
  };
};