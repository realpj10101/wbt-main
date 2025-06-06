import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Member } from '../../models/member.model';
import { environment } from '../../../environments/environment.development';
import { ApiResponse } from '../../models/helpers/apiResponse.model';
import { TeamService } from '../../services/team.service';
import { MemberService } from '../../services/member.service';

@Component({
    selector: 'app-coach-panel-card',
    imports: [
        CommonModule, RouterModule, NgOptimizedImage,
        MatCardModule, MatIconModule, MatButtonModule
    ],
    templateUrl: './coach-panel-card.component.html',
    styleUrl: './coach-panel-card.component.scss'
})
export class CoachPanelCardComponent {
  @Input('memberInput') memberIn: Member | undefined;
  apiUrl = environment.apiUrl;
  private _teamService = inject(TeamService);
}
