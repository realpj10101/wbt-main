import { Component, EventEmitter, inject, Output, output } from '@angular/core';
import { Member } from '../../../models/member.model';
import { Observable, take } from 'rxjs';
import { Gallery, GalleryItem, GalleryModule, ImageItem } from "ng-gallery";
import { MemberService } from '../../../services/member.service';
import { FollowService } from '../../../services/follow.service';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';


@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.scss'
})
export class MemberDetailsComponent {
  private _memberService = inject(MemberService);
  private _followService = inject(FollowService);
  private _route = inject(ActivatedRoute);
  private gallery = inject(Gallery);
  private _snack = inject(MatSnackBar);

  _apiUrl = environment.apiUrl;

  member: Member | undefined;
  members$: Observable<Member[] | null> | undefined;

  images: GalleryItem[] = [];

  // unfillowUsre = output();
  @Output('unfollowUserName') unfollowUserNameOut = new EventEmitter<string>();

  getMember(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (userName)
      this._memberService.getByUserName(userName)
        .pipe(
          take(1)
        ).subscribe({
          next: (res: Member | undefined) => {
            if (res) {
              this.member = res;
              console.log(this.member);
              this.setGalleryImage();
            }
          }
        })
  }

  setGalleryImage(): void {
    if (this.member) {
      for (const photo of this.member.photos) {
        this.images.push(new ImageItem(
          {
            src: this._apiUrl + photo.url_enlarged,
            thumb: this._apiUrl + photo.url_165
          }
        ));
      }

      const gallerRef = this.gallery.ref();
      gallerRef.load(this.images);
    }
  }

  follow(): void {
    if (this.member)
      this._followService.create(this.member.userName).pipe(
        take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (this.member)
              this.member.isFollowing = true;

            console.log(this.member?.isFollowing);
            console.log(this.member);
            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        });
  }

  unfollow(): void {
    if (this.member)
      this._followService.delete(this.member.userName).pipe(
        take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (this.member) {
              this.member.isFollowing = false;
            }
            console.log(this.member?.isFollowing);
            console.log(this.member);

            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        });
  }
}
