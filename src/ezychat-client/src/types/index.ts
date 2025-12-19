// ==========================================
// src/types/index.ts
// ==========================================

export interface AppResponse<T> {
  data: T;
  errors: string[];
  isSuccess: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  previousPageNumber?: number;
  nextPageNumber?: number;
}


export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export * from './quickMessage.types';