import { Component, inject, OnInit } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { ApiResponse } from '../../models/helpers/apiResponse.model';
import { MemberService } from '../../services/member.service';

@Component({
  selector: 'app-coach-panel',
  standalone: true,
  imports: [],
  templateUrl: './coach-panel.component.html',
  styleUrl: './coach-panel.component.scss'
})
export class CoachPanelComponent implements OnInit {
  private _teamService = inject(TeamService);
  private _membreService = inject(MemberService);
  teamName: ApiResponse | undefined;

  ngOnInit(): void {
    this._teamService.getTeamName().subscribe({
      next: (res: ApiResponse) => {
        this.teamName = res, console.log(this.teamName);
      }
    });
  }
}
