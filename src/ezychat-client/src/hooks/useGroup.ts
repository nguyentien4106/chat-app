import { useState, useEffect, useCallback } from "react";
import { GroupInfo } from "@/types/chat.types";
import { groupService } from "@/services/groupService";
import { toast } from "sonner";
import { useSignalR } from "./useSignalR";

interface UseGroupOptions {
  groupId: string;
  isOpen: boolean;
  onClose: () => void;
  onGroupDeleted?: () => void;
  onGroupLeft?: () => void;
}

export const useGroup = ({ groupId, isOpen, onClose, onGroupDeleted, onGroupLeft }: UseGroupOptions) => {
  const [groupInfo, setGroupInfo] = useState<GroupInfo | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [removingMemberId, setRemovingMemberId] = useState<string | null>(null);
  const [isLeaving, setIsLeaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const { leaveGroup } = useSignalR();
  
  const fetchGroupInfo = useCallback(async () => {
    if (!groupId) return;
    
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
  }, [groupId]);

  useEffect(() => {
    if (isOpen && groupId) {
      fetchGroupInfo();
    }
  }, [isOpen, groupId, fetchGroupInfo]);

  const handleLeaveGroup = async () => {
    if (!groupInfo) return;
    
    setIsLeaving(true);
    try {
      await leaveGroup(groupId);  
      await groupService.leaveGroup(groupId);
      toast.success("You have left the group");
      onClose();
      onGroupLeft?.();
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

  const handleDeleteGroup = async () => {
    if (!groupInfo) return;
    
    setIsDeleting(true);
    try {
      await groupService.deleteGroup(groupId);
      toast.success("Group has been deleted");
      setShowDeleteConfirm(false);
      onClose();
      onGroupDeleted?.();
    } catch (err: any) {
      toast.error(err.message || "Failed to delete group");
    } finally {
      setIsDeleting(false);
    }
  };

  const openDeleteConfirm = () => setShowDeleteConfirm(true);
  const closeDeleteConfirm = () => setShowDeleteConfirm(false);

  return {
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
    fetchGroupInfo,
  };
};
