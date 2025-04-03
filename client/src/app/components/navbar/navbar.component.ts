import { Component, inject, OnInit, Signal, WritableSignal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { environment } from '../../../environments/environment.development';
import { LoggedInPlayer } from '../../models/logged-in-player.model';
import { AccountService } from '../../services/account.service';
import { MatTabsModule } from '@angular/material/tabs';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { ResponsiveService } from '../../services/responsive.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavbarMobileComponent } from "./navbar-mobile/navbar-mobile.component";

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule, RouterModule, NgOptimizedImage,
    MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatDividerModule, MatListModule, MatTabsModule,
    NavbarMobileComponent
],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  apiUrl: string = environment.apiUrl;
  loggedInUserSig: Signal<LoggedInPlayer | null> | undefined;
  linksWithAdmin: string[] = ['members', 'friends', 'message', 'users'];
  links: string[] = ['members', 'friends', 'teams'];
  linksWithCoach: string[] = ['members', 'friends', 'teams', 'coach-panel'];
  isMobileViewSignal: WritableSignal<boolean> = inject(ResponsiveService).isMobileViewSig;
  private breakpointObserver = inject(BreakpointObserver);

  private registerPlayerService = inject(AccountService);

  constructor() {
    this.setBreakpointObserver();
  }

  ngOnInit(): void {
    this.loggedInUserSig = this.registerPlayerService.loggedInPlayerSig;
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


  logout(): void {
    this.registerPlayerService.logOut();
  }
}
