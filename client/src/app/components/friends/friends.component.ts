import { Component, inject } from '@angular/core';
import { FollowService } from '../../services/follow.service';
import { Member } from '../../models/member.model';
import { Pagination } from '../../models/helpers/pagination.model';

@Component({
  selector: 'app-friends',
  standalone: true,
  imports: [],
  templateUrl: './friends.component.html',
  styleUrl: './friends.component.scss'
})
export class FriendsComponent {
  followService = inject(FollowService);
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  readonly followings = 'Followings';
  // followParams

}
