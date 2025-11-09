import React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Users, UserPlus } from "lucide-react";
import { Group } from "@/types/chat.types";

interface GroupItemProps {
  group: Group;
  isActive: boolean;
  onSelect: () => void;
  onAddMember: (groupId: string, userName: string) => void;
  onGenerateInvite: (groupId: string) => void;
}

export const GroupItem: React.FC<GroupItemProps> = React.memo(({
  group,
  isActive,
  onSelect,
  onAddMember,
  onGenerateInvite,
}) => {
  const [addUserName, setAddUserName] = React.useState("");

  const handleAddMemberClick = () => {
    onAddMember(group.id, addUserName);
    setAddUserName("");
  };

  return (
    <div
      onClick={onSelect}
      className={`p-4 border-b cursor-pointer hover:bg-muted/50 ${
        isActive ? "bg-accent" : ""
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
                  <Button onClick={handleAddMemberClick}>
                    Add
                  </Button>
                </div>
              </div>
              <div>
                <p className="text-sm font-medium mb-2">Invite Link</p>
                <Button
                  onClick={() => onGenerateInvite(group.id)}
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
  );
});

GroupItem.displayName = 'GroupItem';
