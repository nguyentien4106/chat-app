import React from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { MessageSquare, Users } from "lucide-react";
import { useChatContext } from "@/contexts/ChatContext";
import { useSidebar } from "@/contexts/SidebarContext";
import { SidebarHeader } from "./SidebarHeader";
import { SearchDialog } from "./SearchDialog";
import { ConversationsList } from "./ConversationsList";
import { GroupActions } from "./GroupActions";
import { GroupsList } from "./GroupsList";
import { UserDto } from "@/types/chat.types";

export const ChatSidebar: React.FC = () => {
  const { isOpen } = useSidebar();
  const {
    searchResults,
    isSearching,
    handleSearchUsers,
    handleClearSearch,
    handleStartChat,
  } = useChatContext();
  
  // Local state for search dialog
  const [searchTerm, setSearchTerm] = React.useState("");
  const [showSearch, setShowSearch] = React.useState(false);

  const handleSearch = (value: string) => {
    setSearchTerm(value);
    if (value.length > 2) {
      handleSearchUsers(value);
    } else {
      handleClearSearch();
    }
  };

  const handleUserClick = (user: UserDto) => {
    handleStartChat(user);
    setShowSearch(false);
    setSearchTerm("");
    handleClearSearch();
  };

  if (!isOpen) return null;

  return (
    <div className="w-80 bg-white border-r flex flex-col h-full">
      <SidebarHeader />

      <SearchDialog
        open={showSearch}
        onOpenChange={setShowSearch}
        searchTerm={searchTerm}
        searchResults={searchResults}
        isSearching={isSearching}
        onSearchChange={handleSearch}
        onUserClick={handleUserClick}
      />

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

        <TabsContent value="chats" className="flex-1 overflow-hidden mt-2 mx-0">
          <ConversationsList
          />
        </TabsContent>

        <TabsContent
          value="groups"
          className="flex-1 overflow-hidden flex flex-col mt-2 mx-0 min-h-0"
        >
          <GroupActions
          />

          <GroupsList
          />
        </TabsContent>
      </Tabs>
    </div>
  );
};
