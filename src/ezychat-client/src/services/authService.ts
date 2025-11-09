
// ==========================================
// src/services/authService.ts
// ==========================================
//import { AppResponse } from '@/types';
import { apiService } from './api';
import type { AuthUser, ForgotPasswordResponse, LoginResponse } from '@/types/auth';

export const authService = {
  login: async (userName: string, password: string): Promise<LoginResponse> => {

    return await apiService.post<LoginResponse>('/api/auth/login', { userName, password });
  },

  register: async (email: string, password: string, firstName: string, lastName: string, phoneNumber: string): Promise<string> => {
    return await apiService.post<string>('/api/auth/register', {
      email,
      password,
      firstName,
      lastName,
      phoneNumber
    });
  },

  logout: async (): Promise<void> => {
    return apiService.post('/api/auth/logout');
  },

  getCurrentUser: async (): Promise<AuthUser> => {
    return apiService.get<AuthUser>('/api/auth/me/me');
  },

  refreshToken: async (refreshToken: string): Promise<{ accessToken: string; refreshToken: string }> => {
    return apiService.post<{ accessToken: string; refreshToken: string }>('/api/auth/refresh', { refreshToken });
  },
  forgotPassword: async (email: string): Promise<ForgotPasswordResponse> => {
    await new Promise(resolve => setTimeout(resolve, 1500));
    
    // Simulate checking if email exists
    if (email.includes('@')) {
      return {
        message: 'Password reset link has been sent to your email',
        success: true,
      };
    }
    
    throw new Error('Invalid email address');
  },

  resetPassword: async (token: string, password: string): Promise<ForgotPasswordResponse> => {
    // Mock reset password for demo
    await new Promise(resolve => setTimeout(resolve, 1500));
    
    if (token && password.length >= 8) {
      return {
        message: 'Password has been reset successfully',
        success: true,
      };
    }
    
    throw new Error('Invalid reset token or password');
  },

  verifyResetToken: async (token: string): Promise<boolean> => {
    // Mock token verification
    await new Promise(resolve => setTimeout(resolve, 500));
    return token.length > 10; // Simple mock validation
  },
};
