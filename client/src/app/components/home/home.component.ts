import { Component, inject, Signal, WritableSignal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { environment } from '../../../environments/environment.development';
import { LoggedInUser } from '../../models/logged-in-player.model';
import { ResponsiveService } from '../../services/responsive.service';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HomeMobileComponent } from "./home-mobile/home-mobile.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterLink, RouterOutlet,
    MatButtonModule, MatCardModule,
    HomeMobileComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  apiUrl: string = environment.apiUrl;
  loggedInPlayerSig: Signal<LoggedInUser | null> | undefined;
  isMobileViewSignal: WritableSignal<boolean> = inject(ResponsiveService).isMobileViewSig;
  private breakpointObserver = inject(BreakpointObserver);

  constructor() {
    this.setBreakpointObserver();
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
