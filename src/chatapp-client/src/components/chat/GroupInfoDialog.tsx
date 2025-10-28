import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { GroupInfo } from "@/types/chat.types";
import { groupService } from "@/services/groupService";
import { Loader2, Users, Calendar, Shield, LogOut, Trash2 } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { useChatContext } from "@/contexts/ChatContext";
import { toast } from "sonner";
import { JWT_CLAIMS } from "@/constants/jwtClaims";

interface GroupInfoDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  groupId: string;
}

export const GroupInfoDialog: React.FC<GroupInfoDialogProps> = ({
  open,
  onOpenChange,
  groupId,
}) => {
  const [groupInfo, setGroupInfo] = useState<GroupInfo | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [removingMemberId, setRemovingMemberId] = useState<string | null>(null);
  const [isLeaving, setIsLeaving] = useState(false);
  const { user } = useAuth();
  const { setActiveChat } = useChatContext();

  const currentUserId = user?.[JWT_CLAIMS.NAME_IDENTIFIER];
  const currentUserMember = groupInfo?.members.find(m => m.userId === currentUserId);
  const isCurrentUserAdmin = currentUserMember?.isAdmin || false;
  const isCreator = groupInfo?.createdById === currentUserId;

  useEffect(() => {
    if (open && groupId) {
      fetchGroupInfo();
    }
  }, [open, groupId]);

  const fetchGroupInfo = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await groupService.getGroupInfo(groupId);
      setGroupInfo(data);
    } catch (err: any) {
      setError(err.message || "Failed to load group information");
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (date: Date) => {
    return new Date(date).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const handleLeaveGroup = async () => {
    if (!groupInfo) return;
    
    setIsLeaving(true);
    try {
      await groupService.leaveGroup(groupId);
      toast.success("You have left the group");
      onOpenChange(false);
      setActiveChat(null); // Close the active chat
    } catch (err: any) {
      toast.error(err.message || "Failed to leave group");
    } finally {
      setIsLeaving(false);
    }
  };

  const handleRemoveMember = async (userId: string, userName: string) => {
    if (!groupInfo) return;
    
    setRemovingMemberId(userId);
    try {
      await groupService.removeMember(groupId, userId);
      toast.success(`${userName} has been removed from the group`);
      // Refresh group info
      await fetchGroupInfo();
    } catch (err: any) {
      toast.error(err.message || "Failed to remove member");
    } finally {
      setRemovingMemberId(null);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Group Information</DialogTitle>
        </DialogHeader>

        {loading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-8 w-8 animate-spin text-gray-400" />
          </div>
        ) : error ? (
          <div className="text-center py-8 text-red-500">{error}</div>
        ) : groupInfo ? (
          <div className="space-y-6">
            {/* Group Details */}
            <div className="space-y-4">
              <div className="flex items-center space-x-4">
                <Avatar className="h-16 w-16">
                  <AvatarFallback className="text-2xl">
                    {groupInfo.name[0].toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <div className="flex-1">
                  <h3 className="text-xl font-semibold">{groupInfo.name}</h3>
                  {groupInfo.description && (
                    <p className="text-gray-600 mt-1">{groupInfo.description}</p>
                  )}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4 pt-2">
                <div className="flex items-center space-x-2 text-sm text-gray-600">
                  <Users className="h-4 w-4" />
                  <span>{groupInfo.memberCount} members</span>
                </div>
                <div className="flex items-center space-x-2 text-sm text-gray-600">
                  <Calendar className="h-4 w-4" />
                  <span>Created {formatDate(groupInfo.createdAt)}</span>
                </div>
              </div>
            </div>

            {/* Members List */}
            <div>
              <h4 className="font-semibold mb-3 flex items-center">
                <Users className="h-4 w-4 mr-2" />
                Members ({groupInfo.members.length})
              </h4>
              <div className="space-y-2">
                {groupInfo.members.map((member) => (
                  <div
                    key={member.userId}
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
                  >
                    <div className="flex items-center space-x-3 flex-1">
                      <Avatar>
                        <AvatarFallback>
                          {member.userName[0].toUpperCase()}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex-1">
                        <div className="flex items-center space-x-2">
                          <p className="font-medium">{member.userName}</p>
                          {member.isAdmin && (
                            <Badge variant="secondary" className="text-xs">
                              <Shield className="h-3 w-3 mr-1" />
                              Admin
                            </Badge>
                          )}
                        </div>
                        <p className="text-sm text-gray-500">{member.email}</p>
                      </div>
                    </div>
                    <div className="flex items-center space-x-3">
                      <div className="text-xs text-gray-400">
                        Joined {formatDate(member.joinedAt)}
                      </div>
                      {isCurrentUserAdmin && 
                       member.userId !== currentUserId && 
                       member.userId !== groupInfo.createdById && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleRemoveMember(member.userId, member.userName)}
                          disabled={removingMemberId === member.userId}
                          className="text-red-600 hover:text-red-700 hover:bg-red-50"
                        >
                          {removingMemberId === member.userId ? (
                            <Loader2 className="h-4 w-4 animate-spin" />
                          ) : (
                            <Trash2 className="h-4 w-4" />
                          )}
                        </Button>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Leave Group Button */}
            {!isCreator && (
              <div className="pt-4 border-t">
                <Button
                  variant="destructive"
                  onClick={handleLeaveGroup}
                  disabled={isLeaving}
                  className="w-full"
                >
                  {isLeaving ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Leaving...
                    </>
                  ) : (
                    <>
                      <LogOut className="mr-2 h-4 w-4" />
                      Leave Group
                    </>
                  )}
                </Button>
              </div>
            )}
          </div>
        ) : null}
      </DialogContent>
    </Dialog>
  );
};
