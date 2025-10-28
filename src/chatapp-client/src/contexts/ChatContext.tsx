import React, { createContext, useContext, useState, useEffect, useRef, ReactNode } from 'react';
import { toast } from 'sonner';
import { useSignalR } from '@/hooks/useSignalR';
import { useChat } from '@/hooks/useChat';
import { useFileUpload } from '@/hooks/useFileUpload';
import { useUserSearch } from '@/hooks/useUserSearch';
import { useAuth } from '@/contexts/AuthContext';
import { SendMessageRequest, UserDto, Message } from '@/types/chat.types';

// Types
export enum MessageType {
  Text = 0,
  Image = 1,
  File = 2,
}

export interface ActiveChat {
  id: string;
  name: string;
  type: 'user' | 'group';
}

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
  
  // Conversations & Groups
  conversations: any[];
  groups: any[];
  
  // File Upload
  isUploading: boolean;
  selectedFile: File | null;
  previewUrl: string | null;
  imageInputRef: React.RefObject<HTMLInputElement | null>;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  
  // User Search
  searchResults: UserDto[];
  isSearching: boolean;
  
  // Handlers
  handleChatSelect: (chat: ActiveChat) => Promise<void>;
  handleSendMessage: () => Promise<void>;
  handleKeyDown: (e: React.KeyboardEvent<HTMLInputElement>) => void;
  handleFileSelect: (event: React.ChangeEvent<HTMLInputElement>, isImage: boolean) => void;
  handleCreateGroup: (name: string, description: string) => Promise<void>;
  handleGenerateInvite: (groupId: string) => Promise<void>;
  handleJoinByInvite: (code: string) => Promise<void>;
  handleAddMember: (groupId: string, userName: string) => Promise<void>;
  handleStartChat: (user: UserDto) => Promise<void>;
  handleImageSelect: () => void;
  handleFileButtonSelect: () => void;
  handleClearFile: () => void;
  handleSearchUsers: (query: string) => void;
  handleClearSearch: () => void;
}

const ChatContext = createContext<ChatContextType | undefined>(undefined);

