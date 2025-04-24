import { Component, inject, Inject, OnInit, PLATFORM_ID, WritableSignal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { NavbarComponent } from "./components/navbar/navbar.component";
import { FooterComponent } from "./components/footer/footer.component";
import { NgxSpinnerModule } from 'ngx-spinner';
import { AccountService } from './services/account.service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { ResponsiveService } from './services/responsive.service';
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink, HomeComponent,
    NavbarComponent,
    FooterComponent, NgxSpinnerModule,
    MatIconModule, CommonModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private registerPlayerService = inject(AccountService);
  private platformId = inject(PLATFORM_ID);
  private breakpointObserver = inject(BreakpointObserver);
  isMobileViewSignal: WritableSignal<boolean> = inject(ResponsiveService).isMobileViewSig;

  constructor() {
    this.setBreakpointObserver();
  }

  ngOnInit(): void {
    this.initUserOnPageRefresh();
    this.registerPlayerService.loadLoggedInUserFromStorage();
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

  private setBreakpointObserver(): void {
    this.breakpointObserver.observe('(min-width: 51rem)') // include iPad/tablet
    .pipe(
      takeUntilDestroyed()
    ).subscribe((bPS: BreakpointState) => {
        console.log("ok");
        this.isMobileViewSignal.set(!bPS.matches);
      });
  }
}
