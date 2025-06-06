import { Component, inject, OnInit, Signal, WritableSignal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { environment } from '../../../../environments/environment.development';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { AccountService } from '../../../services/account.service';
import { MatTabsModule } from '@angular/material/tabs';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';

@Component({
    selector: 'app-navbar-mobile',
    imports: [
        CommonModule, RouterModule, NgOptimizedImage,
        MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule,
        MatDividerModule, MatListModule, MatTabsModule,
    ],
    templateUrl: './navbar-mobile.component.html',
    styleUrl: './navbar-mobile.component.scss'
})
export class NavbarMobileComponent implements OnInit {
  apiUrl: string = environment.apiUrl;
  loggedInUserSig: Signal<LoggedInUser | null> | undefined;
  linksWithAdmin: string[] = ['members', 'friends', 'message', 'users'];
  links: string[] = ['members', 'friends', 'teams'];
  linksWithCoach: string[] = ['members', 'friends', 'teams', 'coach-panel'];
  private _accountService = inject(AccountService);

  ngOnInit(): void {
    this.loggedInUserSig = this._accountService.loggedInUserSig;
  }

  logout(): void {
    this._accountService.logout();
  }
}
