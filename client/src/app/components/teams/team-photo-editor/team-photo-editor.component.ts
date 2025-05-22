import { Component, inject, Input, OnInit } from '@angular/core';
import { ShowTeam } from '../../../models/show-team.model';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { FileUploader } from 'ng2-file-upload';
import { CoachAccountService } from '../../../services/coach-account.service';
import { TeamService } from '../../../services/team.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute } from '@angular/router';
import { Photo } from '../../../models/photo.model';
import { take } from 'rxjs';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';

@Component({
  selector: 'app-team-photo-editor',
  standalone: true,
  imports: [],
  templateUrl: './team-photo-editor.component.html',
  styleUrl: './team-photo-editor.component.scss'
})
export class TeamPhotoEditorComponent implements OnInit {
  @Input('teamInput') team: ShowTeam | undefined;
  loggedInUser: LoggedInUser | null | undefined;
  apiUrl = environment.apiUrl;
  errorGlob: string | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  private _coachAccountService = inject(CoachAccountService);
  private _teamService = inject(TeamService);
  private _snackBar = inject(MatSnackBar);
  private _route = inject(ActivatedRoute);
  teamName: string | undefined | null;

  constructor() {
    this.loggedInUser = this._coachAccountService.loggedInUserSig();
  }

  ngOnInit(): void {
    this.initialUploader();
    this.teamName = this._route.snapshot.paramMap.get('teamName');

  }

  fileOverBase(event: boolean): void {
    this.hasBaseDropZoneOver = event;
  }

  initialUploader(): void {
    if (this.loggedInUser) {
      this.uploader = new FileUploader({
        url: this.apiUrl + 'api/team/add-photo/' + this.teamName,
        authToken: 'Bearer' + this.loggedInUser.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 4_000_000 // bytes // 4MB
      });

      this.uploader.onAfterAddingAll = (file) => {
        file.withCredentials = false;
      }

      this.uploader.onSuccessItem = (item, response, status, header) => {
        if (response) {
          const photo: Photo = JSON.parse(response);
          this.team?.photos.push(photo);

          console.log(photo);
        }
      }
    }
  }


  setMainPhotoComp(url_165In: string): void {
    if (this.teamName) {
      this._teamService.setMainPhoto(url_165In, this.teamName)
        .pipe(take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (res && this.team) {

              for (const photo of this.team.photos) {
                if (photo.isMain === true)
                  photo.isMain = false;

                if (photo.url_165 === url_165In) {
                  photo.isMain = true;

                  this.loggedInUser!.profilePhotoUrl = url_165In;
                  this._coachAccountService.setCurrentCoach(this.loggedInUser!);
                }
              }

              this._snackBar.open(res.message, 'close', {
                horizontalPosition: 'center',
                verticalPosition: 'bottom',
                duration: 7000
              });
            }
          }
        })
    }
  }

  deletePhotoComp(url_165In: string, index: number): void {
    if (this.teamName) {
      this._teamService.deletePhoto(url_165In, this.teamName)
        .pipe(take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (res && this.team) {
              this.team.photos.splice(index, 1)

              this._snackBar.open(res.message, 'close', {
                horizontalPosition: 'center',
                verticalPosition: 'bottom',
                duration: 7000
              });
            }
          }
        })
    }
  }
}
