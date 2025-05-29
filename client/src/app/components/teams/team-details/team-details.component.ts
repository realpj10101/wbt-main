import { Component, effect, inject, OnDestroy, OnInit, signal, Signal } from '@angular/core';
import { TeamService } from '../../../services/team.service';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute } from '@angular/router';
import { Gallery } from 'ng-gallery';
import { MatSnackBar } from '@angular/material/snack-bar';
import { take } from 'rxjs';
import { Member } from '../../../models/member.model';
import { ShowTeam } from '../../../models/show-team.model';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { MemberCardComponent } from "../../members/member-card/member-card.component";
import { TeamMembersCardComponent } from "../../team-members-card/team-members-card.component";
import { TeamPhotoEditorComponent } from "../team-photo-editor/team-photo-editor.component";
import { CoachAccountService } from '../../../services/coach-account.service';

@Component({
  selector: 'app-team-details',
  standalone: true,
  imports: [MatTabsModule, MatExpansionModule, TeamMembersCardComponent, TeamPhotoEditorComponent],
  templateUrl: './team-details.component.html',
  styleUrl: './team-details.component.scss'
})
export class TeamDetailsComponent implements OnInit {
  private _teamService = inject(TeamService);
  apiUrl = environment.apiUrl;
  private _route = inject(ActivatedRoute);
  private _gallery = inject(Gallery);
  private _snack = inject(MatSnackBar);
  photoUrl = inject(CoachAccountService).profilePhotoUrl;
  team: ShowTeam | undefined;
  members: Member[] | undefined;
  currentTeamSig: Signal<ShowTeam | null> | undefined;
  isTeamLoaded = signal(false);
  teamSig = this._teamService.currentTeamSig;

  isTeamLoadedSignal(): boolean {
    return this.isTeamLoaded();
  }

  ngOnInit(): void {
    this.getTeam();
    this.getTeamMembers();
  }

  getTeam(): void {
    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    if (teamName)
      this._teamService.getByTeamName(teamName)
        .pipe(
          take(1))
        .subscribe({
          next: (res: ShowTeam | null) => {
            if (res) {
              this.team = res;
              console.log(this.team);
            }
          }
        });
  }

  getTeamMembers(): void {
    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    if (teamName)
      this._teamService.getTeamMembersAsync(teamName)
        .pipe(take(1))
        .subscribe({
          next: (res: Member[]) => {
            if (res) {
              console.log(res);
              this.members = res;
            }
          }
        })
  }
}
