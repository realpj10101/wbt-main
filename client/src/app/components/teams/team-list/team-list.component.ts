import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { TeamService } from '../../../services/team.service';
import { ShowTeam } from '../../../models/show-team.model';
import { PaginationParams } from '../../../models/helpers/paginationParams.model';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { PaginatedResult } from '../../../models/helpers/pagination-result.model';
import { Pagination } from '../../../models/helpers/pagination.model';
import { TeamCardComponent } from "../team-card/team-card.component";

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [
    MatButtonModule, MatCardModule, MatPaginatorModule,
    TeamCardComponent
],
  templateUrl: './team-list.component.html',
  styleUrl: './team-list.component.scss'
})
export class TeamListComponent implements OnInit, OnDestroy {
  private _teamService = inject(TeamService);
  teams: ShowTeam[] | undefined;
  paginationParams: PaginationParams | undefined;
  pageSizeOptions = [5, 10, 15];
  pageEvent: PageEvent | undefined;
  subscribed: Subscription | undefined;
  pagination: Pagination | undefined;

  ngOnInit(): void {
    this.paginationParams = new PaginationParams()

    this.getAll()
  }

  ngOnDestroy(): void {
    this.subscribed?.unsubscribe();
  }

  getAll(): void {
    if (this.paginationParams) {
      this.subscribed = this._teamService.getAll(this.paginationParams).subscribe({
        next: (res: PaginatedResult<ShowTeam[]>) => {
          if (res.body && res.pagination) {
            this.teams = res.body;
            this.pagination = res.pagination;
          }
        }
      });
    }
  }

  handlePageEvent(e: PageEvent) {
    if (this.paginationParams) {
      if (e.pageSize !== this.paginationParams.pageSize)
        e.pageIndex = 0;

      this.pageEvent = e;
      this.paginationParams.pageSize = e.pageSize;
      this.paginationParams.pageNumber = e.pageIndex + 1;

      this.getAll();
    }
  }
}
