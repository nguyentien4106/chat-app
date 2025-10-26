// src/components/ChatApp.tsx
import React, { useState, useEffect, useRef } from "react";
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
  Send,
  Image,
  Paperclip,
  X,
  Download,
  Loader2,
} from "lucide-react";
import { useSignalR } from "@/hooks/useSignalR";
import { useChat } from "@/hooks/useChat";
import { useFileUpload } from "@/hooks/useFileUpload";
// Add this to the top of ChatApp.tsx imports
import { useUserSearch } from "@/hooks/useUserSearch";
import { Search } from "lucide-react";
import { Message, SendMessageRequest, UserDto } from "@/types/chat.types";
import { useAuth } from "@/contexts/AuthContext";

// Types
enum MessageType {
  Text = 0,
  Image = 1,
  File = 2,
}

interface ActiveChat {
  id: string;
  name: string;
  type: "user" | "group";
}

const ChatApp: React.FC = () => {
  // Get authenticated user
  const { user } = useAuth();
  
  // Custom hooks
  const {
    connection,
    isConnected,
    sendMessage: sendSignalRMessage,
    joinGroup,
    onReceiveMessage,
    onMemberAdded,
  } = useSignalR();
  const {
    conversations,
    groups,
    messages,
    loadConversations,
    loadGroups,
    loadMessages,
    addMessage,
    createGroup,
    generateInviteLink,
    joinByInvite,
    addMemberToGroup,
  } = useChat();
  const {
    isUploading,
    selectedFile,
    previewUrl,
    selectFile,
    clearSelection,
    uploadFile,
  } = useFileUpload();

  // Local state
  const [activeChat, setActiveChat] = useState<ActiveChat | null>(null);
  const [messageInput, setMessageInput] = useState<string>("");
  const [newGroupName, setNewGroupName] = useState<string>("");
  const [newGroupDescription, setNewGroupDescription] = useState<string>("");
  const [inviteCode, setInviteCode] = useState<string>("");
  const [addUserId, setAddUserId] = useState<string>("");

  const {
    users: searchResults,
    isSearching,
    searchUsers,
    clearSearch,
  } = useUserSearch();
  const [searchTerm, setSearchTerm] = useState("");
  const [showSearch, setShowSearch] = useState(false);

  // Refs
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const imageInputRef = useRef<HTMLInputElement>(null);

  // Load initial data
  useEffect(() => {
    if (isConnected) {
      loadConversations();
      loadGroups();
    }
  }, [isConnected, loadConversations, loadGroups]);

  // SignalR message listener
  useEffect(() => {
    if (connection) {
      onReceiveMessage((message) => {
        console.log("Message received:", message);

        if (
          activeChat &&
          ((activeChat.type === "user" &&
            (message.senderId === activeChat.id ||
              message.receiverId === activeChat.id)) ||
            (activeChat.type === "group" && message.groupId === activeChat.id))
        ) {
          addMessage(message);
        }

        loadConversations();
      });

      onMemberAdded((data) => {
        if (activeChat?.type === "group" && activeChat.id === data.groupId) {
          loadGroups();
        }
      });
    }
  }, [
    connection,
    activeChat,
    onReceiveMessage,
    onMemberAdded,
    addMessage,
    loadConversations,
    loadGroups,
  ]);

  // Auto-scroll to bottom
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // Handlers
  const handleChatSelect = async (chat: ActiveChat) => {
    setActiveChat(chat);
    await loadMessages(chat.id, chat.type);

    if (chat.type === "group") {
      await joinGroup(chat.id);
    }
  };

  const handleFileSelect = (
    event: React.ChangeEvent<HTMLInputElement>,
    isImage: boolean
  ): void => {
    const file = event.target.files?.[0];
    if (!file) return;

    try {
      selectFile(file, isImage);
    } catch (error) {
      alert((error as Error).message);
    }
  };

  const handleSendMessage = async (): Promise<void> => {
    if ((!messageInput.trim() && !selectedFile) || !activeChat) return;

    try {
      let fileData = null;

      if (selectedFile) {
        fileData = await uploadFile(selectedFile);
      }

      const messageData: SendMessageRequest = {
        messageType: fileData ? fileData.messageType : MessageType.Text,
        content: messageInput.trim() || "",
        groupId: activeChat.type === "user" ? undefined : activeChat.id,
        receiverId: activeChat.type === "user" ? activeChat.id : undefined,
        fileUrl: fileData ? fileData.fileUrl : undefined,
        fileName: fileData ? fileData.fileName : undefined,
        fileSize  : fileData ? fileData.fileSize : undefined,
        fileType  : fileData ? fileData.fileType : undefined,
      };

      const newMessage = await sendSignalRMessage(messageData);
      addMessage(newMessage!);
      setMessageInput("");
      clearSelection();
    } catch (error) {
      console.error("Error sending message:", error);
      alert("Failed to send message");
    }
  };

  const handleCreateGroup = async (): Promise<void> => {
    if (!newGroupName.trim()) return;

    try {
      await createGroup(newGroupName, newGroupDescription);
      setNewGroupName("");
      setNewGroupDescription("");
    } catch (error) {
      alert("Failed to create group");
    }
  };

  const handleGenerateInvite = async (groupId: string): Promise<void> => {
    try {
      const code = await generateInviteLink(groupId);
      alert(
        `Invite code: ${code}\nShare this code with others to invite them.`
      );
    } catch (error) {
      alert("Failed to generate invite code");
    }
  };

  const handleJoinByInvite = async (): Promise<void> => {
    if (!inviteCode.trim()) return;

    try {
      await joinByInvite(inviteCode);
      setInviteCode("");
      alert("Successfully joined group!");
    } catch (error) {
      alert("Invalid or expired invite code");
    }
  };

  const handleAddMember = async (groupId: string): Promise<void> => {
    if (!addUserId.trim()) return;

    try {
      await addMemberToGroup(groupId, addUserId);
      setAddUserId("");
      alert("Member added successfully!");
    } catch (error) {
      alert("Failed to add member. Check permissions or user ID.");
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === "Enter" && !e.shiftKey && !selectedFile) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  const formatFileSize = (bytes?: number): string => {
    if (!bytes) return "0 B";
    const k = 1024;
    const sizes = ["B", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
  };

  const renderMessage = (msg: Message) => {
    const isOwn = msg.senderId === user?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    console.log(msg)
    return (
      <div
        key={msg.id}
        className={`flex ${isOwn ? "justify-end" : "justify-start"}`}
      >
        <div
          className={`max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${
            isOwn ? "bg-blue-500 text-white" : "bg-white text-gray-900 border border-gray-200"
          }`}
        >
          {activeChat?.type === "group" && !isOwn && msg.senderUsername && (
            <p className="text-xs font-semibold mb-1 opacity-70">
              {msg.senderUsername}
            </p>
          )}

          {msg.messageType === MessageType.Image && msg.fileUrl && (
            <div className="mb-2">
              <img
                src={msg.fileUrl}
                alt={msg.fileName || "Image"}
                className="rounded max-w-full h-auto cursor-pointer"
                onClick={() => window.open(msg.fileUrl, "_blank")}
              />
            </div>
          )}

          {msg.messageType === MessageType.File && msg.fileUrl && (
            <div className="flex items-center space-x-2 mb-2 p-2 bg-white bg-opacity-20 rounded">
              <Paperclip className="w-4 h-4" />
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium truncate">{msg.fileName}</p>
                <p className="text-xs opacity-70">
                  {formatFileSize(msg.fileSize)}
                </p>
              </div>
              <a
                href={msg.fileUrl}
                download={msg.fileName}
                target="_blank"
                rel="noopener noreferrer"
              >
                <Download className="w-4 h-4 cursor-pointer" />
              </a>
            </div>
          )}

          {msg.content && <p className="break-words">{msg.content}</p>}

          <p className="text-xs opacity-70 mt-1">
            {new Date(msg.createdAt).toLocaleTimeString()}
          </p>
        </div>
      </div>
    );
  };

  // Add search handler
  const handleSearch = (value: string) => {
    setSearchTerm(value);
    if (value.length > 2) {
      searchUsers(value);
    } else {
      clearSearch();
    }
  };

  const handleStartChat = async (user: UserDto) => {
    setActiveChat({ id: user.id, name: user.username, type: "user" });
    await loadMessages(user.id, "user");
    setShowSearch(false);
    setSearchTerm("");
    clearSearch();
  };

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <div className="w-80 bg-white border-r flex flex-col">
        <div className="p-4 border-b">
          <h1 className="text-xl font-bold">Chat App</h1>
          <p className="text-sm text-gray-500">{user?.userName || user?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || user?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || 'User'}</p>
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
                          onClick={() => handleStartChat(user)}
                          className="flex items-center space-x-3 p-3 rounded-lg hover:bg-gray-100 cursor-pointer"
                        >
                          <Avatar>
                            <AvatarFallback>
                              {user.username[0].toUpperCase()}
                            </AvatarFallback>
                          </Avatar>
                          <div className="flex-1">
                            <p className="font-medium">{user.username}</p>
                            <p className="text-sm text-gray-500">
                              {user.email}
                            </p>
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
        <Tabs defaultValue="chats" className="flex-1 flex flex-col">
          <TabsList className="mx-4 mt-2">
            <TabsTrigger value="chats" className="flex-1">
              <MessageSquare className="w-4 h-4 mr-2" />
              Chats
            </TabsTrigger>
            <TabsTrigger value="groups" className="flex-1">
              <Users className="w-4 h-4 mr-2" />
              Groups
            </TabsTrigger>
          </TabsList>

          <TabsContent value="chats" className="flex-1 overflow-hidden">
            <ScrollArea className="h-full">
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

          <TabsContent
            value="groups"
            className="flex-1 overflow-hidden flex flex-col"
          >
            <div className="p-4 space-y-2">
              <Dialog>
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
                    <Button onClick={handleCreateGroup} className="w-full">
                      Create
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>

              <Dialog>
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
                    <Button onClick={handleJoinByInvite} className="w-full">
                      Join
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>
            </div>

            <ScrollArea className="flex-1">
              {groups.length === 0 ? (
                <div className="p-4 text-center text-gray-500">
                  No groups yet
                </div>
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
                                  placeholder="User ID"
                                  value={addUserId}
                                  onChange={(e) => setAddUserId(e.target.value)}
                                />
                                <Button
                                  onClick={() => handleAddMember(group.id)}
                                >
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

      {/* Chat Area */}
      <div className="flex-1 flex flex-col">
        {activeChat ? (
          <>
            {/* Chat Header */}
            <div className="bg-white border-b p-4">
              <div className="flex items-center space-x-3">
                <Avatar>
                  <AvatarFallback>
                    {activeChat.name[0].toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <div>
                  <h2 className="font-semibold">{activeChat.name}</h2>
                  <p className="text-sm text-gray-500">
                    {activeChat.type === "group"
                      ? "Group Chat"
                      : "Direct Message"}
                  </p>
                </div>
              </div>
            </div>

            {/* Messages */}
            <ScrollArea className="flex-1 p-4">
              <div className="space-y-4">
                {messages.length === 0 ? (
                  <div className="text-center text-gray-500 mt-8">
                    No messages yet. Start the conversation!
                  </div>
                ) : (
                  messages.map(renderMessage)
                )}
                <div ref={messagesEndRef} />
              </div>
            </ScrollArea>

            {/* File Preview */}
            {selectedFile && (
              <div className="bg-white border-t p-4">
                <div className="flex items-center space-x-3 bg-gray-100 p-3 rounded-lg">
                  {previewUrl ? (
                    <img
                      src={previewUrl}
                      alt="Preview"
                      className="w-16 h-16 object-cover rounded"
                    />
                  ) : (
                    <div className="w-16 h-16 bg-gray-200 rounded flex items-center justify-center">
                      <Paperclip className="w-8 h-8 text-gray-400" />
                    </div>
                  )}
                  <div className="flex-1 min-w-0">
                    <p className="font-medium truncate">{selectedFile.name}</p>
                    <p className="text-sm text-gray-500">
                      {formatFileSize(selectedFile.size)}
                    </p>
                  </div>
                  <Button variant="ghost" size="sm" onClick={clearSelection}>
                    <X className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            )}

            {/* Input */}
            <div className="bg-white border-t p-4">
              <div className="flex space-x-2">
                <input
                  ref={imageInputRef}
                  type="file"
                  accept="image/*"
                  className="hidden"
                  onChange={(e) => handleFileSelect(e, true)}
                />
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => imageInputRef.current?.click()}
                  disabled={!isConnected || isUploading}
                >
                  <Image className="w-4 h-4" />
                </Button>

                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".pdf,.doc,.docx,.xls,.xlsx,.txt,.zip,.rar"
                  className="hidden"
                  onChange={(e) => handleFileSelect(e, false)}
                />
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={!isConnected || isUploading}
                >
                  <Paperclip className="w-4 h-4" />
                </Button>

                <Input
                  value={messageInput}
                  onChange={(e) => setMessageInput(e.target.value)}
                  onKeyPress={handleKeyPress}
                  placeholder="Type a message..."
                  className="flex-1"
                  disabled={!isConnected || isUploading}
                />
                <Button
                  onClick={handleSendMessage}
                  disabled={
                    !isConnected ||
                    isUploading ||
                    (!messageInput.trim() && !selectedFile)
                  }
                >
                  {isUploading ? (
                    <Loader2 className="w-4 h-4 animate-spin" />
                  ) : (
                    <Send className="w-4 h-4" />
                  )}
                </Button>
              </div>
            </div>
          </>
        ) : (
          <div className="flex-1 flex items-center justify-center text-gray-500">
            <div className="text-center">
              <MessageSquare className="w-16 h-16 mx-auto mb-4 opacity-20" />
              <p className="text-lg">Select a conversation to start chatting</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ChatApp;
