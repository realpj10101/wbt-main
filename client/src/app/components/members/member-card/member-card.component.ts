import { Component, EventEmitter, inject, Input, output, Output } from '@angular/core';
import { Member } from '../../../models/member.model';
import { environment } from '../../../../environments/environment.development';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FollowService } from '../../../services/follow.service';
import { take } from 'rxjs';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';

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
  @Output('unfollowUsernameOut') unfollowUserNameOut = new EventEmitter<string>();
  apiUrl = environment.apiUrl;
  private _followService = inject(FollowService);
  private _snack = inject(MatSnackBar);

  follow(): void {
    if (this.memberIn)
      this._followService.create(this.memberIn?.userName).pipe(
        take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (this.memberIn)
              this.memberIn.isFollowing = true;

            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        });
  }

  unfollow(): void {
    if (this.memberIn)
      this._followService.delete(this.memberIn.userName).pipe(
        take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (this.memberIn) {
              this.memberIn.isFollowing = false;
              this.unfollowUserNameOut.emit(this.memberIn.userName);
            }

            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        });
  }
}
