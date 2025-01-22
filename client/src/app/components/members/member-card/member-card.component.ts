import { Component, inject, Input, Output } from '@angular/core';
import { Member } from '../../../models/member.mode';
import { environment } from '../../../../environments/environment.development';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.scss'
})
export class MemberCardComponent {
  @Input('memberInput') memberIn: Member | undefined;
  
  apiUrl = environment.apiUrl;
  private _snack = inject(MatSnackBar);
}
