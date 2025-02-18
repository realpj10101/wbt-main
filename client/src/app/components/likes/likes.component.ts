import { Component, inject } from '@angular/core';
import { LikeService } from '../../services/like.service';
import { Member } from '../../models/member.model';

@Component({
  selector: 'app-likes',
  standalone: true,
  imports: [],
  templateUrl: './likes.component.html',
  styleUrl: './likes.component.scss'
})
export class LikesComponent {
  private _likeService = inject(LikeService);
  members: Member[] | undefined;
  // paginatio
}
