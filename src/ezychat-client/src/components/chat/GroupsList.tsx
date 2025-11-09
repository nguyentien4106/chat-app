import React, { useRef, useCallback, useEffect } from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Loader2 } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";
import { GroupItem } from "./GroupItem";

export const GroupsList: React.FC = () => {
  const observerRef = useRef<IntersectionObserver | null>(null);
  const loadMoreRef = useRef<HTMLDivElement>(null);

  const {
    groups,
    activeChat,
    isLoadingGroups,
    hasMoreGroups,
    loadMoreGroups,
    handleChatSelect,
    handleAddMember,
    handleGenerateInvite,
  } = useChatContext();

  // Set up intersection observer for infinite scroll
  const handleObserver = useCallback(
    (entries: IntersectionObserverEntry[]) => {
      const [target] = entries;
      if (target.isIntersecting && hasMoreGroups && !isLoadingGroups) {
        loadMoreGroups();
      }
    },
    [hasMoreGroups, isLoadingGroups, loadMoreGroups]
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
    <ScrollArea className="flex-1 px-4 min-h-0">
      {groups.length === 0 && isLoadingGroups ? (
        <div className="flex items-center justify-center p-8">
          <Loader2 className="w-6 h-6 animate-spin text-muted-foreground" />
        </div>
      ) : groups.length === 0 ? (
        <div className="p-4 text-center text-muted-foreground">No groups yet</div>
      ) : (
        <>
          {groups.map((group) => (
            <GroupItem
              key={group.id}
              group={group}
              isActive={activeChat?.id === group.id}
              onSelect={() =>
                handleChatSelect({
                  id: group.id,
                  name: group.name,
                  userFullName: group.name,
                  type: "group",
                  groupId: group.id,
                })
              }
              onAddMember={handleAddMember}
              onGenerateInvite={handleGenerateInvite}
            />
          ))}
          
          {/* Infinite scroll trigger */}
          {hasMoreGroups && (
            <div ref={loadMoreRef} className="flex items-center justify-center p-4">
              {isLoadingGroups && (
                <Loader2 className="w-5 h-5 animate-spin text-gray-400" />
              )}
            </div>
          )}
          
          {/* End of list indicator */}
          {!hasMoreGroups && groups.length > 0 && (
            <div className="p-4 text-center text-gray-400 text-sm">
              No more groups
            </div>
          )}
        </>
      )}
    </ScrollArea>
  );
};
