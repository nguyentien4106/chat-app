// src/pages/ChatApp.tsx
import React, { useState, useEffect, useRef } from "react";
import { useSignalR } from "@/hooks/useSignalR";
import { useChat } from "@/hooks/useChat";
import { useFileUpload } from "@/hooks/useFileUpload";
import { useUserSearch } from "@/hooks/useUserSearch";
import { SendMessageRequest, UserDto } from "@/types/chat.types";
import { useAuth } from "@/contexts/AuthContext";
import { ChatSidebar, ChatArea, EmptyChatState } from "@/components/chat";

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
  const currentUserId = user?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
  
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

  const {
    users: searchResults,
    isSearching,
    searchUsers,
    clearSearch,
  } = useUserSearch();

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

  const handleCreateGroup = async (name: string, description: string): Promise<void> => {
    if (!name.trim()) return;

    try {
      await createGroup(name, description);
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

  const handleJoinByInvite = async (code: string): Promise<void> => {
    if (!code.trim()) return;

    try {
      await joinByInvite(code);
      alert("Successfully joined group!");
    } catch (error) {
      alert("Invalid or expired invite code");
    }
  };

  const handleAddMember = async (groupId: string, userId: string): Promise<void> => {
    if (!userId.trim()) return;

    try {
      await addMemberToGroup(groupId, userId);
      alert("Member added successfully!");
    } catch (error) {
      alert("Failed to add member. Check permissions or user ID.");
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  const handleStartChat = async (user: UserDto) => {
    setActiveChat({ id: user.id, name: user.username, type: "user" });
    await loadMessages(user.id, "user");
  };

  return (
    <div className="flex h-full bg-gray-100 overflow-hidden">
      {/* Hidden file inputs */}
      <input
        ref={imageInputRef}
        type="file"
        accept="image/*"
        className="hidden"
        onChange={(e) => handleFileSelect(e, true)}
      />
      <input
        ref={fileInputRef}
        type="file"
        accept=".pdf,.doc,.docx,.xls,.xlsx,.txt,.zip,.rar"
        className="hidden"
        onChange={(e) => handleFileSelect(e, false)}
      />

      <ChatSidebar
        user={user}
        isConnected={isConnected}
        activeChat={activeChat}
        conversations={conversations}
        groups={groups}
        onChatSelect={handleChatSelect}
        onCreateGroup={handleCreateGroup}
        onJoinByInvite={handleJoinByInvite}
        onAddMember={handleAddMember}
        onGenerateInvite={handleGenerateInvite}
        searchResults={searchResults}
        isSearching={isSearching}
        onSearchUsers={searchUsers}
        onClearSearch={clearSearch}
        onStartChat={handleStartChat}
      />

      <div className="flex-1 flex flex-col h-full overflow-hidden min-w-0">
        {activeChat ? (
          <ChatArea
            activeChat={activeChat}
            messages={messages}
            currentUserId={currentUserId}
            messageInput={messageInput}
            isConnected={isConnected}
            isUploading={isUploading}
            selectedFile={selectedFile}
            previewUrl={previewUrl}
            messagesEndRef={messagesEndRef}
            onMessageChange={setMessageInput}
            onSendMessage={handleSendMessage}
            onKeyDown={handleKeyDown}
            onImageSelect={() => imageInputRef.current?.click()}
            onFileSelect={() => fileInputRef.current?.click()}
            onClearFile={clearSelection}
          />
        ) : (
          <EmptyChatState />
        )}
      </div>
    </div>
  );
};

export default ChatApp;
