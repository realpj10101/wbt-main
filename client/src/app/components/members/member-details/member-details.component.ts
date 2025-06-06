import { Component, EventEmitter, inject, OnInit, Output, output, Signal } from '@angular/core';
import { Member } from '../../../models/member.model';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { interval, Observable, take } from 'rxjs';
import { Gallery, GalleryItem, GalleryModule, ImageItem } from "ng-gallery";
import { MemberService } from '../../../services/member.service';
import { FollowService } from '../../../services/follow.service';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { MatIconModule } from '@angular/material/icon';
import { IntlModule } from "angular-ecmascript-intl";
import { LightboxModule } from "ng-gallery/lightbox";
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from "@angular/material/tabs";
import { ChangeDetectionStrategy, signal } from '@angular/core';
import { matExpansionAnimations, MatExpansionModule } from '@angular/material/expansion';
import { TeamService } from '../../../services/team.service';
import { AccountService } from '../../../services/account.service';

@Component({
    selector: 'app-member-details',
    imports: [
        CommonModule,
        MatIconModule, MatButtonModule, MatTabsModule, MatExpansionModule,
        GalleryModule, LightboxModule,
        IntlModule, RouterModule
    ],
    // changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './member-details.component.html',
    styleUrl: './member-details.component.scss'
})
export class MemberDetailsComponent implements OnInit {
  member: Member | undefined;
  members$: Observable<Member[] | null> | undefined;
  teamName: string | undefined;
  loggedInUserSig: Signal<LoggedInUser | null> | undefined;

  images: GalleryItem[] = [];

  @Output('unfollowUsernameOut') unfollowUserNameOut = new EventEmitter<string>();
  private _memberService = inject(MemberService);
  private _followService = inject(FollowService);
  _apiUrl = environment.apiUrl;
  private _route = inject(ActivatedRoute);
  private gallery = inject(Gallery);
  private _snack = inject(MatSnackBar);
  readonly panelOpenState = signal(false);
  private _teamService = inject(TeamService);
  private _accountService = inject(AccountService);

  details: string[] = ['Bio', 'Achievements']

  ngOnInit(): void {
    this.getMember();

    this.loggedInUserSig = this._accountService.loggedInUserSig;

    if (this.loggedInUserSig && this.loggedInUserSig()?.roles.includes('coach')) {
      this._teamService.getTeamName().subscribe({
        next: (team: ApiResponse) => { this.teamName = team.message, console.log(this.teamName) },
        error: (err) => console.log("Error fetching team name:", err)
      })
    }
  }

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
              this.setGalleryImages();
            }
          }
        });
  }

  setGalleryImages(): void {
    if (this.member) {
      for (const photo of this.member.photos) {
        this.images.push(new ImageItem(
          {
            src: this._apiUrl + '/' + photo.url_enlarged,
            thumb: this._apiUrl + '/' + photo.url_165
          }
        ));
      }

      const galleryRef = this.gallery.ref();
      galleryRef.load(this.images);
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

            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        })
  }

  unfollow(): void {
    if (this.member)
      this._followService.delete(this.member.userName).pipe(
        take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (this.member) {
              this.member.isFollowing = false
            }

            this._snack.open(res.message, 'close', {
              duration: 7000,
              horizontalPosition: 'center',
              verticalPosition: 'top'
            })
          }
        })
  }

  addMember(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (this.teamName && userName) {
      this._teamService.addMember(this.teamName, userName).subscribe({
        next: (res: ApiResponse) => {
          if (this.member)
            this.member.isInTeam = true;

          this._snack.open(res.message, 'close', {
            duration: 7000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          })
        }
      });
    }
  }

  removeMember(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (this.teamName && userName) {
      this._teamService.removeMember(this.teamName, userName.toLowerCase()).subscribe({
        next: (res: ApiResponse) => {
          if (this.member)
            this.member.isInTeam = false;

          this._snack.open(res.message, 'close', {
            duration: 7000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          })
        }
      });
    }
  }

  assignCaptain(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (userName)
      this._teamService.assignCaptain(userName).subscribe({
        next: (res: ApiResponse) => {
          if (this.member)
            this.member.isCaptain = true;

          this._snack.open(res.message, 'close', {
            duration: 7000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          })
        }
      });
  }


  removeCaptain(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (userName)
      this._teamService.removeCaptain(userName).subscribe({
        next: (res: ApiResponse) => {
          if (this.member)
            this.member.isCaptain = false;

          this._snack.open(res.message, 'close', {
            duration: 7000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          })
        }
      });
  }
}