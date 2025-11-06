import { useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import type { HubConnection } from '@microsoft/signalr';
import { ActiveChat, Message, Group, Conversation } from '@/types/chat.types';

interface UseSignalREventsProps {
  connection: HubConnection | null;
  activeChat: ActiveChat | null;
  setActiveChat: (chat: ActiveChat | null) => void;
  currentUserId: string | undefined;
  groups: Group[];
  onMessagesEvent: (message: Message) => void;
  onLastMessageEvent?: (message: Message) => void;
  onGroupMemberEvent: (data: { groupId: string; event: 'memberAdded' | 'memberRemoved' }) => void;
  onGroupEvent: (data: { groupId: string; event: 'createdGroup' | 'removedGroup', group: Group | null }) => void;
  onReceiveMessage: (callback: (message: Message) => void) => void;
  onMemberAdded: (callback: (data: any) => void) => void;
  onMemberRemoved: (callback: (data: any) => void) => void;
  onMemberLeft: (callback: (data: any) => void) => void;
  onGroupDeleted: (callback: (data: any) => void) => void;
  addConversation: (conversation: Conversation  ) => void;
}

export const useSignalREvents = ({
  connection,
  activeChat,
  setActiveChat,
  currentUserId,
  groups,
  onMessagesEvent,
  onLastMessageEvent,
  onGroupMemberEvent,
  onGroupEvent,
  onReceiveMessage,
  onMemberAdded,
  onMemberRemoved,
  onMemberLeft,
  onGroupDeleted,
  addConversation
}: UseSignalREventsProps) => {

  const isCurrentUser = useCallback((userId: string): boolean => {
    return userId === currentUserId;
  }, [currentUserId]);

  const groupOpening = useCallback((groupId: string): boolean => {
    return activeChat?.type === 'group' && activeChat.groupId === groupId;
  }, [activeChat]);

  const currentUserOpeningGroup = useCallback((groupId: string, userId: string): boolean => {
    return isCurrentUser(userId) && groupOpening(groupId);
  }, [isCurrentUser, groupOpening]);

  const hasGroupNotification = useCallback((groupId: string): boolean => {
    return groups.some(group => group.id === groupId);
  }, [groups]);

  const thisUserReceivedMessage = useCallback((message: Message): boolean => {
    return message.receiverId === currentUserId;
  }, [currentUserId]);

  const thisUserOpeningConversation = useCallback((conversationId?: string): boolean => {
    return activeChat?.type === 'user' && activeChat.id === conversationId;
  }, [activeChat]);

  const thisUserOpeningGroup = useCallback((groupId?: string): boolean => {
    return activeChat?.type === 'group' && activeChat.groupId === groupId;
  }, [activeChat]);

  useEffect(() => {
    if (!connection) return;

    onReceiveMessage((message) => {
      console.log('Received message via SignalR:', message);
      if(message.isNewConversation && thisUserReceivedMessage(message)) {
        // New conversation started
        addConversation({
          id: message.conversationId,
          lastMessage: message.content ?? "",
          lastMessageAt: message.createdAt,
          userId: message.senderId!,
          userName: message.senderUserName!,
          userFullName: message.senderFullName!,
          unreadCount: 1,
        })
      }
      else if (thisUserReceivedMessage(message)) {
        onLastMessageEvent && onLastMessageEvent(message);
      }
      
      if (
        activeChat &&
        ((thisUserOpeningConversation(message.conversationId) && thisUserReceivedMessage(message)) ||
          (thisUserOpeningGroup(message.groupId) && activeChat.id === message.groupId))
      ) {
        onMessagesEvent(message);
      }

      // Show notification for direct messages
      if (currentUserId && thisUserReceivedMessage(message) && message.senderId !== currentUserId) {
        toast.info(message.senderUserName, {
          description: message.content,
          duration: 5000,
        });
      }
      
      // Show notification for group messages
      if (message.groupId && message.senderId !== currentUserId && hasGroupNotification(message.groupId) && !thisUserOpeningGroup(message.groupId)) {
        toast.info(`${message.groupName}`, {
          description: `${message.senderUserName}: ${message.content}`,
          duration: 5000,
        });
      }
    });
    
    onMemberAdded((data) => {
      console.log('Member added via SignalR:', data);
      onGroupMemberEvent({ groupId: data.groupId, event: 'memberAdded' });

      if (groupOpening(data.groupId)) {
        onMessagesEvent(data.message);
      }

      if (isCurrentUser(data.userId)) {
        onGroupEvent({ groupId: data.groupId, event: 'createdGroup', group: data.group });
        toast.success(`You are added to group ${data.group.name}`);
      }
    });

    onMemberRemoved((data) => {
      onGroupMemberEvent({ groupId: data.groupId, event: 'memberRemoved' });
      if (groupOpening(data.groupId)) {
        onMessagesEvent(data.message);
      }

      if (isCurrentUser(data.userId)) {
        onGroupEvent({ groupId: data.groupId, event: 'removedGroup', group: null });
      }

      if (currentUserOpeningGroup(data.groupId, data.userId)) {
        setActiveChat(null);
        toast.info('You have been removed from the group');
      }
    });

    onMemberLeft((data) => {
      onGroupMemberEvent({ groupId: data.groupId, event: 'memberRemoved' });

      if (isCurrentUser(data.userId)) {
        toast.info('You have left the group');
        setActiveChat(null);
        onGroupEvent({ groupId: data.groupId, event: 'removedGroup', group: null });
      }

      if (groupOpening(data.groupId)) {
        onMessagesEvent(data.message);
      }
    });

    onGroupDeleted((data) => {
      onGroupEvent({ groupId: data.groupId, event: 'removedGroup', group: null });
    })
  }, [
    connection,
    activeChat,
    currentUserId,
    groups,
    hasGroupNotification,
    groupOpening,
    isCurrentUser,
    onMessagesEvent,
    onGroupMemberEvent,
    onGroupEvent,
    onReceiveMessage,
    onMemberAdded,
    onMemberRemoved,
    onMemberLeft,
    onLastMessageEvent,
    setActiveChat,
    currentUserOpeningGroup,
  ]);
};
