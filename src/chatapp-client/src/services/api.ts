// ==========================================
// src/services/api.ts
// ==========================================
import axios, { AxiosInstance, AxiosError } from 'axios';
import Cookies from 'js-cookie';
import { toast } from 'sonner';
import { AppResponse } from '../types/index';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080';

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
      timeout: 30000,
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        // Add auth token if exists
        const token = Cookies.get('chat_app_access_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        
        // Don't set Content-Type for FormData - let browser set it with boundary
        if (config.data instanceof FormData) {
          delete config.headers['Content-Type'];
        }
        
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        this.handleError(error);
        return Promise.reject(error);
      }
    );
  }

  private handleError(error: AxiosError) {
    if (error.response) {
      const status = error.response.status;
      const message = (error.response.data as any)?.message || error.message;

      switch (status) {
        case 400:
          toast.error('Bad Request', { description: message });
          break;
        case 401:
          toast.error('Unauthorized', { description: 'Please login again' });
          // Handle logout
          break;
        case 403:
          toast.error('Forbidden', { description: 'You don\'t have permission' });
          break;
        case 404:
          toast.error('Not Found', { description: message });
          break;
        case 415:
          toast.error('Unsupported Media Type', { 
            description: 'Invalid file format or content type' 
          });
          break;
        case 500:
          toast.error('Server Error', { description: 'Something went wrong' });
          break;
        default:
          toast.error('Error', { description: message });
      }
    } else if (error.request) {
      toast.error('Network Error', { 
        description: 'Cannot connect to server' 
      });
    } else {
      toast.error('Error', { description: error.message });
    }
  }

  // Generic methods
  async get<T>(url: string, params?: any): Promise<T> {
    const response = await this.client.get<AppResponse<T>>(url, { params });
    if(response.data.isSuccess === false){
      throw new Error(response.data.errors.join(', '));
    }
    return response.data.data;
  }

  async post<T>(url: string, data?: any, config?: any): Promise<T> {
    const response = await this.client.post<AppResponse<T>>(url, data, config);
    const resData = response.data;
    if(resData.isSuccess === false){
      throw new Error(resData.errors.join(', '));
    }

    return resData.data;
  }

  async put<T>(url: string, data?: any, config?: any): Promise<T> {
    const response = await this.client.put<AppResponse<T>>(url, data, config);
    if(response.data.isSuccess === false){
      throw new Error(response.data.errors.join(', '));
    }
    return response.data.data;
  }

  async delete<T>(url: string, data?: any): Promise<T> {
    const response = await this.client.delete<AppResponse<T>>(url, { data });
    if(response.data.isSuccess === false){
      throw new Error(response.data.errors.join(', '));
    }
    return response.data.data;
  }
}

export const apiService = new ApiService();