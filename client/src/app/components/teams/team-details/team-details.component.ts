import { Component, inject, OnInit } from '@angular/core';
import { TeamService } from '../../../services/team.service';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute } from '@angular/router';
import { Gallery } from 'ng-gallery';
import { MatSnackBar } from '@angular/material/snack-bar';
import { take } from 'rxjs';

@Component({
  selector: 'app-team-details',
  standalone: true,
  imports: [],
  templateUrl: './team-details.component.html',
  styleUrl: './team-details.component.scss'
})
export class TeamDetailsComponent implements OnInit {
  private _teamService = inject(TeamService);
  apiUrl = environment.apiUrl;
  private _route = inject(ActivatedRoute);
  private _gallery = inject(Gallery);
  private _snack = inject(MatSnackBar);
  
  ngOnInit(): void {
      this.getTeam();
  }

  getTeam(): void {
  
    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    if (teamName)
      this._teamService.getByTeamName(teamName)
        .pipe(
          take(1))
          .subscribe();
  }
}
