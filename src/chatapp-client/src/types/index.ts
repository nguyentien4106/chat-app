// ==========================================
// src/types/index.ts
// ==========================================
export interface Device {
  id: string;
  serialNumber: string;
  deviceName: string;
  model?: string;
  ipAddress?: string;
  deviceStatus: string;
  lastOnline?: string;
  isActive: boolean;
  location?: string;
  port?: number;
}

export interface User {
  id: string;
  pin: string;
  fullName: string;
  cardNumber?: string;
  email?: string;
  phoneNumber?: string;
  department?: string;
  isActive: boolean;
  privilege: 0 | 1 | 2 | 14;
  verifyMode: number;
  createdAt: string;
  updatedAt: string;
  deviceId: string;
  deviceName?: string;
}

export interface AttendanceLog {
  id: string;
  deviceId: number;
  deviceName: string;
  userId?: number;
  userName: string
  pin: string;
  verifyType?: number;
  attendanceState: number;
  attendanceTime: string;
  workCode?: string;
  createdAt: string;
}

export interface DeviceCommand {
  id: string;
  deviceId: number;
  command: string;
  priority: number;
  status: string;
  responseData?: string;
  errorMessage?: string;
  createdAt: string;
  sentAt?: string;
  completedAt?: string;
}

export interface DeviceInfo {
  deviceId: string;
  firmwareVersion?: string;
  enrolledUserCount: number;
  fingerprintCount: number;
  attendanceCount: number;
  deviceIp?: string;
  fingerprintVersion?: string;
  faceVersion?: string;
  faceTemplateCount?: string;
  devSupportData?: string;
}

export interface CreateDeviceRequest {
  serialNumber: string;
  deviceName: string;
  model?: string;
  ipAddress?: string;
  port?: number;
  location?: string;
  applicationUserId: string
}

export interface SendCommandRequest {
  commandType: string;
  command?: string;
  priority?: number;
}

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