import React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { LinkIcon } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";

export const GroupActions: React.FC = () => {
  const {
    handleCreateGroup,
    handleJoinByInvite,
  } = useChatContext()
  
  const [newGroupName, setNewGroupName] = React.useState("");
  const [newGroupDescription, setNewGroupDescription] = React.useState("");
  const [inviteCode, setInviteCode] = React.useState("");
  const [showCreateGroup, setShowCreateGroup] = React.useState(false);
  const [showJoinByCode, setShowJoinByCode] = React.useState(false);

  const handleCreateGroupClick = () => {
    handleCreateGroup(newGroupName, newGroupDescription);
    setNewGroupName("");
    setNewGroupDescription("");
    setShowCreateGroup(false);
  };

  const handleJoinByInviteClick = () => {
    handleJoinByInvite(inviteCode);
    setInviteCode("");
    setShowJoinByCode(false);
  };

  return (
    <div className="px-4 py-2 space-y-2 flex-shrink-0">
      <Dialog open={showCreateGroup} onOpenChange={setShowCreateGroup}>
        <DialogTrigger asChild>
          <Button className="w-full">Create Group</Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Create New Group</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <Input
              placeholder="Group name"
              value={newGroupName}
              onChange={(e) => setNewGroupName(e.target.value)}
            />
            <Input
              placeholder="Description (optional)"
              value={newGroupDescription}
              onChange={(e) => setNewGroupDescription(e.target.value)}
            />
            <Button onClick={handleCreateGroupClick} className="w-full">
              Create
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={showJoinByCode} onOpenChange={setShowJoinByCode}>
        <DialogTrigger asChild>
          <Button variant="outline" className="w-full">
            <LinkIcon className="w-4 h-4 mr-2" />
            Join by Code
          </Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Join Group by Invite Code</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <Input
              placeholder="Enter invite code"
              value={inviteCode}
              onChange={(e) => setInviteCode(e.target.value)}
            />
            <Button onClick={handleJoinByInviteClick} className="w-full">
              Join
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
