// ==========================================
// src/types/auth.ts

import { JWT_CLAIMS } from "@/constants/jwtClaims";

// ==========================================
export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: AuthUser;
  accessToken: string
  refreshToken: string
}

export interface AuthUser {
  [JWT_CLAIMS.NAME_IDENTIFIER]: string;
  [JWT_CLAIMS.EMAIL]: string;
  [JWT_CLAIMS.NAME]: string;
  [JWT_CLAIMS.ROLE]: string;
  userName?: string;
  roles: string[];
  iss: string;
  exp: number;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  password: string;
  confirmPassword: string;
}

export interface ForgotPasswordResponse {
  message: string;
  success: boolean;
}

export interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  applicationUserId: string | null;
}