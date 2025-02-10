import { Component } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { Member } from '../../../models/member.model';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss'
})
export class UserEditComponent {
  apiUrl = environment.apiUrl;
  member: Member | undefined;
}
