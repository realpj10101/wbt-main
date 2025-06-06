import { Component, Input } from '@angular/core';
import { UserWithRole } from '../../../models/user-with-role.model';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-user-details',
    imports: [
        MatCardModule
    ],
    templateUrl: './user-details.component.html',
    styleUrl: './user-details.component.scss'
})
export class UserDetailsComponent {
  @Input('userInput') userIn: UserWithRole | undefined;
}
