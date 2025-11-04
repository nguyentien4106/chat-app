
// src/hooks/useChat.ts
import { useState, useCallback, useRef, use } from 'react';
import type { Conversation, Group, Message } from '@/types/chat.types';
import { conversationService } from '@/services/conversationService';
import { groupService } from '@/services/groupService';
import { messageService } from '@/services/messageService';
import { useAuth } from '@/contexts/AuthContext';
import { PaginationRequest } from '@/types';
import { defaultPaginationRequest } from '@/constants';

interface UseChatReturn {
  conversations: Conversation[];
  isLoadingConversations: boolean;
  hasMoreConversations: boolean;

  groups: Group[];
  isLoadingGroups: boolean;

  messages: Message[];
  messagesPagination: PaginationRequest;
  isLoadingMessages: boolean;
  hasMoreMessages: boolean;
  
  loadConversations: (loadMore?: boolean) => Promise<void>;
  addConversation: (conversation: Conversation) => void;
  loadGroups: () => Promise<void>;
  loadMessages: (chatId: string, type: 'user' | 'group', loadMore?: boolean) => Promise<void>;
  addMessage: (message: Message) => void;
  clearMessages: () => void;
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
  
  const [isLoadingGroups, setIsLoadingGroups] = useState<boolean>(true);
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

  const loadMessages = useCallback(async (chatId: string, type: 'user' | 'group', loadMore: boolean = false) => {
    try {
      // Prevent loading more if already loading or no more messages
      if (loadMore && (isLoadingMessages || !hasMoreMessages)) {
        return;
      }

      setIsLoadingMessages(true);
      
      if (type === 'user') {
        // Determine pagination
        const pagination = loadMore
          ? { ...messagesPagination, pageNumber: messagesPagination.pageNumber + 1 }
          : { ...defaultPaginationRequest, pageNumber: 1 };

        const result = await conversationService.getConversationMessages(chatId, pagination);
        
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
        
        setMessagesPagination(pagination);
        setHasMoreMessages(result.hasNextPage);
        return;
      }

      // Group messages
      const msgs = await messageService.getGroupMessages(chatId);
      setMessages(msgs);
      setHasMoreMessages(false); // Group messages don't have pagination yet
      
    } catch (error) {
      console.error('Error loading messages:', error);
    } finally {
      setIsLoadingMessages(false);
    }
  }, [isLoadingMessages, hasMoreMessages, messagesPagination]);

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
    messages,
    messagesPagination,
    isLoadingMessages,
    hasMoreMessages,
    loadConversations,
    addConversation,
    loadGroups,
    loadMessages,
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
