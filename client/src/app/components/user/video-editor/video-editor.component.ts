import { Component, inject, Input, OnInit } from '@angular/core';
import { Member } from '../../../models/member.model';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { environment } from '../../../../environments/environment.development';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../../services/account.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Video } from '../../../models/video.mode';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { VideoCardComponent } from "../video-card/video-card.component";

@Component({
  selector: 'app-video-editor',
  standalone: true,
  imports: [
    CommonModule,
    FileUploadModule, MatFormFieldModule, MatCardModule, MatIconModule, MatButtonModule,
    VideoCardComponent
],
  templateUrl: './video-editor.component.html',
  styleUrl: './video-editor.component.scss'
})
export class VideoEditorComponent implements OnInit {
  @Input('memberInput') memberIn: Member | undefined;
  loggedInUser: LoggedInUser | null | undefined;
  apiUrl = environment.apiUrl;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  private _accountService = inject(AccountService);
  private _snackbar = inject(MatSnackBar);

  constructor() {
    this.loggedInUser = this._accountService.loggedInUserSig();
  }

  ngOnInit(): void {
    this.initialUploader();
  }

  fileOverBase(event: boolean): void {
    this.hasBaseDropZoneOver = event;
  }

  initialUploader(): void {
    console.log(this.uploader);
    
    if (this.loggedInUser) {
      this.uploader = new FileUploader({
        url: this.apiUrl + 'api/user/upload-video',
        authToken: 'Bearer ' + this.loggedInUser.token,
        isHTML5: true,
        allowedFileType: ['video'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 100_000_000
      });

      this.uploader.onSuccessItem = (item, response) => {
        if (response) {
          const video: Video = JSON.parse(response);
          this.memberIn?.videos.push(video);
        }
      }
    }
  }
}
