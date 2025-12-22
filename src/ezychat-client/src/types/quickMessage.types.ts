export interface QuickMessage {
  id: string;
  content: string;
  key: string;
  userId: string;
  createdAt: Date;
}

export interface CreateQuickMessageDto {
  content: string;
  key: string;
}

export interface UpdateQuickMessageDto {
  id: string;
  content: string;
  key: string;
}
