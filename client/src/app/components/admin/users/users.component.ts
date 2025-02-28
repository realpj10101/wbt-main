import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserWithRole } from '../../../models/user-with-role.model';
import { AdminService } from '../../../services/admin.service';
import { Subscription } from 'rxjs';
import { UserDetailsComponent } from "../user-details/user-details.component";

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [UserDetailsComponent],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit, OnDestroy {
  users: UserWithRole[] | undefined;
  adminService = inject(AdminService);
  subsibed: Subscription | undefined;

  ngOnInit(): void { 
    this.getAll();
  }

  ngOnDestroy(): void {
      
  }

  getAll(): void {
    this.subsibed = this.adminService.getUsersWithRoles().subscribe({
      next: (res: UserWithRole[]) => this.users = res
    });
  }
}
