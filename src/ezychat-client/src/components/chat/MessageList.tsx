import React, { useRef, useEffect } from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageItem } from "./MessageItem";
import { useChatContext } from "@/contexts/ChatContext";

export const MessageList: React.FC = () => {
  const { 
    activeChat, 
    messages, 
    currentUserId, 
    messagesEndRef, 
    isLoadingMessages, 
    hasMoreMessages, 
    handleLoadMoreMessages 
  } = useChatContext();
  
  const scrollAreaRef = useRef<HTMLDivElement>(null);
  const isLoadingMoreRef = useRef(false);
  const canLoadMoreRef = useRef(false);
  const activeChatIdRef = useRef<string | null>(null);
  const hasScrolledDownRef = useRef(false);

  // Track when active chat changes and prevent immediate threshold trigger
  useEffect(() => {
    const currentChatId = activeChat?.id || null;
    
    if (currentChatId !== activeChatIdRef.current) {
      // Chat has changed, reset flags
      canLoadMoreRef.current = false;
      hasScrolledDownRef.current = false;
      activeChatIdRef.current = currentChatId;
    }
  }, [activeChat?.id]);

  // Use effect to add scroll listener to the viewport element
  useEffect(() => {
    const scrollArea = scrollAreaRef.current;
    if (!scrollArea) return;
    
    // Find the viewport element within the ScrollArea
    const viewport = scrollArea.querySelector('[data-radix-scroll-area-viewport]') as HTMLDivElement;
    if (!viewport) return;

    const handleScroll = async () => {
      const scrollTop = viewport.scrollTop;
      const threshold = 100; // Load more when within 100px of top

      // Track if user has scrolled down from the initial position
      if (scrollTop > threshold && !hasScrolledDownRef.current) {
        hasScrolledDownRef.current = true;
        canLoadMoreRef.current = true;
      }

      // Check if user scrolled near the top and we have more messages to load
      if (
        scrollTop <= threshold && 
        hasMoreMessages && 
        !isLoadingMessages && 
        !isLoadingMoreRef.current &&
        canLoadMoreRef.current &&
        hasScrolledDownRef.current
      ) {
        isLoadingMoreRef.current = true;
        
        // Store current scroll position to maintain it after loading
        const scrollHeight = viewport.scrollHeight;
        
        try {
          await handleLoadMoreMessages();
          
          // Maintain scroll position after new messages are loaded
          // This prevents the jump to top effect
          requestAnimationFrame(() => {
            if (viewport.scrollHeight > scrollHeight) {
              const newScrollTop = viewport.scrollHeight - scrollHeight + scrollTop;
              viewport.scrollTop = newScrollTop;
            }
          });
        } finally {
          isLoadingMoreRef.current = false;
        }
      }
    };

    viewport.addEventListener('scroll', handleScroll, { passive: true });
    
    return () => {
      viewport.removeEventListener('scroll', handleScroll);
    };
  }, [hasMoreMessages, isLoadingMessages, handleLoadMoreMessages]);

  return (
    <div className="flex-1 overflow-hidden min-h-0">
      <ScrollArea ref={scrollAreaRef} className="h-full p-4">
        <div className="space-y-4">
          {/* Loading indicator for more messages */}
          {isLoadingMessages && hasMoreMessages && (
            <div className="text-center text-muted-foreground py-2">
              Loading more messages...
            </div>
          )}
          
          {messages.length === 0 ? (
            <div className="text-center text-gray-500 mt-8">
              No messages yet. Start the conversation!
            </div>
          ) : (
            messages.map((msg) => (
              <MessageItem
                key={msg.id}
                message={msg}
                isOwn={msg.senderId === currentUserId}
                showSender={activeChat?.type === "group"}
              />
            ))
          )}
          <div ref={messagesEndRef} />
        </div>
      </ScrollArea>
    </div>
  );
};
