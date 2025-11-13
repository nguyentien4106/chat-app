import React, { useRef, useCallback, useEffect } from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Loader2 } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";
import { ConversationItem } from "./ConversationItem";

export const ConversationsList: React.FC = () => {
  const observerRef = useRef<IntersectionObserver | null>(null);
  const loadMoreRef = useRef<HTMLDivElement>(null);

  const {
    conversations,
    isLoadingConversations,
    hasMoreConversations,
    loadMoreConversations,
    activeChat,
    handleChatSelect
  } = useChatContext();

  // Set up intersection observer for infinite scroll
  const handleObserver = useCallback(
    (entries: IntersectionObserverEntry[]) => {
      const [target] = entries;
      if (target.isIntersecting && hasMoreConversations && !isLoadingConversations) {
        loadMoreConversations();
      }
    },
    [hasMoreConversations, isLoadingConversations, loadMoreConversations]
  );

  useEffect(() => {
    const element = loadMoreRef.current;
    const option = {
      root: null,
      rootMargin: "20px",
      threshold: 0
    };

    observerRef.current = new IntersectionObserver(handleObserver, option);

    if (element) {
      observerRef.current.observe(element);
    }

    return () => {
      if (observerRef.current && element) {
        observerRef.current.unobserve(element);
      }
    };
  }, [handleObserver]);

  return (
    <ScrollArea className="h-full px-4">
      {conversations.length === 0 && isLoadingConversations ? (
        <div className="flex items-center justify-center p-8">
          <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
        </div>
      ) : conversations.length === 0 ? (
        <div className="p-4 text-center text-muted-foreground">
          No conversations yet
        </div>
      ) : (
        <>
          {conversations.map((conv) => (
            <ConversationItem
              key={conv.userId}
              conversation={conv}
              isActive={activeChat?.id === conv.id}
              onSelect={() =>
                handleChatSelect({
                  id: conv.id ?? "",
                  name: conv.userName,
                  type: "user",
                  conversationId: conv.id,
                  receiverId: conv.userId,
                  userFullName: conv.userFullName,
                  unreadCount: conv.unreadCount
                })
              }
            />
          ))}
          
          {/* Infinite scroll trigger */}
          {hasMoreConversations && (
            <div ref={loadMoreRef} className="flex items-center justify-center p-4">
              {isLoadingConversations && (
                <Loader2 className="w-5 h-5 animate-spin text-muted-foreground" />
              )}
            </div>
          )}
          
          {/* End of list indicator */}
          {!hasMoreConversations && conversations.length > 0 && (
            <div className="p-4 text-center text-muted-foreground text-sm">
              No more conversations
            </div>
          )}
        </>
      )}
    </ScrollArea>
  );
};
