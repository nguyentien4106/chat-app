import React from "react";
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
import { Search, Loader2 } from "lucide-react";
import { User } from "@/types/chat.types";

interface SearchDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  searchTerm: string;
  searchResults: User[];
  isSearching: boolean;
  onSearchChange: (value: string) => void;
  onUserClick: (user: User) => void;
}

export const SearchDialog: React.FC<SearchDialogProps> = ({
  open,
  onOpenChange,
  searchTerm,
  searchResults,
  isSearching,
  onSearchChange,
  onUserClick,
}) => {
  return (
    <div className="p-4 border-b">
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogTrigger asChild>
          <Button variant="outline" className="w-full">
            <Search className="w-4 h-4 mr-2" />
            Start New Chat
          </Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Start New Conversation</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="relative">
              <Search className="absolute left-3 top-3 w-4 h-4 text-gray-400" />
              <Input
                placeholder="Search by username or email..."
                value={searchTerm}
                onChange={(e) => onSearchChange(e.target.value)}
                className="pl-10"
              />
            </div>

            <ScrollArea className="h-64">
              {isSearching ? (
                <div className="flex items-center justify-center p-4">
                  <Loader2 className="w-6 h-6 animate-spin" />
                </div>
              ) : searchResults.length === 0 ? (
                <div className="text-center text-gray-500 p-4">
                  {searchTerm.length > 2
                    ? "No users found"
                    : "Type to search users"}
                </div>
              ) : (
                <div className="space-y-2">
                  {searchResults.map((user) => (
                    <div
                      key={user.id}
                      onClick={() => onUserClick(user)}
                      className="flex items-center space-x-3 p-3 rounded-lg hover:bg-gray-100 cursor-pointer"
                    >
                      <Avatar>
                        <AvatarFallback>
                          {user.userName[0].toUpperCase()}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex-1">
                        <p className="font-medium">{user.firstName} {user.lastName}</p>
                        <p className="text-sm text-gray-500">{user.email}</p>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </ScrollArea>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
