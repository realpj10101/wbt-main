import { Component, inject, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import {RouterLink, RouterOutlet} from '@angular/router';
import {HomeComponent} from './components/home/home.component';
import { NavbarComponent } from "./components/navbar/navbar.component";
import { FooterComponent } from "./components/footer/footer.component";
import { NgxSpinnerModule } from 'ngx-spinner';
import { RegisterPlayerService } from './services/register-player.service';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink, HomeComponent,
    NavbarComponent,
    FooterComponent, NgxSpinnerModule
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private registerPlayerService = inject(RegisterPlayerService);
  private platformId = inject(PLATFORM_ID);

  ngOnInit(): void {
      this.initUserOnPageRefresh();
  }

  initUserOnPageRefresh(): void {
    if (isPlatformBrowser(this.platformId)) {
      const loggedInPlayerStr = localStorage.getItem('loggedInPlayer');
      
      if (loggedInPlayerStr) {
         // First, check if user's token is not expired.
        this.registerPlayerService.authorizeLoggedInPlayer();

        // Then, set the authorized logged-in user
        this.registerPlayerService.setCurrentPlayer(JSON.parse(loggedInPlayerStr))
      }
    }
  }
}
