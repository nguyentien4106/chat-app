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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  MessageSquare,
  Users,
  UserPlus,
  LinkIcon,
  Search,
  Loader2,
} from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";
import { JWT_CLAIMS } from "@/constants/jwtClaims";

export const ChatSidebar: React.FC = () => {
  const {
    user,
    isConnected,
    activeChat,
    conversations,
    groups,
    searchResults,
    isSearching,
    handleChatSelect,
    handleCreateGroup,
    handleJoinByInvite,
    handleAddMember,
    handleGenerateInvite,
    handleSearchUsers,
    handleClearSearch,
    handleStartChat,
  } = useChatContext();
  
  // Local state for dialogs
  const [newGroupName, setNewGroupName] = React.useState("");
  const [newGroupDescription, setNewGroupDescription] = React.useState("");
  const [inviteCode, setInviteCode] = React.useState("");
  const [addUserName, setAddUserName] = React.useState("");
  const [searchTerm, setSearchTerm] = React.useState("");
  const [showSearch, setShowSearch] = React.useState(false);
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

  const handleAddMemberClick = (groupId: string) => {
    handleAddMember(groupId, addUserName);
    setAddUserName("");
  };

  const handleSearch = (value: string) => {
    setSearchTerm(value);
    if (value.length > 2) {
      handleSearchUsers(value);
    } else {
      handleClearSearch();
    }
  };

  const handleUserClick = (user: any) => {
    handleStartChat(user);
    setShowSearch(false);
    setSearchTerm("");
    handleClearSearch();
  };

  return (
    <div className="w-80 bg-white border-r flex flex-col h-full">
      {/* Header */}
      <div className="p-4 border-b">
        <h1 className="text-xl font-bold">Chat App</h1>
        <p className="text-sm text-gray-500">
          {user?.userName ||
            user?.[JWT_CLAIMS.NAME] ||
            user?.[JWT_CLAIMS.EMAIL] ||
            "User"}
        </p>
        <div className="flex items-center mt-2">
          <div
            className={`w-2 h-2 rounded-full mr-2 ${
              isConnected ? "bg-green-500" : "bg-red-500"
            }`}
          />
          <span className="text-xs text-gray-500">
            {isConnected ? "Connected" : "Disconnected"}
          </span>
        </div>
      </div>

      {/* Search Dialog */}
      <div className="p-4 border-b">
        <Dialog open={showSearch} onOpenChange={setShowSearch}>
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
                  onChange={(e) => handleSearch(e.target.value)}
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
                        onClick={() => handleUserClick(user)}
                        className="flex items-center space-x-3 p-3 rounded-lg hover:bg-gray-100 cursor-pointer"
                      >
                        <Avatar>
                          <AvatarFallback>
                            {user.userName[0].toUpperCase()}
                          </AvatarFallback>
                        </Avatar>
                        <div className="flex-1">
                          <p className="font-medium">{user.userName}</p>
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

      {/* Tabs */}
      <Tabs defaultValue="chats" className="flex-1 flex flex-col overflow-hidden min-h-0">
        <TabsList className="mx-4 mt-2 flex-shrink-0">
          <TabsTrigger value="chats" className="flex-1">
            <MessageSquare className="w-4 h-4 mr-2" />
            Chats
          </TabsTrigger>
          <TabsTrigger value="groups" className="flex-1">
            <Users className="w-4 h-4 mr-2" />
            Groups
          </TabsTrigger>
        </TabsList>

        {/* Chats Tab */}
        <TabsContent value="chats" className="flex-1 overflow-hidden mt-2 mx-0">
          <ScrollArea className="h-full px-4">
            {conversations.length === 0 ? (
              <div className="p-4 text-center text-gray-500">
                No conversations yet
              </div>
            ) : (
              conversations.map((conv) => (
                <div
                  key={conv.userId}
                  onClick={() =>
                    handleChatSelect({
                      id: conv.userId,
                      name: conv.username,
                      type: "user",
                    })
                  }
                  className={`p-4 border-b cursor-pointer hover:bg-gray-50 ${
                    activeChat?.id === conv.userId ? "bg-blue-50" : ""
                  }`}
                >
                  <div className="flex items-center space-x-3">
                    <Avatar>
                      <AvatarFallback>
                        {conv.username[0].toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                      <p className="font-medium truncate">{conv.username}</p>
                      <p className="text-sm text-gray-500 truncate">
                        {conv.lastMessage}
                      </p>
                    </div>
                    {conv.unreadCount > 0 && (
                      <span className="bg-blue-500 text-white text-xs rounded-full px-2 py-1">
                        {conv.unreadCount}
                      </span>
                    )}
                  </div>
                </div>
              ))
            )}
          </ScrollArea>
        </TabsContent>

        {/* Groups Tab */}
        <TabsContent
          value="groups"
          className="flex-1 overflow-hidden flex flex-col mt-2 mx-0 min-h-0"
        >
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

          <ScrollArea className="flex-1 px-4 min-h-0">
            {groups.length === 0 ? (
              <div className="p-4 text-center text-gray-500">No groups yet</div>
            ) : (
              groups.map((group) => (
                <div
                  key={group.id}
                  onClick={() =>
                    handleChatSelect({
                      id: group.id,
                      name: group.name,
                      type: "group",
                    })
                  }
                  className={`p-4 border-b cursor-pointer hover:bg-gray-50 ${
                    activeChat?.id === group.id ? "bg-blue-50" : ""
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
                            <p className="text-sm font-medium mb-2">
                              Add Member
                            </p>
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
                            <p className="text-sm font-medium mb-2">
                              Invite Link
                            </p>
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
              ))
            )}
          </ScrollArea>
        </TabsContent>
      </Tabs>
    </div>
  );
};
