import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FollowService } from '../../services/follow.service';
import { Member } from '../../models/member.model';
import { Pagination } from '../../models/helpers/pagination.model';
import { FollowParams } from '../../models/helpers/follow-params.model';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PaginatedResult } from '../../models/helpers/pagination-result.model';
import { MatTabChangeEvent, MatTabsModule } from '@angular/material/tabs';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { FollowPredicate } from '../../enums/follow-predicate-enum';

@Component({
  selector: 'app-friends',
  standalone: true,
  imports: [
    CommonModule,
    MemberCardComponent,
    MatTabsModule, MatPaginatorModule
  ],
  templateUrl: './friends.component.html',
  styleUrl: './friends.component.scss'
})
export class FriendsComponent implements OnInit {
  followService = inject(FollowService);
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  readonly followings = 'Followings';
  followParams = new FollowParams();

  pageSizeOption = [3, 9, 12];
  pageEvent: PageEvent | undefined;

  ngOnInit(): void {

  }

  getAll(): void {
    this.members = [];

    this.followService.getAll(this.followParams).subscribe({
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
    if (event.tab.textLabel === this.followings) {
      this.followParams.predicate = FollowPredicate.FOLLOWINGS;

      this.getAll();
    }
    else {
      this.followParams.predicate = FollowPredicate.FOLLOWERS;

      this.getAll();
    }
  }

  removeUnfollowedFromMembers(userName: string): void {
    const members = this.members?.filter(member => member.userName !== userName);
    this.members = members;
  }

  handlePageEvent(e: PageEvent) {
    if (e.pageSize !== this.followParams.pageSize)
      e.pageIndex = 0;

    this.pageEvent = e;
    this.followParams.pageSize = e.pageSize;
    this.followParams.pageNumber = e.pageIndex + 1;

    this.getAll();
  }
}
