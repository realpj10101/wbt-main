import { inject, Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { SenderMessage } from '../models/sender-message.model';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { ChatMessageRes } from '../models/message-res.model';

@Injectable({
  providedIn: 'root'
})
export class TeamMessagingService {
  private hubConection!: HubConnection;
  private messagesSignal = signal<ChatMessageRes[]>([]);
  messages = this.messagesSignal.asReadonly();
  private _http = inject(HttpClient);

  private apiUrl = 'http://localhost:5000/api/teammessaging/message';

  startConnection(): void {
    this.hubConection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/chatHub', {
        withCredentials: true
      })
      .build();

    this.hubConection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('SignalR connection error: ', err));

    this.hubConection.on(
      'ReceiveMessage',
      (senderUserName: string, message: string, timeStamp: Date) => {
        const newMessage: ChatMessageRes = { senderUserName, message, timeStamp };
        this.messagesSignal.update((msgs) => [...msgs, newMessage]);
      }
    );
  }

  sendMessage(user: string, message: string, teamName: string): void {
    this.hubConection.invoke('SendMessage', user, message, teamName).catch(console.error);
  }

  loadMessage() {
    return this._http.get<ChatMessageRes[]>(this.apiUrl).pipe(
      tap((messages) => this.messagesSignal.set(messages))
    )
  }
}
