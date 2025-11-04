import { useCallback } from 'react';
import { toast } from 'sonner';
import { SendMessageRequest, UserDto, Message, MessageType, ActiveChat, Conversation } from '@/types/chat.types';

interface UseChatHandlersProps {
  activeChat: ActiveChat | null;
  setActiveChat: (chat: ActiveChat | null) => void;
  messageInput: string;
  setMessageInput: (input: string) => void;
  selectedFile: File | null;
  messageInputRef: React.RefObject<HTMLInputElement | null>;
  loadMessages: (chatId: string, type: 'user' | 'group', loadMore?: boolean) => Promise<void>;
  sendSignalRMessage: (messageData: SendMessageRequest) => Promise<Message | undefined>;
  addMessage: (message: Message) => void;
  onLastMessageEvent?: (message: Message) => void;
  clearSelection: () => void;
  uploadFile: (file: File) => Promise<any>;
  createGroup: (name: string, description?: string) => Promise<void>;
  generateInviteLink: (groupId: string) => Promise<string>;
  joinByInvite: (inviteCode: string) => Promise<void>;
  addMemberToGroup: (groupId: string, userName: string) => Promise<void>;
  markConversationAsRead: (conversationId: string, senderId: string) => Promise<void>;
  joinGroup: (groupId: string) => Promise<void>;
  addConversation: (conversation: Conversation) => void;
}

export const useChatHandlers = ({
  activeChat,
  setActiveChat,
  messageInput,
  setMessageInput,
  selectedFile,
  messageInputRef,
  loadMessages,
  sendSignalRMessage,
  addMessage,
  onLastMessageEvent,
  clearSelection,
  uploadFile,
  createGroup,
  generateInviteLink,
  joinByInvite,
  addMemberToGroup,
  markConversationAsRead,
  joinGroup,
  addConversation,
}: UseChatHandlersProps) => {
  
  const handleChatSelect = useCallback(async (chat: ActiveChat) => {
    setActiveChat(chat);
    await loadMessages(chat.id, chat.type);
    
    if (chat.type === 'group') {
      await joinGroup(chat.id);
    } else if (chat.type === 'user' && chat.conversationId) {
      await markConversationAsRead(chat.conversationId, chat.id);
    }
    
    setTimeout(() => {
      messageInputRef.current?.focus();
    }, 100);
  }, [setActiveChat, loadMessages, joinGroup, markConversationAsRead, messageInputRef]);

  const handleSendMessage = useCallback(async (): Promise<void> => {
    if ((!messageInput.trim() && !selectedFile) || !activeChat) return;
    
    try {
      let fileData = null;
      
      if (selectedFile) {
        fileData = await uploadFile(selectedFile);
      }
      
      const messageData: SendMessageRequest = {
        messageType: fileData ? fileData.messageType : MessageType.Text,
        content: messageInput.trim() || '',
        groupId: activeChat.type === 'group' ? activeChat.groupId : undefined,
        receiverId: activeChat.type === 'user' ? activeChat.receiverId : undefined,
        conversationId: activeChat.type === 'user' ? activeChat.conversationId : undefined,
        fileUrl: fileData ? fileData.fileUrl : undefined,
        fileName: fileData ? fileData.fileName : undefined,
        fileSize: fileData ? fileData.fileSize : undefined,
        fileType: fileData ? fileData.fileType : undefined,
      };
      
      const newMessage = await sendSignalRMessage(messageData);
      addMessage(newMessage!);
      onLastMessageEvent && onLastMessageEvent(newMessage!);
      setMessageInput('');
      clearSelection();
    } catch (error: any) {
      toast.error('Failed to send message. ' + error.message);
    }
  }, [messageInput, selectedFile, activeChat, uploadFile, sendSignalRMessage, addMessage, onLastMessageEvent, setMessageInput, clearSelection]);

  const handleCreateGroup = useCallback(async (name: string, description: string): Promise<void> => {
    if (!name.trim()) return;
    
    try {
      await createGroup(name, description);
      toast.success('Group created successfully!');
    } catch (error) {
      toast.error('Failed to create group. ' + (error as Error).message);
    }
  }, [createGroup]);

  const handleGenerateInvite = useCallback(async (groupId: string): Promise<string> => {
    try {
      const code = await generateInviteLink(groupId);
      toast.success(`Invite code: ${code}`, {
        description: 'Share this code with others to invite them.',
        duration: 5000,
      });
      return code;
    } catch (error: any) {
      toast.error('Failed to generate invite code. ' + error.message);
      return '';
    }
  }, [generateInviteLink]);

  const handleJoinByInvite = useCallback(async (code: string): Promise<void> => {
    if (!code.trim()) return;
    
    try {
      await joinByInvite(code);
      toast.success('Successfully joined group!');
    } catch (error: any) {
      toast.error('Invalid or expired invite code.' + error.message);
    }
  }, [joinByInvite]);

  const handleAddMember = useCallback(async (groupId: string, userName: string): Promise<void> => {
    if (!userName.trim()) return;
    
    try {
      await addMemberToGroup(groupId, userName);
      toast.success('Member added successfully!');
    } catch (error: any) {
      toast.error('Failed to add member.' + error.message);
    }
  }, [addMemberToGroup]);

  const handleKeyDown = useCallback((e: React.KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  }, [handleSendMessage]);

  const handleStartChat = useCallback(async (user: UserDto) => {
    setActiveChat({ 
      id: user.id, 
      name: user.userName, 
      type: 'user',
      conversationId: undefined,
      receiverId: user.id
    });

    const newConv: Conversation = {
      id: undefined,
      userId: user.id,
      userName: user.userName,
      lastMessage: '',
      unreadCount: 0,
    };

    addConversation(newConv);
    await loadMessages(user.id, 'user');
  }, [setActiveChat, addConversation, loadMessages]);

  const handleMarkAsRead = useCallback(async (conversationId: string, senderId: string) => {
    await markConversationAsRead(conversationId, senderId);
  }, [markConversationAsRead]);

  const handleLoadMoreMessages = useCallback(async () => {
    if (!activeChat) return;
    
    if (activeChat.type === 'user' && activeChat.conversationId) {
      await loadMessages(activeChat.conversationId, 'user', true);
    } else if (activeChat.type === 'group' && activeChat.groupId) {
      await loadMessages(activeChat.groupId, 'group', true);
    }
  }, [activeChat, loadMessages]);

  return {
    handleChatSelect,
    handleSendMessage,
    handleCreateGroup,
    handleGenerateInvite,
    handleJoinByInvite,
    handleAddMember,
    handleKeyDown,
    handleStartChat,
    handleMarkAsRead,
    handleLoadMoreMessages,
  };
};
