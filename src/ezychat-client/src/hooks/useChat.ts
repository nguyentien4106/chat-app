
// src/hooks/useChat.ts
import { useState, useCallback, useRef } from 'react';
import type { Conversation, Group, Message } from '@/types/chat.types';
import { conversationService } from '@/services/conversationService';
import { groupService } from '@/services/groupService';
import { useAuth } from '@/contexts/AuthContext';
import { PaginationRequest } from '@/types';
import { defaultPaginationRequest } from '@/constants';
import { formatISO } from 'date-fns';

export interface UseChatReturn {
  conversations: Conversation[];
  isLoadingConversations: boolean;
  hasMoreConversations: boolean;

  groups: Group[];
  isLoadingGroups: boolean;
  hasMoreGroups: boolean;

  messages: Message[];
  messagesPagination: PaginationRequest;
  isLoadingMessages: boolean;
  hasMoreMessages: boolean;
  
  loadConversations: (loadMore?: boolean) => Promise<void>;
  addConversation: (conversation: Conversation) => void;
  loadGroups: (loadMore?: boolean) => Promise<void>;
  loadUserMessages: (conversationId: string, loadMore?: boolean) => Promise<void>;
  loadGroupMessages: (groupId: string, loadMore?: boolean) => Promise<void>;
  addMessage: (message: Message) => void;
  clearMessages: () => void;
  createGroup: (name: string, description?: string) => Promise<void>;
  generateInviteLink: (groupId: string) => Promise<string>;
  joinByInvite: (inviteCode: string) => Promise<void>;
  addMemberToGroup: (groupId: string, userName: string) => Promise<void>;
  markConversationAsRead: (conversationId: string, senderId: string) => Promise<void>;

  //Events
  onGroupMemberEvent: (data: { groupId: string; memberCount: number; group?: Group }) => void;
  onGroupEvent: (data: { groupId: string; event: 'createdGroup' | 'removedGroup', group: Group | null }) => void;
  onMessagesEvent: (message: Message) => void;
  onLastMessageEvent?: (message: Message) => void;
}

