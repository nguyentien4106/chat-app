
// src/hooks/useChat.ts
import { useState, useCallback } from 'react';
import type { Conversation, Group, Message } from '@/types/chat.types';
import { conversationService } from '@/services/conversationService';
import { groupService } from '@/services/groupService';
import { messageService } from '@/services/messageService';
import { useAuth } from '@/contexts/AuthContext';

interface UseChatReturn {
  conversations: Conversation[];
  isLoadingConversations: boolean;

  groups: Group[];
  isLoadingGroups: boolean;

  messages: Message[];
  loadConversations: () => Promise<void>;
  addConversation: (conversation: Conversation) => void;
  loadGroups: () => Promise<void>;
  loadMessages: (chatId: string, type: 'user' | 'group') => Promise<void>;
  addMessage: (message: Message) => void;
  createGroup: (name: string, description?: string) => Promise<void>;
  generateInviteLink: (groupId: string) => Promise<string>;
  joinByInvite: (inviteCode: string) => Promise<void>;
  addMemberToGroup: (groupId: string, userName: string) => Promise<void>;
  markConversationAsRead: (conversationId: string, senderId: string) => Promise<void>;

  //Events
  onGroupMemberEvent: (data: { groupId: string; event: 'memberAdded' | 'memberRemoved' }) => void;
  onGroupEvent: (data: { groupId: string; event: 'createdGroup' | 'removedGroup', group: Group | null }) => void;
  onMessagesEvent: (message: Message) => void;
  onLastMessageEvent?: (message: Message) => void;
}

export const useChat = (): UseChatReturn => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [isLoadingConversations, setIsLoadingConversations] = useState<boolean>(true);
  const [isLoadingGroups, setIsLoadingGroups] = useState<boolean>(true);
  const [groups, setGroups] = useState<Group[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);
  const { applicationUserId } = useAuth();
  const loadConversations = useCallback(async () => {
    setIsLoadingConversations(true);
    try {
      const result = await conversationService.getUserConversations();
      setIsLoadingConversations(false);
      setConversations(result);
    } catch (error) {
      console.error('Error loading conversations:', error);
    }
    finally {
      setIsLoadingConversations(false);
    }
  }, []);

  const loadGroups = useCallback(async () => {
    setIsLoadingGroups(true);
    try {
      const result = await groupService.getUserGroups();
      setIsLoadingGroups(false);
      setGroups(result);
    } catch (error) {
      console.error('Error loading groups:', error);
    }
    finally {
      setIsLoadingGroups(false);
    }
  }, []);

  const loadMessages = useCallback(async (chatId: string, type: 'user' | 'group') => {
    try {
      const msgs = type === 'user'
        ? await messageService.getConversationMessages(chatId)
        : await messageService.getGroupMessages(chatId);

      if(msgs === null || msgs.length === 0) {
        
        return;
      }

      console.log('Loaded messages:', msgs);

      setMessages(msgs);
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
      onGroupEvent({ groupId: newGroup.id, event: 'createdGroup', group: newGroup });
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

  const onGroupEvent = useCallback(({ groupId, event, group }: { groupId: string; event: 'createdGroup' | 'removedGroup', group: Group | null }) => {
    setGroups(prev => {
      if (event === 'createdGroup' && group != null ) {
        return [...prev, group];
      } else if (event === 'removedGroup') {
        return prev.filter(g => g.id !== groupId);
      }
      return prev;
    });
  }, []);

  const onGroupMemberEvent = ({ groupId, event }: { groupId: string; event: 'memberAdded' | 'memberRemoved' }) => {
    setGroups(prev => {
      const updatedGroups = prev.map(g => {
        if (g.id === groupId) {
          return { ...g, memberCount: event === 'memberAdded' ? g.memberCount + 1 : g.memberCount - 1 };
        }
        return g;
      });
      return updatedGroups;
    });
  }

  const onMessagesEvent = useCallback((message: Message) => {
    addMessage(message);
  }, [addMessage]);

  const onLastMessageEvent = useCallback((message: Message) => {
    console.log('Updating conversations/groups with new message', message);
    if(message.conversationId){
      setConversations(prev => {
        return prev.map(conv => {
          if (conv.id === message.conversationId) {
            return { ...conv, lastMessage: message.content || '', unreadCount: conv.unreadCount + (message.receiverId === applicationUserId ? 1 : 0) };
          }
          return conv;
        });
      });
    }

    if(message.groupId){
      // For group messages, we might want to update the last message in the groups list if needed
      // This part can be implemented as per requirements
      setGroups(prev => {
        return prev.map(group => {
          if (group.id === message.groupId) {
            return { ...group, lastMessage: message };
          }
          return group;
        });
      });
    }
  }, []);

  const addConversation = useCallback((conversation: Conversation) => {
    setConversations(prev => {
      const exists = prev.some(c => c.id === conversation.id);
      if (exists) return prev;
      return [...prev, conversation];
    });
  }, []);

  const markConversationAsRead = useCallback(async (conversationId: string, senderId: string) => {
    try {
      await conversationService.markAsRead(conversationId, senderId);
      // Update local state to reset unread count to 0
      setConversations(prev => 
        prev.map(conv => 
          conv.id === conversationId 
            ? { ...conv, unreadCount: 0 }
            : conv
        )
      );
    } catch (error) {
      console.error('Error marking conversation as read:', error);
    }
  }, []);

  return {
    conversations,
    isLoadingConversations,
    groups,
    isLoadingGroups,
    messages,
    loadConversations,
    addConversation,
    loadGroups,
    loadMessages,
    addMessage,
    createGroup,
    generateInviteLink,
    joinByInvite,
    addMemberToGroup,
    markConversationAsRead,
    onGroupMemberEvent,
    onGroupEvent,
    onMessagesEvent,
    onLastMessageEvent
  };
};
