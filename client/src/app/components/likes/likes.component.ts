import { Component, inject, OnInit } from '@angular/core';
import { LikeService } from '../../services/like.service';
import { Member } from '../../models/member.model';
import { Pagination } from '../../models/helpers/pagination.model';
import { LikeParams } from '../../models/helpers/like-params.model';
import { PageEvent } from '@angular/material/paginator';
import { PaginatedResult } from '../../models/helpers/pagination-result.model';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { LikePredicate } from '../../enums/like-predicate-enum';

@Component({
  selector: 'app-likes',
  standalone: true,
  imports: [],
  templateUrl: './likes.component.html',
  styleUrl: './likes.component.scss'
})
export class LikesComponent implements OnInit {
  private _likeService = inject(LikeService);
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  readonly likings = 'Likings';
  likeParams = new LikeParams();

  pageSizeOption = [3, 9, 12];
  pageEvent: PageEvent | undefined;

  ngOnInit(): void {
      this.getAll();
  }

  getAll(): void {
    this.members = [];

    this._likeService.getAll(this.likeParams).subscribe({
      next: (res: PaginatedResult<Member[]>) => {
        if (res.body && res.pagination) {
          this.members = res.body;
          this.pagination = res.pagination;
        }
      },
      error: err => console.log(err)
    });
  }

  onTabChange(event: MatTabChangeEvent) {
    if (event.tab.textLabel == this.likings) {
      this.likeParams.predicate = LikePredicate.LIKINGS;

      this.getAll();
    }
    else {
      this.likeParams.predicate = LikePredicate.LIKERS;

      this.getAll();
    }
  }

  removeDislikedUserFromMembers(userName: string): void {
    const members = this.members?.filter(member => member.userName !== userName);
    this.members = members;
  }

  handlePageEvent(e: PageEvent): void {
    if (e.pageSize !== this.likeParams.pageSize)
      e.pageIndex = 0;

    this.pageEvent = e;
    this.likeParams.pageSize = e.pageSize;
    this.likeParams.pageNumber = e.pageIndex + 1;

    this.getAll();
  }


}
