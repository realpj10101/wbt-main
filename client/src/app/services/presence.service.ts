import { Injectable, Signal, signal } from '@angular/core';
import { OnlineUser } from '../models/online-user.model';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private _hubConnection!: signalR.HubConnection;

  private onlineUsersSig = signal<OnlineUser[]>([]);
  public onlineUsers: Signal<OnlineUser[]> = this.onlineUsersSig;

  async startConnection(): Promise<void> {
    console.log('ok');
    
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/presence')
      .withAutomaticReconnect()
      .build();

    this.registerHubEvents();    
  }

  stopConnection(): void {
    if (this._hubConnection) {
      this._hubConnection.stop();
    }
  }

  private registerHubEvents(): void {
    this._hubConnection.on('GetOnlineUsers', (users: OnlineUser[]) => {      
      this.onlineUsersSig.set(users);

      console.log(this.onlineUsersSig);
    });

    this._hubConnection.onclose(error => {
      console.warn('Presence hub connection cleosed:', error);
    });
  }
}
