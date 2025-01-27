import { Component, inject, Input, Output } from '@angular/core';
import { Member } from '../../../models/member.model';
import { environment } from '../../../../environments/environment.development';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [
    CommonModule, RouterModule, NgOptimizedImage,
    MatCardModule, MatIconModule, MatButtonModule
  ],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.scss'
})
export class MemberCardComponent {
  @Input('memberInput') memberIn: Member | undefined;

  apiUrl = environment.apiUrl;
  private _snack = inject(MatSnackBar);
}
