// src/pages/ChatApp.tsx
import React from 'react';
import { ChatProvider, useChatContext } from '@/contexts/ChatContext';
import { ChatSidebar, ChatArea, EmptyChatState } from '@/components/chat';

const ChatAppContent: React.FC = () => {
  const {
    activeChat,
    imageInputRef,
    fileInputRef,
    handleFileSelect,
  } = useChatContext();
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
      
      <ChatSidebar />
      
      <div className="flex-1 flex flex-col h-full overflow-hidden min-w-0">
        {activeChat ? <ChatArea /> : <EmptyChatState />}
      </div>
    </div>
  );
};

const ChatApp: React.FC = () => {
  return (
    <ChatProvider>
      <ChatAppContent />
    </ChatProvider>
  );
};

export default ChatApp;
