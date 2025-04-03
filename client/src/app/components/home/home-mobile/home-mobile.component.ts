import { Component, inject, Signal, WritableSignal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-home-mobile',
  standalone: true,
  imports: [
    RouterLink, RouterOutlet,
    MatButtonModule, MatCardModule,
  ],
  templateUrl: './home-mobile.component.html',
  styleUrl: './home-mobile.component.scss'
})
export class HomeMobileComponent {

}