export const ChatProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  // Auth
  const { user } = useAuth();
  const currentUserId = user?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
  
  // Custom hooks
  const {
    connection,
    isConnected,
    sendMessage: sendSignalRMessage,
    joinGroup,
    onReceiveMessage,
    onMemberAdded,
  } = useSignalR();
  
  const {
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
    addMemberToGroup,
  } = useChat();
  
  const {
    isUploading,
    selectedFile,
    previewUrl,
    selectFile,
    clearSelection,
    uploadFile,
  } = useFileUpload();
  
  const {
    users: searchResults,
    isSearching,
    searchUsers,
    clearSearch,
  } = useUserSearch();
  
  // Local state
  const [activeChat, setActiveChat] = useState<ActiveChat | null>(null);
  const [messageInput, setMessageInput] = useState<string>('');
  
  // Refs
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const imageInputRef = useRef<HTMLInputElement>(null);
  
  // Load initial data
  useEffect(() => {
    if (isConnected) {
      loadConversations();
      loadGroups();
    }
  }, [isConnected, loadConversations, loadGroups]);
  
  // SignalR message listener
  useEffect(() => {
    if (connection) {
      onReceiveMessage((message) => {
        console.log('Message received:', message);
        
        if (
          activeChat &&
          ((activeChat.type === 'user' &&
            (message.senderId === activeChat.id || message.receiverId === activeChat.id)) ||
            (activeChat.type === 'group' && message.groupId === activeChat.id))
        ) {
          addMessage(message);
        }
        
      });
      
      onMemberAdded((data) => {
        if (activeChat?.type === 'group' && activeChat.id === data.groupId) {
          loadGroups();
        }
      });
    }
  }, [connection, activeChat, onReceiveMessage, onMemberAdded, addMessage, loadConversations, loadGroups]);
  
  // Auto-scroll to bottom
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);
  
  // Handlers
  const handleChatSelect = async (chat: ActiveChat) => {
    setActiveChat(chat);
    await loadMessages(chat.id, chat.type);
    
    if (chat.type === 'group') {
      await joinGroup(chat.id);
    }
  };
  
  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>, isImage: boolean): void => {
    const file = event.target.files?.[0];
    if (!file) return;
    
    try {
      selectFile(file, isImage);
    } catch (error) {
      toast.error((error as Error).message);
    }
  };
  
  const handleSendMessage = async (): Promise<void> => {
    if ((!messageInput.trim() && !selectedFile) || !activeChat) return;
    
    try {
      let fileData = null;
      
      if (selectedFile) {
        fileData = await uploadFile(selectedFile);
      }
      
      const messageData: SendMessageRequest = {
        messageType: fileData ? fileData.messageType : MessageType.Text,
        content: messageInput.trim() || '',
        groupId: activeChat.type === 'user' ? undefined : activeChat.id,
        receiverId: activeChat.type === 'user' ? activeChat.id : undefined,
        fileUrl: fileData ? fileData.fileUrl : undefined,
        fileName: fileData ? fileData.fileName : undefined,
        fileSize: fileData ? fileData.fileSize : undefined,
        fileType: fileData ? fileData.fileType : undefined,
      };
      
      const newMessage = await sendSignalRMessage(messageData);
      addMessage(newMessage!);
      setMessageInput('');
      clearSelection();
    } catch (error) {
      console.error('Error sending message:', error);
      toast.error('Failed to send message');
    }
  };
  
  const handleCreateGroup = async (name: string, description: string): Promise<void> => {
    if (!name.trim()) return;
    
    try {
      await createGroup(name, description);
      toast.success('Group created successfully!');
    } catch (error) {
      toast.error('Failed to create group');
    }
  };
  
  const handleGenerateInvite = async (groupId: string): Promise<void> => {
    try {
      const code = await generateInviteLink(groupId);
      toast.success(`Invite code: ${code}`, {
        description: 'Share this code with others to invite them.',
        duration: 5000,
      });
    } catch (error) {
      toast.error('Failed to generate invite code');
    }
  };
  
  const handleJoinByInvite = async (code: string): Promise<void> => {
    if (!code.trim()) return;
    
    try {
      await joinByInvite(code);
      toast.success('Successfully joined group!');
    } catch (error) {
      toast.error('Invalid or expired invite code');
    }
  };
  
  const handleAddMember = async (groupId: string, userName: string): Promise<void> => {
    if (!userName.trim()) return;
    
    try {
      await addMemberToGroup(groupId, userName);
      toast.success('Member added successfully!');
    } catch (error) {
      toast.error('Failed to add member. Check permissions or user ID.');
    }
  };
  
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };
  
  const handleStartChat = async (user: UserDto) => {
    setActiveChat({ id: user.id, name: user.username, type: 'user' });
    await loadMessages(user.id, 'user');
  };
  
  const handleImageSelect = () => imageInputRef.current?.click();
  const handleFileButtonSelect = () => fileInputRef.current?.click();
  const handleClearFile = () => clearSelection();
  const handleSearchUsers = (query: string) => searchUsers(query);
  const handleClearSearch = () => clearSearch();
  
  const value: ChatContextType = {
    // User
    user,
    currentUserId,
    
    // Connection
    isConnected,
    
    // Active Chat
    activeChat,
    setActiveChat,
    
    // Messages
    messages,
    messageInput,
    setMessageInput,
    messagesEndRef,
    
    // Conversations & Groups
    conversations,
    groups,
    
    // File Upload
    isUploading,
    selectedFile,
    previewUrl,
    imageInputRef,
    fileInputRef,
    
    // User Search
    searchResults,
    isSearching,
    
    // Handlers
    handleChatSelect,
    handleSendMessage,
    handleKeyDown,
    handleFileSelect,
    handleCreateGroup,
    handleGenerateInvite,
    handleJoinByInvite,
    handleAddMember,
    handleStartChat,
    handleImageSelect,
    handleFileButtonSelect,
    handleClearFile,
    handleSearchUsers,
    handleClearSearch,
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
