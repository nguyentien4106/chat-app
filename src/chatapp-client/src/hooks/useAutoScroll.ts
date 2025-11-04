import { useEffect, useRef } from 'react';

interface UseAutoScrollProps {
  isLoadingMessages: boolean;
  messages: any[];
}

export const useAutoScroll = ({ isLoadingMessages, messages }: UseAutoScrollProps) => {
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Only auto-scroll if we're not currently loading more messages
    if (!isLoadingMessages) {
      messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages, isLoadingMessages]);

  return { messagesEndRef };
};
