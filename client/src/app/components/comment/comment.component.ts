import { Component, inject, Input, OnInit } from '@angular/core';
import { CommentService } from '../../services/comment.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Member } from '../../models/member.model';
import { environment } from '../../../environments/environment.development';
import { take } from 'rxjs';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentInput } from '../../models/comment.model';
import { ApiResponse } from '../../models/helpers/apiResponse.model';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../../services/member.service';
import { MatIconModule } from '@angular/material/icon';
import { IntlModule } from "angular-ecmascript-intl";
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from "@angular/material/tabs";
import { UserComment } from '../../models/user-comment.model';
import { matExpansionAnimations, MatExpansionModule } from '@angular/material/expansion';
import { CommonModule } from '@angular/common';
import { P } from '@angular/cdk/keycodes';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule, ReactiveFormsModule,
    MatIconModule, MatButtonModule, MatTabsModule,
    IntlModule, MatExpansionModule
  ],
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.scss'
})
export class CommentComponent implements OnInit {
  @Input('memberInput') memberInput: Member | undefined;
  private _commentService = inject(CommentService);
  private _snack = inject(MatSnackBar);
  private _fb = inject(FormBuilder);
  private _route = inject(ActivatedRoute);
  private _memberService = inject(MemberService);
  http = inject(HttpClient);
  apiUrl = environment.apiUrl;
  comments: UserComment[] | undefined;
  userName: string | null = this._route.snapshot.paramMap.get('userName');

  comFg: FormGroup = this._fb.group({
    contentCtrl: ''
  })

  get ContetCtrl(): FormControl {
    return this.comFg.get('contentCtrl') as FormControl;
  }

  ngOnInit(): void {
    this.getMember();
    this.getComments(); 
  }

  getMember(): void {
    const userName: string | null = this._route.snapshot.paramMap.get('userName');

    if (userName)
      this._memberService.getByUserName(userName)
        .pipe(
          take(1))
        .subscribe({
          next: (res: Member | undefined) => {
            if (res) {
              this.memberInput = res;
            }
          }
        })
  }

  add(): void {
    let commetnIn: CommentInput = {
      content: this.ContetCtrl.value
    }

    this._commentService.add(this.memberInput?.userName, commetnIn)
      .pipe(take(1))
      .subscribe({
        next: (res: ApiResponse) => {
          if(this.userName && this.memberInput?.userName) {

            const newComment: UserComment  = {
              commenterName: this.userName,
              commentedMemberName: this.memberInput?.userName,
              content: commetnIn.content,
              createdAt: new Date()
            }
            this.comments?.push(newComment);
          }
            
          this._snack.open(res.message, 'close', {
            duration: 7000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          })
        }
      });

    console.log('com compo')
  }

  getComments(): void {
      const userName: string | null = this._route.snapshot.paramMap.get('userName');
      // console.log('user comment', userName);

      this._commentService.gerAllUserComments(userName).pipe(take(1))
        .subscribe({
          next: (res: UserComment[]) =>
          (
            console.log(res),
            this.comments = res
          )
        });
  }
}
