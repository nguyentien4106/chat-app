import { useState, useEffect, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import type { Message, SendMessageRequest } from '@/types/chat.types';
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
  onMemberAdded: (callback: (data: { groupId: string; userId: string }) => void) => void;
}

export const useSignalR = (): UseSignalRReturn => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState<boolean>(false);

  useEffect(() => {
    const token = Cookies.get("chat_app_access_token");
    
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, {
        accessTokenFactory: () => token || '',
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    newConnection.start()
      .then(() => {
        console.log('SignalR Connected');
        setIsConnected(true);
      })
      .catch(err => {
        console.error('SignalR Connection Error:', err);
        setIsConnected(false);
      });

    newConnection.onreconnecting(() => {
      console.log('Reconnecting...');
      setIsConnected(false);
    });

    newConnection.onreconnected(() => {
      console.log('Reconnected');
      setIsConnected(true);
    });

    newConnection.onclose(() => {
      console.log('Connection closed');
      setIsConnected(false);
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, []);

  const sendMessage = useCallback(async (message: SendMessageRequest): Promise<Message | undefined> => {
    if (connection && isConnected) {
      const result = await connection.invoke<AppResponse<Message>>('SendMessage', message);
      if (result.isSuccess) {
        return result.data;
      }

      return undefined;
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
      connection.on('ReceiveMessage', callback);
    }
  }, [connection]);

  const onMemberAdded = useCallback((callback: (data: { groupId: string; userId: string }) => void) => {
    if (connection) {
      connection.on('MemberAdded', callback);
    }
  }, [connection]);

  return {
    connection,
    isConnected,
    sendMessage,
    joinGroup,
    leaveGroup,
    onReceiveMessage,
    onMemberAdded
  };
};
