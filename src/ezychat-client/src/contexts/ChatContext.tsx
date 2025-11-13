import React, { createContext, useContext, useState, useEffect, useRef, ReactNode } from 'react';
import { useSignalR } from '@/hooks/useSignalR';
import { useChat } from '@/hooks/useChat';
import { useFileUpload } from '@/hooks/useFileUpload';
import { useUserSearch } from '@/hooks/useUserSearch';
import { useChatHandlers } from '@/hooks/useChatHandlers';
import { useSignalREvents } from '@/hooks/useSignalREvents';
import { useAutoScroll } from '@/hooks/useAutoScroll';
import { useAuth } from '@/contexts/AuthContext';
import { ActiveChat, Conversation, Message, User } from '@/types/chat.types';
import { JWT_CLAIMS } from '@/constants/jwtClaims';
import { toast } from 'sonner';

interface ChatContextType {
  // User
  user: any;
  currentUserId: string | undefined;
  
  // Connection
  isConnected: boolean;
  
  // Active Chat
  activeChat: ActiveChat | null;
  setActiveChat: (chat: ActiveChat | null) => void;
  
  // Messages
  messages: Message[];
  messageInput: string;
  setMessageInput: (input: string) => void;
  messagesEndRef: React.RefObject<HTMLDivElement | null>;
  messageInputRef: React.RefObject<HTMLInputElement | null>;
  isLoadingMessages: boolean;
  hasMoreMessages: boolean;
  
  // Conversations & Groups
  conversations: Conversation[];
  isLoadingConversations: boolean;
  hasMoreConversations: boolean;
  loadMoreConversations: () => Promise<void>;
  groups: any[];
  isLoadingGroups: boolean;
  hasMoreGroups: boolean;
  loadMoreGroups: () => Promise<void>;

  // File Upload
  isUploading: boolean;
  selectedFile: File | null;
  previewUrl: string | null;
  imageInputRef: React.RefObject<HTMLInputElement | null>;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  
  // User Search
  searchResults: User[];
  isSearching: boolean;
  
  // Handlers
  handleChatSelect: (chat: ActiveChat) => Promise<void>;
  handleSendMessage: () => Promise<void>;
  handleKeyDown: (e: React.KeyboardEvent<HTMLInputElement>) => void;
  handleFileSelect: (event: React.ChangeEvent<HTMLInputElement>, isImage: boolean) => void;
  handleCreateGroup: (name: string, description: string) => Promise<void>;
  handleGenerateInvite: (groupId: string) => Promise<string>;
  handleJoinByInvite: (code: string) => Promise<void>;
  handleAddMember: (groupId: string, userName: string) => Promise<void>;
  handleStartChat: (user: User) => Promise<void>;
  handleImageSelect: () => void;
  handleFileButtonSelect: () => void;
  handleClearFile: () => void;
  handleSearchUsers: (query: string) => void;
  handleClearSearch: () => void;
  handleMarkAsRead: (conversationId: string, senderId: string) => Promise<void>;
  handleLoadMoreMessages: () => Promise<void>;

  // Events

  chatHandlers: ReturnType<typeof useChatHandlers>;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

export const ChatProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  // Auth
  const { user } = useAuth();
  const currentUserId = user?.[JWT_CLAIMS.NAME_IDENTIFIER];
  
  // Local state
  const [activeChat, setActiveChat] = useState<ActiveChat | null>(null);
  const [messageInput, setMessageInput] = useState<string>('');
  
  // Refs
  const fileInputRef = useRef<HTMLInputElement>(null);
  const imageInputRef = useRef<HTMLInputElement>(null);
  const messageInputRef = useRef<HTMLInputElement>(null);
  
  // Custom hooks
  const signalR = useSignalR();
  const chat = useChat();
  const fileUpload = useFileUpload();
  const userSearch = useUserSearch();
  
  // Auto-scroll functionality
  const { messagesEndRef } = useAutoScroll({
    isLoadingMessages: chat.isLoadingMessages,
    messages: chat.messages,
    currentUserId,
  });
  
  // Load initial data
  useEffect(() => {
    if (signalR.isConnected) {
      chat.loadConversations();
      chat.loadGroups();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [signalR.isConnected]);

  // SignalR events
  useSignalREvents({
    connection: signalR.connection,
    activeChat,
    setActiveChat,
    currentUserId,
    groups: chat.groups,
    chat: chat,
    signalR: signalR
  });
  
  // All handlers
  const handlers = useChatHandlers({
    activeChat,
    setActiveChat,
    messageInput,
    setMessageInput,
    messageInputRef,
    signalR: signalR,
    chat: chat,
    fileUpload: fileUpload
  });

  // File handlers
  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>, isImage: boolean): void => {
    try {
        const file = event.target.files?.[0];
        if (!file) return;
        fileUpload.selectFile(file, isImage);
    }
    catch (error) {
        console.error('Error selecting file:', error);
        toast.error('Failed to select file. ' + (error instanceof Error ? error.message : String(error)));
    }
  };

  const handleImageSelect = () => imageInputRef.current?.click();
  const handleFileButtonSelect = () => fileInputRef.current?.click();
  const handleClearFile = () => fileUpload.clearSelection();
  
  // Search handlers
  const handleSearchUsers = (query: string) => userSearch.searchUsers(query);
  const handleClearSearch = () => userSearch.clearSearch();
  
  const value: ChatContextType = {
    // User
    user,
    currentUserId,
    
    // Connection
    isConnected: signalR.isConnected,
    
    // Active Chat
    activeChat,
    setActiveChat,
    
    // Messages
    messages: chat.messages,
    messageInput,
    setMessageInput,
    messagesEndRef,
    messageInputRef,
    isLoadingMessages: chat.isLoadingMessages,
    hasMoreMessages: chat.hasMoreMessages,
    
    // Conversations & Groups
    conversations: chat.conversations,
    isLoadingConversations: chat.isLoadingConversations,
    hasMoreConversations: chat.hasMoreConversations,
    loadMoreConversations: async () => await chat.loadConversations(true),
    groups: chat.groups,
    isLoadingGroups: chat.isLoadingGroups,
    hasMoreGroups: chat.hasMoreGroups,
    loadMoreGroups: async () => await chat.loadGroups(true),
    
    // File Upload
    isUploading: fileUpload.isUploading,
    selectedFile: fileUpload.selectedFile,
    previewUrl: fileUpload.previewUrl,
    imageInputRef,
    fileInputRef,
    
    // User Search
    searchResults: userSearch.users,
    isSearching: userSearch.isSearching,
    
    // Handlers
    handleChatSelect: handlers.handleChatSelect,
    handleSendMessage: handlers.handleSendMessage,
    handleKeyDown: handlers.handleKeyDown,
    handleFileSelect,
    handleCreateGroup: handlers.handleCreateGroup,
    handleGenerateInvite: handlers.handleGenerateInvite,
    handleJoinByInvite: handlers.handleJoinByInvite,
    handleAddMember: handlers.handleAddMember,
    handleStartChat: handlers.handleStartChat,
    handleImageSelect,
    handleFileButtonSelect,
    handleClearFile,
    handleSearchUsers,
    handleClearSearch,
    handleMarkAsRead: handlers.handleMarkAsRead,
    handleLoadMoreMessages: handlers.handleLoadMoreMessages,
    chatHandlers: handlers
  };
  
  return <ChatContext.Provider value={value}>{children}</ChatContext.Provider>;
};

export const useChatContext = () => {
  const context = useContext(ChatContext);
  if (context === undefined) {
    throw new Error('useChatContext must be used within a ChatProvider');
  }
  return context;
};
