import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import {RouterLink, RouterOutlet} from '@angular/router';
import { environment } from '../../../environments/environment.development';
import { LoggedInPlayer } from '../../models/logged-in-player.model';
import { single } from 'rxjs';
import { RegisterPlayerService } from '../../services/register-player.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    MatButtonModule, MatToolbarModule, MatIconModule,
    RouterLink, RouterOutlet
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit{
  apiUrl: string = environment.apiUrl
  loggedInUserSig: Signal<LoggedInPlayer | null> | undefined;
  linksWithAsmin: string[] = ['members', 'friends', 'message', 'admin'];
  links: string[] = ['members', 'friends', 'messages'];

  private registerPlayerService = inject(RegisterPlayerService);

  ngOnInit(): void {
      this.loggedInUserSig = this.registerPlayerService.loggedInPlayerSig;
  }

  logOut(): void {
    this.registerPlayerService.logOut();  
  }

}
