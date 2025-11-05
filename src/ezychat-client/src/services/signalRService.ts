import * as signalR from '@microsoft/signalr';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  public async startConnection(token: string): Promise<signalR.HubConnection> {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/chat', {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    try {
      await this.connection.start();
      console.log('SignalR Connected');
      return this.connection;
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
      throw err;
    }
  }

  public getConnection(): signalR.HubConnection | null {
    return this.connection;
  }

  public async stopConnection(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      console.log('SignalR Disconnected');
    }
  }

  public onReceiveMessage(callback: (message: any) => void): void {
    if (this.connection) {
      this.connection.on('ReceiveMessage', callback);
    }
  }

  public onMemberAdded(callback: (data: any) => void): void {
    if (this.connection) {
      this.connection.on('MemberAdded', callback);
    }
  }

  public onUserTyping(callback: (data: any) => void): void {
    if (this.connection) {
      this.connection.on('UserTyping', callback);
    }
  }

  public async sendMessage(data: {
    content?: string;
    receiverId?: string;
    groupId?: string;
    type: number;
    fileUrl?: string;
    fileName?: string;
    fileType?: string;
    fileSize?: number;
  }): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('SendMessage', data);
    }
  }

  public async joinGroup(groupId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('JoinGroup', groupId);
    }
  }

  public async leaveGroup(groupId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('LeaveGroup', groupId);
    }
  }

  public async notifyTyping(receiverId?: string, groupId?: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('UserTyping', receiverId, groupId);
    }
  }
}

export default new SignalRService();