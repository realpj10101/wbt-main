import { Component, inject, OnInit } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { ApiResponse } from '../../models/helpers/apiResponse.model';
import { MemberService } from '../../services/member.service';
import { Member } from '../../models/member.model';
import { MemberParams } from '../../models/helpers/member-params.model';
import { PageEvent, MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { PaginatedResult } from '../../models/helpers/pagination-result.model';
import { Pagination } from '../../models/helpers/pagination.model';
import { CoachPanelCardComponent } from '../coach-panel-card/coach-panel-card.component';

@Component({
  selector: 'app-coach-panel',
  standalone: true,
  imports: [
    CoachPanelCardComponent,
    MatPaginator
  ],
  templateUrl: './coach-panel.component.html',
  styleUrl: './coach-panel.component.scss'
})
export class CoachPanelComponent implements OnInit {
  private _teamService = inject(TeamService);
  private _membreService = inject(MemberService);
  members: Member[] | undefined;
  memberParams: MemberParams | undefined;
  pageSizeOptions = [5, 10, 25];
  pageEvent: PageEvent | undefined;
  subscribed: Subscription | undefined;
  pagination: Pagination | undefined;

  ngOnInit(): void {
    this.memberParams = new MemberParams();

    this.getAll();
  }

  getAll(): void {
    if (this.memberParams)
      this.subscribed = this._membreService.getAll(this.memberParams).subscribe({
        next: (res: PaginatedResult<Member[]>) => {
          if (res.body && res.pagination) {
            this.members = res.body;
            this.pagination = res.pagination;
            console.log(this.members);
          }
        }
      })
  }

  handlePageEvent(e: PageEvent) {
    if (this.memberParams) {
      if (e.pageSize !== this.memberParams.pageSize)
        e.pageIndex = 0;

      this.pageEvent = e;
      this.memberParams.pageSize = e.pageSize;
      this.memberParams.pageNumber = e.pageIndex + 1;

      this.getAll();
    }
  }
}
