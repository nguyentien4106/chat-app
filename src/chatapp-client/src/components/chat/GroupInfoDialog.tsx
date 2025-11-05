import React from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription,
} from "@/components/ui/dialog";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Loader2, Users, Calendar, Shield, LogOut, Trash2 } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { useChatContext } from "@/contexts/ChatContext";
import { JWT_CLAIMS } from "@/constants/jwtClaims";
import { useGroup } from "@/hooks/useGroup";

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
  const { user } = useAuth();
  const { setActiveChat } = useChatContext();

  const {
    groupInfo,
    loading,
    error,
    removingMemberId,
    isLeaving,
    isDeleting,
    showDeleteConfirm,
    handleLeaveGroup,
    handleRemoveMember,
    handleDeleteGroup,
    openDeleteConfirm,
    closeDeleteConfirm,
  } = useGroup({
    groupId,
    isOpen: open,
    onClose: () => onOpenChange(false),
    onGroupDeleted: () => setActiveChat(null),
    onGroupLeft: () => setActiveChat(null),
  });

  const currentUserId = user?.[JWT_CLAIMS.NAME_IDENTIFIER];
  const currentUserMember = groupInfo?.members.find(m => m.userId === currentUserId);
  const isCurrentUserAdmin = currentUserMember?.isAdmin || false;
  const isCreator = groupInfo?.createdById === currentUserId;

  const formatDate = (date: Date) => {
    return new Date(date).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Group Information</DialogTitle>
        </DialogHeader>

        {loading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
          </div>
        ) : error ? (
          <div className="text-center py-8 text-destructive">{error}</div>
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
                    <p className="text-muted-foreground mt-1">{groupInfo.description}</p>
                  )}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4 pt-2">
                <div className="flex items-center space-x-2 text-sm text-muted-foreground">
                  <Users className="h-4 w-4" />
                  <span>{groupInfo.memberCount} members</span>
                </div>
                <div className="flex items-center space-x-2 text-sm text-muted-foreground">
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
                    className="flex items-center justify-between p-3 bg-muted/50 rounded-lg hover:bg-muted transition-colors"
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
                        <p className="text-sm text-muted-foreground">{member.email}</p>
                      </div>
                    </div>
                    <div className="flex items-center space-x-3">
                      <div className="text-xs text-muted-foreground/80">
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

            {/* Action Buttons */}
            <div className="pt-4 border-t space-y-2">
              {/* Leave Group Button - for non-creators */}
              {!isCreator && (
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
              )}

              {/* Delete Group Button - only for creators */}
              {isCreator && (
                <Button
                  variant="destructive"
                  onClick={openDeleteConfirm}
                  disabled={isDeleting}
                  className="w-full"
                >
                  <Trash2 className="mr-2 h-4 w-4" />
                  Delete Group
                </Button>
              )}
            </div>
          </div>
        ) : null}
      </DialogContent>

      {/* Delete Confirmation Dialog */}
      <Dialog open={showDeleteConfirm} onOpenChange={closeDeleteConfirm}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Delete Group</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete "{groupInfo?.name}"? This action cannot be undone and all members will be removed from the group.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="gap-2 sm:gap-0">
            <Button
              variant="outline"
              onClick={closeDeleteConfirm}
              disabled={isDeleting}
            >
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleDeleteGroup}
              disabled={isDeleting}
            >
              {isDeleting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Deleting...
                </>
              ) : (
                <>
                  <Trash2 className="mr-2 h-4 w-4" />
                  Delete Group
                </>
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </Dialog>
  );
};
