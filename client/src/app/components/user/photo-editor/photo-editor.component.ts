import { Component, inject, Input, OnInit } from '@angular/core';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { NgOptimizedImage, CommonModule } from '@angular/common';
import { Member } from '../../../models/member.model';
import { LoggedInPlayer } from '../../../models/logged-in-player.model';
import { AccountService } from '../../../services/account.service';
import { UserService } from '../../../services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Photo } from '../../../models/photo.model';
import { take } from 'rxjs';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [
    CommonModule,
    NgOptimizedImage, FileUploadModule,
    MatFormFieldModule, MatCardModule, MatIconModule, MatButtonModule
  ],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.scss'
})
export class PhotoEditorComponent implements OnInit {
  @Input('memberInput') member: Member | undefined;
  loggedInPlayer: LoggedInPlayer | null | undefined;
  errorGlob: string | undefined;
  // apiUrl: string | undefined;
  photoUrl: string | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  private _accountService = inject(AccountService);
  private _userService = inject(UserService);
  private _snackBar = inject(MatSnackBar);

  constructor() {
    this.loggedInPlayer = this._accountService.loggedInPlayerSig();
  }

  ngOnInit(): void {
      this.initialUploader();
  }

  fileOverBase(event: boolean): void {
    this.hasBaseDropZoneOver = event;
  }

  initialUploader(): void {
    if (this.loggedInPlayer) {
      this.uploader = new FileUploader({
        url:  'http://localhost:5000/api/playeruser/add-photo',
        authToken: 'Bearer' + this.loggedInPlayer.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 4_000_000, // bytes // 4Mb
      });

      this.uploader.onAfterAddingAll = (file) => {
        file.withCredentials = false;
      }

      this.uploader.onSuccessItem = (item, response, status, header) => {
        if (response) {
          const photo: Photo = JSON.parse(response);
          this.member?.photos.push(photo);

          // set navbar profile photo when first photo is uploaded
          if (this.member?.photos.length === 1)
            this.setNavbarProfilePhoto(photo.url_165);
        }
      }
    }
  }


  // set navbar photo only when first photo is uploaded
  setNavbarProfilePhoto(url_165: string): void {
    if (this.loggedInPlayer) {

      this.loggedInPlayer.profilePhotoUrl = url_165;

      this._accountService.loggedInPlayerSig.set(this.loggedInPlayer);
    }
  }


  // Set main photo for card and album
  setMainPhotoComp(url_165In: string): void {

    this._userService.setMainPhoto(url_165In)
      .pipe(take(1))
      .subscribe({
        next: (res: ApiResponse) => {
          if (res && this.member) {

            for (const photo of this.member.photos) {
              // unset user previous main photo
              if (photo.isMain === true)
                photo.isMain = false;

              if (photo.url_165 === url_165In) {
                photo.isMain = true;

                this.loggedInPlayer!.profilePhotoUrl = url_165In;
                this._accountService.setCurrentPlayer(this.loggedInPlayer!);
              }
            }

            this._snackBar.open(res.message, 'close', {
              horizontalPosition: 'center',
              verticalPosition: 'bottom',
              duration: 7000
            });

            console.log(this.member.photos);
          }
        }
      });
  }

  deletePhotoComp(url_165In: string, index: number): void {
    this._userService.deletePhoto(url_165In)
      .pipe(take(1))
      .subscribe({
        next: (res: ApiResponse) => {
          if (res && this.member) {
            this.member.photos.splice(index, 1);

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
