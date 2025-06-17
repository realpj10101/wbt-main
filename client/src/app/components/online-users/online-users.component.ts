import { Component, inject, OnInit } from '@angular/core';
import { OnlineUser } from '../../models/online-user.model';
import { PresenceService } from '../../services/presence.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-online-users',
  imports: [
    CommonModule
  ],
  templateUrl: './online-users.component.html',
  styleUrl: './online-users.component.scss'
})
export class OnlineUsersComponent implements OnInit {
  private _presenceService = inject(PresenceService);
  onlineUsers = this._presenceService.onlineUsers;

  async ngOnInit() {
    await this._presenceService.startConnection();
  }
}
