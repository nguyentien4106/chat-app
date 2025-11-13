import { useEffect, useRef } from 'react';

interface UseAutoScrollProps {
  isLoadingMessages: boolean;
  messages: any[];
  currentUserId?: string;
}

export const useAutoScroll = ({ isLoadingMessages, messages, currentUserId }: UseAutoScrollProps) => {
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const previousMessageCountRef = useRef(0);
  const shouldAutoScrollRef = useRef(true);

  useEffect(() => {
    // Only auto-scroll if we're not currently loading more messages
    if (!isLoadingMessages && messages.length > 0) {
      const newMessageCount = messages.length - previousMessageCountRef.current;
      
      // If messages length changed
      if (newMessageCount !== 0) {
        const lastMessage = messages[messages.length - 1];
        const isOwnMessage = currentUserId && lastMessage.senderId === currentUserId;
        
        if (isOwnMessage || shouldAutoScrollRef.current) {
          messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
          shouldAutoScrollRef.current = false;
        }
      }
      
      previousMessageCountRef.current = messages.length;
    }
  }, [messages, isLoadingMessages, currentUserId]);

  // Reset auto-scroll flag when messages are cleared (chat changed)
  useEffect(() => {
    if (messages.length === 0) {
      shouldAutoScrollRef.current = true;
      previousMessageCountRef.current = 0;
    }
  }, [messages.length]);

  return { messagesEndRef };
};
