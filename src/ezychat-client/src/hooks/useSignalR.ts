import { useState, useEffect, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import type { Group, Message, SendMessageRequest } from '@/types/chat.types';
import Cookies from 'js-cookie';
import { AppResponse } from '@/types';
import { ACCESSTOKEN_KEY, REFRESHTOKEN_KEY } from '@/constants/auth';
import { authService } from '@/services/authService';

const SIGNALR_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5000/hubs/chat';

interface UseSignalRReturn {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
  sendMessage: (message: SendMessageRequest) => Promise<Message | undefined>;
  joinGroup: (groupId: string) => Promise<void>;
  leaveGroup: (groupId: string) => Promise<void>;
  onReceiveMessage: (callback: (message: Message) => void) => void;
  onMemberAdded: (callback: (data: { groupId: string; userId: string; group: Group; message: Message }) => void) => void;
  onMemberRemoved: (callback: (data: { groupId: string; userId: string, message: Message }) => void) => void;
  onMemberLeft: (callback: (data: { groupId: string; userId: string, message: Message}) => void) => void;
  onGroupDeleted: (callback: (data: { groupId: string; groupName: string }) => void) => void;
}

export const useSignalR = (): UseSignalRReturn => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState<boolean>(false);
  const isRefreshingToken = useRef(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  // Function to get fresh token, with refresh if needed
  const getFreshToken = async (): Promise<string | null> => {
    let token = Cookies.get(ACCESSTOKEN_KEY);
    
    if (!token) {
      console.warn('No access token found');
      return null;
    }

    // Check if token is expired or about to expire
    try {
      const tokenPayload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = tokenPayload.exp * 1000; // Convert to milliseconds
      const currentTime = Date.now();
      const timeUntilExpiry = expirationTime - currentTime;

      // If token expires in less than 1 minute, refresh it
      if (timeUntilExpiry < 60000) {
        console.log('Token expired or about to expire, refreshing...');
        
        if (isRefreshingToken.current) {
          // Wait for ongoing refresh
          await new Promise(resolve => setTimeout(resolve, 1000));
          return Cookies.get(ACCESSTOKEN_KEY) || null;
        }

        isRefreshingToken.current = true;
        try {
          const refreshToken = Cookies.get(REFRESHTOKEN_KEY);
          if (!refreshToken) {
            console.error('No refresh token available');
            return null;
          }

          const response = await authService.refreshToken(refreshToken);
          Cookies.set(ACCESSTOKEN_KEY, response.accessToken);
          Cookies.set(REFRESHTOKEN_KEY, response.refreshToken);
          console.log('Token refreshed successfully');
          return response.accessToken;
        } catch (error) {
          console.error('Failed to refresh token:', error);
          return null;
        } finally {
          isRefreshingToken.current = false;
        }
      }

      return token;
    } catch (error) {
      console.error('Error parsing token:', error);
      return token; // Return token anyway, let server validate
    }
  };

  useEffect(() => { 
    let isMounted = true;
    let reconnectTimeoutId: NodeJS.Timeout | null = null;

    const createConnection = () => {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(SIGNALR_HUB_URL, {
          accessTokenFactory: async () => {
            const token = await getFreshToken();
            return token || '';
          },
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: retryContext => {
            // Exponential backoff: 0s, 2s, 10s, 30s
            if (retryContext.elapsedMilliseconds < 60000) {
              return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
            }
            // Stop retrying after 1 minute
            return null;
          }
        })
        .configureLogging(signalR.LogLevel.Warning)
        .build();

      return newConnection;
    };

    const setupConnection = async () => {
      const token = await getFreshToken();
      
      if (!token) {
        console.warn('No valid access token found, skipping SignalR connection');
        return;
      }

      const newConnection = createConnection();
      connectionRef.current = newConnection;

      // Set up event handlers before starting
      newConnection.onreconnecting(async (error) => {
        if (isMounted) {
          console.log('SignalR: Reconnecting...', error?.message);
          setIsConnected(false);
          
          // Try to refresh token before reconnection
          await getFreshToken();
        }
      });

      newConnection.onreconnected(() => {
        if (isMounted) {
          console.log('SignalR: Reconnected successfully');
          setIsConnected(true);
        }
      });

      newConnection.onclose(async (error) => {
        if (isMounted) {
          if (error) {
            console.error('SignalR: Connection closed with error:', error);
            
            // Check if it's an authentication error
            if (error.message && (
              error.message.includes('401') || 
              error.message.includes('Unauthorized') ||
              error.message.includes('token')
            )) {
              console.log('SignalR: Authentication error detected, attempting to reconnect with fresh token...');
              
              // Try to refresh token and reconnect
              const freshToken = await getFreshToken();
              if (freshToken && isMounted) {
                // Recreate connection with fresh token
                reconnectTimeoutId = setTimeout(() => {
                  if (isMounted) {
                    setupConnection();
                  }
                }, 2000);
              }
            }
          } else {
            console.log('SignalR: Connection closed');
          }
          setIsConnected(false);
        }
      });

      setConnection(newConnection);

      // Start the connection
      try {
        await newConnection.start();
        if (isMounted) {
          console.log('SignalR: Connected successfully');
          setIsConnected(true);
        }
      } catch (err: any) {
        if (isMounted) {
          console.error('SignalR: Connection failed:', err);
          setIsConnected(false);
          
          // If authentication failed, try with fresh token
          if (err.message && (
            err.message.includes('401') || 
            err.message.includes('Unauthorized') ||
            err.message.includes('token')
          )) {
            console.log('SignalR: Authentication failed, will retry with fresh token');
            const freshToken = await getFreshToken();
            if (freshToken && isMounted) {
              reconnectTimeoutId = setTimeout(() => {
                if (isMounted) {
                  setupConnection();
                }
              }, 2000);
            }
          }
        }
      }
    };

    setupConnection();

    return () => {
      isMounted = false;
      
      if (reconnectTimeoutId) {
        clearTimeout(reconnectTimeoutId);
      }
      
      // Gracefully stop the connection
      const stopConnection = async () => {
        const conn = connectionRef.current;
        if (conn && conn.state !== signalR.HubConnectionState.Disconnected) {
          try {
            await conn.stop();
            console.log('SignalR: Connection stopped cleanly');
          } catch (err) {
            console.error('SignalR: Error stopping connection:', err);
          }
        }
      };

      stopConnection();
    };
  }, []);

  const sendMessage = useCallback(async (message: SendMessageRequest): Promise<Message | undefined> => {
    if (connection && isConnected) {
      const result = await connection.invoke<AppResponse<Message>>('SendMessage', message);
      if (result.isSuccess) {
        return result.data;
      }

      throw new Error(result.errors.join(', '));
    }
  }, [connection, isConnected]);

  const joinGroup = useCallback(async (groupId: string): Promise<void> => {
    if (connection && isConnected) {
      await connection.invoke('JoinGroup', groupId);
    }
  }, [connection, isConnected]);

  const leaveGroup = useCallback(async (groupId: string): Promise<void> => {
    if (connection && isConnected) {
      await connection.invoke('LeaveGroup', groupId);
    }
  }, [connection, isConnected]);

  const onReceiveMessage = useCallback((callback: (message: Message) => void) => {
    if (connection) {
      connection.off('ReceiveMessage');
      connection.on('ReceiveMessage', callback);
    }
  }, [connection]);

  const onMemberAdded = useCallback((callback: (data: { groupId: string; userId: string, group: Group, message: Message }) => void) => {
    if (connection) {
      connection.off('MemberAdded');
      connection.on('MemberAdded', callback);
    }
  }, [connection]);

  const onMemberRemoved = useCallback((callback: (data: { groupId: string; userId: string, message: Message}) => void) => {
    if (connection) {
      connection.off('MemberRemoved');
      connection.on('MemberRemoved', async (data: { groupId: string; userId: string, message: Message}) => {
        // Call the user's callback first
        callback(data);
        
        // Automatically leave the group on the server side
        if (connection && isConnected) {
          try {
            await connection.invoke('LeaveGroup', data.groupId);
            console.log(`Automatically left group ${data.groupId} after member removal`);
          } catch (err) {
            console.error('Failed to leave group on server after removal:', err);
          }
        }
      });
    }
  }, [connection, isConnected]);

  const onMemberLeft = useCallback((callback: (data: { groupId: string; userId: string, message: Message }) => void) => {
    if (connection) {
      connection.off('MemberLeft');
      connection.on('MemberLeft', async (data: { groupId: string; userId: string, message: Message }) => {
        // Call the user's callback first
        callback(data);
        
        // Automatically leave the group on the server side
        if (connection && isConnected) {
          try {
            await connection.invoke('LeaveGroup', data.groupId);
            console.log(`Automatically left group ${data.groupId} after leaving`);
          } catch (err) {
            console.error('Failed to leave group on server after leaving:', err);
          }
        }
      });
    }
  }, [connection, isConnected]);

  const onGroupDeleted = useCallback((callback: (data: { groupId: string; groupName: string }) => void) => {
    if (connection) {
      connection.off('GroupDeleted');
      connection.on('GroupDeleted', async (data: { groupId: string; groupName: string }) => {
        // Call the user's callback first
        callback(data);
        
        // Automatically leave the group on the server side when deleted
        if (connection && isConnected) {
          try {
            await connection.invoke('LeaveGroup', data.groupId);
            console.log(`Automatically left deleted group ${data.groupId}`);
          } catch (err) {
            console.error('Failed to leave deleted group on server:', err);
          }
        }
      });
    }
  }, [connection, isConnected]); 

  return {
    connection,
    isConnected,
    sendMessage,
    joinGroup,
    leaveGroup,
    onReceiveMessage,
    onMemberAdded,
    onMemberRemoved,
    onMemberLeft,
    onGroupDeleted,
  };
};
