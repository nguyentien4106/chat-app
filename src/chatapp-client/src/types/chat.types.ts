// src/types/chat.types.ts

export interface User {
  id: string;
  name: string;
  email?: string;
  username?: string;
}

export enum MessageType {
  Text = 0,
  Image = 1,
  File = 2,
  Notification = 3
}

export interface Message {
  id: string | number;
  senderId: string;
  senderUserName?: string;
  receiverId?: string;
  conversationId?: string;
  groupId?: string;
  content?: string;
  messageType: MessageType;
  fileUrl?: string;
  fileName?: string;
  fileType?: string;
  fileSize?: number;
  createdAt: Date;
  isRead: boolean;
  groupName: string
}

export interface Conversation {
  id: string | undefined;
  userId: string;
  userName: string;
  lastMessage: string;
  lastMessageAt?: Date;
  unreadCount: number;
}

export interface Group {
  id: string;
  name: string;
  description?: string;
  createdById?: string;
  createdAt?: Date;
  memberCount: number;
}

export interface GroupMember {
  userId: string;
  userName: string;
  email: string;
  joinedAt: Date;
  isAdmin: boolean;
}

export interface GroupInfo {
  id: string;
  name: string;
  description?: string;
  createdById: string;
  createdAt: Date;
  memberCount: number;
  members: GroupMember[];
}

export interface ActiveChat {
  id: string;
  name: string;
  type: 'user' | 'group';
  conversationId?: string; // For direct messages - null when starting a new chat
  receiverId?: string; // For direct messages - the other user's ID
  groupId?: string; // For group messages
}

export interface SendMessageRequest {
  content?: string;
  receiverId?: string;
  conversationId?: string;
  groupId?: string;
  messageType: MessageType;
  fileUrl?: string;
  fileName?: string;
  fileType?: string;
  fileSize?: number;
}

export interface FileUploadResponse {
  fileUrl: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  messageType: MessageType;
}

export interface CreateGroupRequest {
  name: string;
  description?: string;
}

export interface JoinGroupRequest {
  inviteCode: string;
}

export interface AddMemberRequest {
  userId: string;
}

export interface InviteLinkResponse {
  inviteCode: string;
}

export interface AuthRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  userName: string;
  email: string;
}

// src/types/chat.types.ts - Add to existing file
export interface UserDto {
  id: string;
  userName: string;
  email: string;
  isAdmin: boolean;
}