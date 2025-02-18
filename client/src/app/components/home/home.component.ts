import { Component, Signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { environment } from '../../../environments/environment.development';
import { LoggedInPlayer } from '../../models/logged-in-player.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterLink, RouterOutlet,
    MatButtonModule, MatCardModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  apiUrl: string = environment.apiUrl;
  loggedInPlayerSig: Signal<LoggedInPlayer | null> | undefined;
}