export const useChat = (): UseChatReturn => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [isLoadingConversations, setIsLoadingConversations] = useState<boolean>(false);
  const isLoadingConversationsRef = useRef<boolean>(false);
  const conversationsPaginationRef = useRef<PaginationRequest>({
    ...defaultPaginationRequest,
    pageSize: 10,
    sortBy: 'LastMessageAt',
    sortOrder: 'desc'
  });
  const [hasMoreConversations, setHasMoreConversations] = useState<boolean>(false);
  const hasMoreConversationsRef = useRef<boolean>(false);
  
  const [isLoadingGroups, setIsLoadingGroups] = useState<boolean>(false);
  const isLoadingGroupsRef = useRef<boolean>(false);
  const groupsPaginationRef = useRef<PaginationRequest>({
    ...defaultPaginationRequest,
    pageSize: 10,
    sortBy: 'CreatedAt',
    sortOrder: 'desc'
  });
  const [hasMoreGroups, setHasMoreGroups] = useState<boolean>(false);
  const hasMoreGroupsRef = useRef<boolean>(false);
  
  const [groups, setGroups] = useState<Group[]>([]);
  const [messages, setMessages] = useState<Message[]>([]);
  const { applicationUserId } = useAuth();
  const [messagesPagination, setMessagesPagination] = useState<PaginationRequest>(defaultPaginationRequest);
  const [isLoadingMessages, setIsLoadingMessages] = useState<boolean>(false);
  const [hasMoreMessages, setHasMoreMessages] = useState<boolean>(false);

  const loadConversations = useCallback(async (loadMore: boolean = false) => {
    try {
      // Prevent loading more if already loading or no more conversations
      if (loadMore && (isLoadingConversationsRef.current || !hasMoreConversationsRef.current)) {
        return;
      }

      isLoadingConversationsRef.current = true;
      setIsLoadingConversations(true);

      // Determine pagination
      const pagination = loadMore
        ? { ...conversationsPaginationRef.current, pageNumber: conversationsPaginationRef.current.pageNumber + 1 }
        : { ...conversationsPaginationRef.current, pageNumber: 1 };

      const result = await conversationService.getUserConversations(pagination);
      
      if (!result || result.items.length === 0) {
        if (!loadMore) {
          setConversations([]);
        }
        hasMoreConversationsRef.current = false;
        setHasMoreConversations(false);
        return;
      }

      // Update conversations
      if (loadMore) {
        setConversations(prev => [...prev, ...result.items]);
      } else {
        setConversations(result.items);
      }

      // Update pagination state
      conversationsPaginationRef.current = pagination;
      hasMoreConversationsRef.current = result.hasNextPage;
      setHasMoreConversations(result.hasNextPage);
      
    } catch (error) {
      console.error('Error loading conversations:', error);
    } finally {
      isLoadingConversationsRef.current = false;
      setIsLoadingConversations(false);
    }
  }, []);

  const loadGroups = useCallback(async (loadMore: boolean = false) => {
    try {
      // Prevent loading more if already loading or no more groups
      if (loadMore && (isLoadingGroupsRef.current || !hasMoreGroupsRef.current)) {
        return;
      }

      isLoadingGroupsRef.current = true;
      setIsLoadingGroups(true);

      // Determine pagination
      const pagination = loadMore
        ? { ...groupsPaginationRef.current, pageNumber: groupsPaginationRef.current.pageNumber + 1 }
        : { ...groupsPaginationRef.current, pageNumber: 1 };

      const result = await groupService.getUserGroups(pagination);
      
      if (!result || result.items.length === 0) {
        if (!loadMore) {
          setGroups([]);
        }
        hasMoreGroupsRef.current = false;
        setHasMoreGroups(false);
        return;
      }

      // Update groups
      if (loadMore) {
        setGroups(prev => [...prev, ...result.items]);
      } else {
        setGroups(result.items);
      }

      // Update pagination state
      groupsPaginationRef.current = pagination;
      hasMoreGroupsRef.current = result.hasNextPage;
      setHasMoreGroups(result.hasNextPage);
      
    } catch (error) {
      console.error('Error loading groups:', error);
    } finally {
      isLoadingGroupsRef.current = false;
      setIsLoadingGroups(false);
    }
  }, []);

  const loadUserMessages = useCallback(async (conversationId: string, loadMore: boolean = false) => {
    try {
      // Prevent loading more if already loading or no more messages
      if (loadMore && (isLoadingMessages || !hasMoreMessages)) {
        return;
      }

      setIsLoadingMessages(true);
      
      // For cursor-based pagination, use the oldest message's createdAt as beforeDateTime
      let beforeDateTime = "";
      if (loadMore && messages.length > 0) {
        // Find the oldest message (first in the array since they're chronological)
        const oldestMessage = messages[0];
        beforeDateTime = formatISO(new Date(oldestMessage.createdAt));
      }

      const result = await conversationService.getConversationMessages(conversationId, beforeDateTime);
      
      if (!result || result.items.length === 0) {
        if (!loadMore) {
          setMessages([]);
          setHasMoreMessages(false);
        } else {
          setHasMoreMessages(false);
        }
        return;
      }

      // Reverse messages to show chronologically (oldest to newest)
      const chronologicalMessages = [...result.items].reverse();
      
      if (loadMore) {
        // Prepend older messages
        setMessages(prev => [...chronologicalMessages, ...prev]);
      } else {
        // Initial load: replace all messages
        setMessages(chronologicalMessages);
      }
      
      setHasMoreMessages(result.hasNextPage);
      
    } catch (error) {
      console.error('Error loading user messages:', error);
    } finally {
      setIsLoadingMessages(false);
    }
  }, [isLoadingMessages, hasMoreMessages, messages]);

  const loadGroupMessages = useCallback(async (groupId: string, loadMore: boolean = false) => {
    try {
      // Prevent loading more if already loading or no more messages
      if (loadMore && (isLoadingMessages || !hasMoreMessages)) {
        return;
      }

      setIsLoadingMessages(true);
      
      // For cursor-based pagination, use the oldest message's createdAt as beforeDateTime
      let beforeDateTime = "";
      if (loadMore && messages.length > 0) {
        // Find the oldest message (first in the array since they're chronological)
        const oldestMessage = messages[0];
        beforeDateTime = formatISO(new Date(oldestMessage.createdAt));
      }

      const result = await groupService.getGroupMessages(groupId, beforeDateTime);
      
      if (!result || result.items.length === 0) {
        if (!loadMore) {
          setMessages([]);
          setHasMoreMessages(false);
        } else {
          setHasMoreMessages(false);
        }
        return;
      }

      // Reverse messages to show chronologically (oldest to newest)
      const chronologicalMessages = [...result.items].reverse();
      
      if (loadMore) {
        // Prepend older messages
        setMessages(prev => [...chronologicalMessages, ...prev]);
      } else {
        // Initial load: replace all messages
        setMessages(chronologicalMessages);
      }
      
      setHasMoreMessages(result.hasNextPage);
      
    } catch (error) {
      console.error('Error loading group messages:', error);
    } finally {
      setIsLoadingMessages(false);
    }
  }, [isLoadingMessages, hasMoreMessages, messages]);

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
        return [group, ...prev];
      } else if (event === 'removedGroup') {
        return prev.filter(g => g.id !== groupId);
      }
      return prev;
    });
  }, []);

  const onGroupMemberEvent = ({ groupId, memberCount }: { groupId: string; memberCount: number, group?: Group }) => {
    setGroups(prev => {
      const updatedGroups = prev.map(g => {
        if (g.id === groupId) {
          return { ...g, memberCount : memberCount  };
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
      return [ conversation, ...prev ];
    });
  }, []);

  const clearMessages = useCallback(() => {
    setMessages([]);
    setMessagesPagination(defaultPaginationRequest);
    setHasMoreMessages(false);
  }, []);

  const markConversationAsRead = useCallback(async (conversationId: string) => {
    try {
      await conversationService.markAsRead(conversationId);
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
    hasMoreConversations,
    groups,
    isLoadingGroups,
    hasMoreGroups,
    messages,
    messagesPagination,
    isLoadingMessages,
    hasMoreMessages,
    loadConversations,
    addConversation,
    loadGroups,
    loadUserMessages,
    loadGroupMessages,
    addMessage,
    clearMessages,
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
