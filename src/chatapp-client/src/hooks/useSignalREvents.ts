import { useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import type { HubConnection } from '@microsoft/signalr';
import { ActiveChat, Message, Group } from '@/types/chat.types';

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

  useEffect(() => {
    if (!connection) return;

    onReceiveMessage((message) => {
      if (message.receiverId === currentUserId) {
        onLastMessageEvent && onLastMessageEvent(message);
      }
      
      if (
        activeChat &&
        ((activeChat.type === 'user' && (message.receiverId === currentUserId)) ||
          (activeChat.type === 'group' && message.groupId === activeChat.groupId))
      ) {
        onMessagesEvent(message);
      }

      // Show notification for direct messages
      if (currentUserId && message.receiverId === currentUserId && message.senderId !== currentUserId) {
        toast.info(message.senderUserName, {
          description: message.content,
          duration: 5000,
        });
      }
      
      // Show notification for group messages
      if (message.groupId && message.senderId !== currentUserId && hasGroupNotification(message.groupId) && !groupOpening(message.groupId)) {
        toast.info(`${message.groupName}`, {
          description: `${message.senderUserName}: ${message.content}`,
          duration: 5000,
        });
      }
    });
    
    onMemberAdded((data) => {
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
      }

      if (groupOpening(data.groupId)) {
        onMessagesEvent(data.message);
      }
    });
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
