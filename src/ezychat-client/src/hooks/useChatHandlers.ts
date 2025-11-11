import { useCallback } from 'react';
import { toast } from 'sonner';
import { SendMessageRequest, MessageType, ActiveChat, Conversation, User } from '@/types/chat.types';
import { UseChatReturn } from './useChat';
import { UseSignalRReturn } from './useSignalR';
import { FileUploadReturn } from './useFileUpload';
import { useAuth } from '@/contexts/AuthContext';

interface UseChatHandlersProps {
  activeChat: ActiveChat | null;
  setActiveChat: (chat: ActiveChat | null) => void;
  messageInput: string;
  setMessageInput: (input: string) => void;
  messageInputRef: React.RefObject<HTMLInputElement | null>;
  chat: UseChatReturn,
  signalR: UseSignalRReturn,
  fileUpload: FileUploadReturn
}

export const useChatHandlers = ({
  activeChat,
  setActiveChat,
  messageInput,
  setMessageInput,
  messageInputRef,
  chat,
  signalR,
  fileUpload
}: UseChatHandlersProps) => {
  const { user } = useAuth();
  
  const handleChatSelect = useCallback(async (activeChat: ActiveChat) => {
    setActiveChat(activeChat);
    if(activeChat.id){
      if (activeChat.type === 'user' && activeChat.conversationId) {
        await chat.loadUserMessages(activeChat.conversationId);

      } else if (activeChat.type === 'group' && activeChat.groupId) {
        await chat.loadGroupMessages(activeChat.groupId);
      }
    }

    if (activeChat.type === 'group') {
      //await signalR.joinGroup(activeChat.id);
    } else if (activeChat.type === 'user' && activeChat.conversationId) {
      await chat.markConversationAsRead(activeChat.conversationId, activeChat.id);
    }
    
    setTimeout(() => {
      messageInputRef.current?.focus();
    }, 100);
  }, [setActiveChat, chat.loadUserMessages, chat.loadGroupMessages, signalR.joinGroup, chat.markConversationAsRead, messageInputRef]);

  const handleSendMessage = useCallback(async (): Promise<void> => {
    if ((!messageInput.trim() && !fileUpload.selectedFile) || !activeChat) return;

    try {
      let fileData = null;

      if (fileUpload.selectedFile) {
        fileData = await fileUpload.uploadFile(fileUpload.selectedFile);
      }
      
      const messageData: SendMessageRequest = {
        messageType: fileData ? fileData.messageType : MessageType.Text,
        content: messageInput.trim() || '',
        fileUrl: fileData ? fileData.fileUrl : undefined,
        fileName: fileData ? fileData.fileName : undefined,
        fileSize: fileData ? fileData.fileSize : undefined,
        fileType: fileData ? fileData.fileType : undefined,
        type: activeChat.type,
      };

      if(activeChat.type === 'group'){
        messageData['groupId'] = activeChat.groupId;
        messageData['groupName'] = activeChat.name;
        messageData['senderUserName'] = user?.userName;
      }
      else {
        messageData['receiverId'] = activeChat.receiverId;
        messageData['conversationId'] = activeChat.conversationId;
        messageData['senderUserName'] = user?.userName;
      }
      
      const newMessage = await signalR.sendMessage(messageData);
      chat.addMessage(newMessage!);
      if(newMessage?.isNewConversation){
        chat.updateConversation(newMessage?.conversationId!, newMessage!.receiverId!, newMessage!.content!);
      }
      chat.onLastMessageEvent && chat.onLastMessageEvent(newMessage!);
      setMessageInput('');
      fileUpload.clearSelection();
    } catch (error: any) {
      toast.error('Failed to send message. ' + error.message);
    }
  }, [messageInput, fileUpload.selectedFile, activeChat, fileUpload.uploadFile, signalR.sendMessage, chat.addMessage, chat.onLastMessageEvent, setMessageInput, fileUpload.clearSelection]);

  const handleCreateGroup = useCallback(async (name: string, description: string): Promise<void> => {
    if (!name.trim()) return;
    
    try {
      await chat.createGroup(name, description);
      toast.success('Group created successfully!');
    } catch (error) {
      toast.error('Failed to create group. ' + (error as Error).message);
    }
  }, [chat.createGroup]);

  const handleGenerateInvite = useCallback(async (groupId: string): Promise<string> => {
    try {
      const code = await chat.generateInviteLink(groupId);
      toast.success(`Invite code: ${code}`, {
        description: 'Share this code with others to invite them.',
        duration: 5000,
      });
      return code;
    } catch (error: any) {
      toast.error('Failed to generate invite code. ' + error.message);
      return '';
    }
  }, [chat.generateInviteLink]);

  const handleJoinByInvite = useCallback(async (code: string): Promise<void> => {
    if (!code.trim()) return;
    
    try {
      await chat.joinByInvite(code);
      toast.success('Successfully joined group!');
    } catch (error: any) {
      toast.error(error.message);
    }
  }, [chat.joinByInvite]);

  const handleAddMember = useCallback(async (groupId: string, userName: string): Promise<void> => {
    if (!userName.trim()) return;
    
    try {
      await chat.addMemberToGroup(groupId, userName);
      toast.success('Member added successfully!');
    } catch (error: any) {
      toast.error('Failed to add member.' + error.message);
    }
  }, [chat.addMemberToGroup]);

  const handleKeyDown = useCallback((e: React.KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  }, [handleSendMessage]);

  const handleStartChat = useCallback(async (user: User) => {
    setActiveChat({ 
      id: user.id, 
      name: user.userName, 
      type: 'user',
      conversationId: undefined,
      receiverId: user.id,
      userFullName: user.firstName + ' ' + user.lastName
    });

    const newConv: Conversation = {
      id: undefined,
      userId: user.id,
      userName: user.userName,
      lastMessage: '',
      unreadCount: 0,
      userFullName: user.firstName + ' ' + user.lastName,
    };

    chat.addConversation(newConv);
    chat.clearMessages();
    // Messages will be loaded when first message is sent
  }, [setActiveChat, chat.addConversation, chat.clearMessages]);

  const handleMarkAsRead = useCallback(async (conversationId: string, senderId: string) => {
    await chat.markConversationAsRead(conversationId, senderId);
  }, [chat.markConversationAsRead]);

  const handleLoadMoreMessages = useCallback(async () => {
    if (!activeChat) return;
    
    if (activeChat.type === 'user' && activeChat.conversationId) {
      await chat.loadUserMessages(activeChat.conversationId, true);
    } else if (activeChat.type === 'group' && activeChat.groupId) {
      await chat.loadGroupMessages(activeChat.groupId, true);
    }
  }, [activeChat, chat.loadUserMessages, chat.loadGroupMessages]);

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
