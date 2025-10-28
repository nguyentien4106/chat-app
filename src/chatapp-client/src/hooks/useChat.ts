
// src/hooks/useChat.ts
import { useState, useCallback } from 'react';
import type { Conversation, Group, Message } from '@/types/chat.types';
import { conversationService } from '@/services/conversationService';
import { groupService } from '@/services/groupService';
import { messageService } from '@/services/messageService';

interface UseChatReturn {
  conversations: Conversation[];
  groups: Group[];
  messages: Message[];
  loadConversations: () => Promise<void>;
  loadGroups: () => Promise<void>;
  loadMessages: (chatId: string, type: 'user' | 'group') => Promise<void>;
  addMessage: (message: Message) => void;
  createGroup: (name: string, description?: string) => Promise<void>;
  generateInviteLink: (groupId: string) => Promise<string>;
  joinByInvite: (inviteCode: string) => Promise<void>;
  addMemberToGroup: (groupId: string, userName: string) => Promise<void>;
}

export const useChat = (): UseChatReturn => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [groups, setGroups] = useState<Group[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);

  const loadConversations = useCallback(async () => {
    try {
      const result = await conversationService.getUserConversations();
      setConversations(result);
    } catch (error) {
      console.error('Error loading conversations:', error);
    }
  }, []);

  const loadGroups = useCallback(async () => {
    try {
      const result = await groupService.getUserGroups();
      setGroups(result);
    } catch (error) {
      console.error('Error loading groups:', error);
    }
  }, []);

  const loadMessages = useCallback(async (chatId: string, type: 'user' | 'group') => {
    try {
      const response = type === 'user'
        ? await messageService.getConversationMessages(chatId)
        : await messageService.getGroupMessages(chatId);
      
      setMessages(response.reverse());
    } catch (error) {
      console.error('Error loading messages:', error);
    }
  }, []);

  const addMessage = useCallback((message: Message) => {
    setMessages(prev => {
      // Check if message already exists to prevent duplicates
      const messageExists = prev.some(m => m.id === message.id);
      if (messageExists) {
        return prev;
      }
      return [...prev, message];
    });
  }, []);

  const createGroup = useCallback(async (name: string, description?: string) => {
    try {
      const newGroup = await groupService.createGroup({ name, description });
      await loadGroups();
    } catch (error) {
      console.error('Error creating group:', error);
      throw error;
    }
  }, [loadGroups]);

  const generateInviteLink = useCallback(async (groupId: string): Promise<string> => {
    try {
      const response = await groupService.generateInviteLink(groupId);
      return response.inviteCode;
    } catch (error) {
      console.error('Error generating invite:', error);
      throw error;
    }
  }, []);

  const joinByInvite = useCallback(async (inviteCode: string) => {
    try {
      await groupService.joinByInvite(inviteCode);
      await loadGroups();
    } catch (error) {
      console.error('Error joining group:', error);
      throw error;
    }
  }, [loadGroups]);

  const addMemberToGroup = useCallback(async (groupId: string, userName: string) => {
    try {
      await groupService.addMember(groupId, userName);
    } catch (error) {
      console.error('Error adding member:', error);
      throw error;
    }
  }, []);

  return {
    conversations,
    groups,
    messages,
    loadConversations,
    loadGroups,
    loadMessages,
    addMessage,
    createGroup,
    generateInviteLink,
    joinByInvite,
    addMemberToGroup
  };
};
