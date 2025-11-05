import React, { useRef, useCallback, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Users, UserPlus, Loader2 } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";

export const GroupsList: React.FC = () => {
  const [addUserName, setAddUserName] = React.useState("");
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
  const handleAddMemberClick = (groupId: string) => {
    handleAddMember(groupId, addUserName);
    setAddUserName("");
  };

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
          <div
            key={group.id}
            onClick={() =>
              handleChatSelect({
                id: group.id,
                name: group.name,
                userFullName: group.name,
                type: "group",
                groupId: group.id,
              })
            }
            className={`p-4 border-b cursor-pointer hover:bg-muted/50 ${
              activeChat?.id === group.id ? "bg-accent" : ""
            }`}
          >
            <div className="flex items-center space-x-3">
              <Avatar>
                <AvatarFallback>
                  <Users className="w-4 h-4" />
                </AvatarFallback>
              </Avatar>
              <div className="flex-1">
                <p className="font-medium">{group.name}</p>
                <p className="text-sm text-gray-500">
                  {group.memberCount} members
                </p>
              </div>
              <Dialog>
                <DialogTrigger asChild>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <UserPlus className="w-4 h-4" />
                  </Button>
                </DialogTrigger>
                <DialogContent onClick={(e) => e.stopPropagation()}>
                  <DialogHeader>
                    <DialogTitle>Manage Group</DialogTitle>
                  </DialogHeader>
                  <div className="space-y-4">
                    <div>
                      <p className="text-sm font-medium mb-2">Add Member</p>
                      <div className="flex space-x-2">
                        <Input
                          placeholder="User Name"
                          value={addUserName}
                          onChange={(e) => setAddUserName(e.target.value)}
                        />
                        <Button onClick={() => handleAddMemberClick(group.id)}>
                          Add
                        </Button>
                      </div>
                    </div>
                    <div>
                      <p className="text-sm font-medium mb-2">Invite Link</p>
                      <Button
                        onClick={() => handleGenerateInvite(group.id)}
                        className="w-full"
                      >
                        Generate Invite Code
                      </Button>
                    </div>
                  </div>
                </DialogContent>
              </Dialog>
            </div>
          </div>
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
