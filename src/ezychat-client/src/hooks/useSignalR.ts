import { useState, useEffect, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import type { Group, Message, SendMessageRequest } from '@/types/chat.types';
import Cookies from 'js-cookie';
import { AppResponse } from '@/types';

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

  useEffect(() => {
    const token = Cookies.get("chat_app_access_token");
    
    if (!token) {
      console.warn('No access token found, skipping SignalR connection');
      return;
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, {
        accessTokenFactory: () => token || '',
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

    let isMounted = true;

    // Set up event handlers before starting
    newConnection.onreconnecting(() => {
      if (isMounted) {
        console.log('SignalR: Reconnecting...');
        setIsConnected(false);
      }
    });

    newConnection.onreconnected(() => {
      if (isMounted) {
        console.log('SignalR: Reconnected successfully');
        setIsConnected(true);
      }
    });

    newConnection.onclose((error) => {
      if (isMounted) {
        if (error) {
          console.error('SignalR: Connection closed with error:', error);
        } else {
          console.log('SignalR: Connection closed');
        }
        setIsConnected(false);
      }
    });

    setConnection(newConnection);

    // Start the connection
    const startConnection = async () => {
      try {
        await newConnection.start();
        if (isMounted) {
          console.log('SignalR: Connected successfully');
          setIsConnected(true);
        }
      } catch (err) {
        if (isMounted) {
          console.error('SignalR: Connection failed:', err);
          setIsConnected(false);
        }
      }
    };

    startConnection();

    return () => {
      isMounted = false;
      
      // Gracefully stop the connection
      const stopConnection = async () => {
        if (newConnection.state !== signalR.HubConnectionState.Disconnected) {
          try {
            await newConnection.stop();
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
