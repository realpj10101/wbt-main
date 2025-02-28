import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserWithRole } from '../../../models/user-with-role.model';
import { AdminService } from '../../../services/admin.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit, OnDestroy {
  users: UserWithRole[] | undefined;
  adminService = inject(AdminService);
  subsibed: Subscription | undefined;

  ngOnInit(): void { 
      
  }

  ngOnDestroy(): void {
      
  }

  getAll(): void {
    
  }
}
