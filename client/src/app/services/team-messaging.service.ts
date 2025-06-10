import { computed, inject, Injectable, signal, WritableSignal } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { SenderMessage } from '../models/sender-message.model';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { ChatMessageRes } from '../models/message-res.model';
import { UserStatus } from '../models/user-status.model';

@Injectable({
  providedIn: 'root'
})
export class TeamMessagingService {
  private _hubConection!: HubConnection;
  private _messagesSignal = signal<ChatMessageRes[]>([]);
  private _userStatusesSignal: WritableSignal<Map<string, UserStatus>> = signal(new Map());
  private _lastSeenMap = new Map<string, Date>();

  messages = this._messagesSignal.asReadonly();
  userStatus = this._userStatusesSignal.asReadonly();
  
  private _http = inject(HttpClient);

  private apiUrl = 'http://localhost:5000/api/teammessaging/message';

  startConnection(): void {
    this._hubConection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/chatHub', {
        withCredentials: true
      })
      .build();

    this._hubConection
      .start()
      .then(() => {
        console.log('✅ SignalR connected');

        this.getOnlineUsersFromServer();
      })
      .catch((err) => console.error('SignalR connection error: ', err));

    this._hubConection.on(
      'ReceiveMessage',
      (senderUserName: string, message: string, timeStamp: Date) => {
        const newMessage: ChatMessageRes = { senderUserName, message, timeStamp };
        this._messagesSignal.update((msgs) => [...msgs, newMessage]);
      }
    );

    this._hubConection.on('UserOnline', (userName: string) => {
      const lowerUserName = userName.toLowerCase();
      this._userStatusesSignal.update((statusMap) => {
        console.log('UserOnline', this._userStatusesSignal());
        console.log('ok');
        
        const newMap = new Map(statusMap) // Create a new map for immutability

        console.log('map before', newMap);

        // Set user as online, removing any previous last seen time
        newMap.set(lowerUserName, { userName: lowerUserName, isOnline: true });

        console.log('map after', newMap);
        
        return newMap;
      });
      console.log('User came online', lowerUserName, this._userStatusesSignal());
    })

    this._hubConection.on('UserOffline', (userName: string, lastSeen: string) => {
      const lowerUserName = userName.toLowerCase();
      this._userStatusesSignal.update((statusMap) => {
        console.log('UserOffline', this._userStatusesSignal());
        
        const newMap = new Map(statusMap);

        // Set user as offline and record their last seen time
        newMap.set(lowerUserName, { userName: lowerUserName, isOnline: false, lastSeen: new Date(lastSeen) });
        return newMap;
      })
      console.log('User went offline:', lowerUserName, new Date(lastSeen), this._userStatusesSignal());
    })
  }

  sendMessage(user: string, message: string, teamName: string): void {
    this._hubConection.invoke('SendMessage', user, message, teamName).catch(console.error);
  }

  loadMessage() {
    return this._http.get<ChatMessageRes[]>(this.apiUrl).pipe(
      tap((messages) => this._messagesSignal.set(messages))
    )
  }

  getUserStatusSignal(userName: string) {
    const lowerUserName = userName.toLowerCase();
    console.log(this._userStatusesSignal());
    return computed(() => this._userStatusesSignal().get(lowerUserName));
  }

  getLastSeen(userName: string): Date | undefined {
    const lowerUserName = userName.toLowerCase();
    const userStatus = this._userStatusesSignal().get(lowerUserName); // get status from the map

    console.log('is online?', userStatus?.isOnline);

    return userStatus?.isOnline === false ? userStatus.lastSeen : undefined;
  }

  isUserOnline(userName: string): boolean {
    const lowerUserName = userName.toLowerCase();
    const userStatus: UserStatus | undefined = this._userStatusesSignal().get(lowerUserName);// get status from the map

    console.log(userStatus);

    if (userStatus)
      return userStatus?.isOnline === true;

    return false;
  }

  private getOnlineUsersFromServer(): void {
    this._hubConection
    .invoke<UserStatus[]>('GetOnlineUsers') // Call the Hub method and expect a list of UserStatusDto
    .then((userStatusDtos) => {
      this._userStatusesSignal.update((statusMap) => {
        const newMap = new Map(); // Create a brand new map to reflect the current server state
        userStatusDtos.forEach((dto) => {
          const lowerUserName = dto.userName.toLowerCase();
          newMap.set(lowerUserName, {
            userName: lowerUserName,
            isOnline: dto.isOnline,
            // Convert string timestamp to Date object if available
            lastSeen: dto.lastSeen ? new Date(dto.lastSeen) : undefined
          });
        });
        return newMap;
      });
      console.log('Initial online/offline statuses loaded:', this._userStatusesSignal());
    })
    .catch((err) => console.error('Error loading initial user statuses:', err));
  }
}
