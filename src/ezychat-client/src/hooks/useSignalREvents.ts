import { useEffect, useCallback } from 'react';
import { toast } from 'sonner';
import type { HubConnection } from '@microsoft/signalr';
import { ActiveChat, Message, Group } from '@/types/chat.types';
import { UseChatReturn } from './useChat';
import { UseSignalRReturn } from './useSignalR';

interface UseSignalREventsProps {
  connection: HubConnection | null;
  activeChat: ActiveChat | null;
  setActiveChat: (chat: ActiveChat | null) => void;
  currentUserId: string | undefined;
  groups: Group[];
  chat: UseChatReturn
  signalR: UseSignalRReturn
}

export const useSignalREvents = ({
  connection,
  activeChat,
  setActiveChat,
  currentUserId,
  groups,
  chat,
  signalR
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

  const updateChatSideBar = useCallback((message: Message) => {
    // Update chat sidebar last message and unread count
    if(message.isNewConversation && thisUserReceivedMessage(message)) {
      // New conversation started
      chat.addConversation({
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
      chat.onLastMessageEvent && chat.onLastMessageEvent(message);
    }
  }, []);

  const updateInsideChat = useCallback((message: Message) => {
    if ((thisUserOpeningConversation(message.conversationId) && thisUserReceivedMessage(message)) ||
        (thisUserOpeningGroup(message.groupId) && activeChat?.id === message.groupId)
    ) {
      chat.onMessagesEvent(message);
    }
  }, [activeChat, chat.onMessagesEvent, thisUserOpeningConversation, thisUserReceivedMessage, thisUserOpeningGroup]);

  const updateNotifications = useCallback((message: Message) => {
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
  }, [currentUserId, hasGroupNotification, thisUserReceivedMessage, thisUserOpeningGroup]);
  
  useEffect(() => {
    if (!connection) return;
    // on message event
    signalR.onReceiveMessage((message) => {
      updateChatSideBar(message);
      updateNotifications(message);

      if(activeChat){
        updateInsideChat(message);
      }

    });
    
    // on group event
    signalR.onGroupHasNewMember((data) => {
      chat.onGroupMemberEvent({ group: data.group, groupId: data.group.id, memberCount: data.group.memberCount });

      if (groupOpening(data.group.id)) {
        chat.onMessagesEvent(data.message);
      }

    });

    signalR.onGroupHasMemberLeft((data) => {
      chat.onGroupMemberEvent({ groupId: data.groupId, memberCount: data.memberCount });
      if (groupOpening(data.groupId)) {
        chat.onMessagesEvent(data.message);
      }
    });

    signalR.onGroupDeleted((data) => {
      chat.onGroupEvent({ groupId: data.groupId, event: 'removedGroup', group: null });
    })

    // onMember event

    signalR.onMemberJoinGroup((data) => {
      if(!isCurrentUser(data.newMemberId)) return;

      chat.onGroupEvent({ groupId: data.group.id, event: 'createdGroup', group: data.group });
      toast.success(`You are added to group ${data.group.name}`);
    });

    signalR.onMemberLeftGroup((data) => {
      if (isCurrentUser(data.removeMemberId)) {
        chat.onGroupEvent({ groupId: data.groupId, event: 'removedGroup', group: null });
      }

      if (groupOpening(data.groupId)) {
        chat.onMessagesEvent(data.message);
      }

      if (currentUserOpeningGroup(data.groupId, data.removeMemberId)) {
        setActiveChat(null);
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
    chat.onMessagesEvent,
    chat.onGroupMemberEvent,
    chat.onGroupEvent,
    chat.onLastMessageEvent,
    signalR.onReceiveMessage,
    signalR.onGroupHasNewMember,
    signalR.onGroupHasMemberLeft,
    setActiveChat,
    currentUserOpeningGroup,
  ]);
};
