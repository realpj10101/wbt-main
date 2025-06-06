import { Component, inject } from '@angular/core';
import { CommentService } from '../../services/comment.service';
import { Member } from '../../models/member.model';
import { Pagination } from '../../models/helpers/pagination.model';

@Component({
    selector: 'app-messages',
    imports: [],
    templateUrl: './messages.component.html',
    styleUrl: './messages.component.scss'
})
export class MessagesComponent {
  private _commentService = inject(CommentService);
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  readonly commentings = 'Commentings';

}
