import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MemberService } from '../../../services/member.service';
import { Observable, Subscription } from 'rxjs';
import { Member } from '../../../models/member.model';
import { Pagination } from '../../../models/helpers/pagination.model';
import { MemberParams } from '../../../models/helpers/member-params.model';
import { PageEvent, MatPaginatorModule } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { PaginatedResult } from '../../../models/helpers/pagination-result.model';
import { MemberCardComponent } from "../member-card/member-card.component";
import { AbstractControl, FormBuilder, FormControl } from '@angular/forms';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [
    CommonModule,
    MatPaginatorModule,
    MemberCardComponent
  ],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.scss'
})
export class MemberListComponent implements OnInit, OnDestroy {
  memberService = inject(MemberService);
  members$: Observable<Member[] | null> | undefined;
  subscribed: Subscription | undefined;
  pagination: Pagination | undefined;
  members: Member[] | undefined;
  memberParams: MemberParams | undefined;
  pageSizeOptions = [5, 10, 25];
  pageEvent: PageEvent | undefined;
  orderOptions: string[] = ['lastAcitve', 'created', 'age'];
  orderOptionsView: string[] = ['Last Active', 'Created', 'Age'];

  private _fB = inject(FormBuilder);

  filterFg = this._fB.group({
    orderByCtrl: []
  });

  ngOnInit(): void {
    this.memberParams = new MemberParams();

    this.getAll();
  }

  ngOnDestroy(): void {
    this.subscribed?.unsubscribe();
  }

  get OrderByCtrl(): AbstractControl {
    return this.filterFg.get('orderByCtrl') as FormControl;
  }

  getAll(): void {
    if (this.memberParams)
      this.subscribed = this.memberService.getAll(this.memberParams).subscribe({
        next: (response: PaginatedResult<Member[]>) => {
          if (response.body && response.pagination) {
            this.members = response.body;
            this.pagination = response.pagination;
          }
        }
      });
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

  updateMembeParams(): void {
    if (this.memberParams) {
      this.memberParams.orderBy = this.OrderByCtrl.value;
    }
  }
}
