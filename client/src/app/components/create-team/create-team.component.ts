import { Component, inject } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CreateTeam } from '../../models/create.team.model';
import { ShowTeam } from '../../models/show-team.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-create-team',
  standalone: true,
  imports: [
    FormsModule, ReactiveFormsModule
  ],
  templateUrl: './create-team.component.html',
  styleUrl: './create-team.component.scss'
})
export class CreateTeamComponent {
  private _teamService = inject(TeamService);
  private _snack = inject(MatSnackBar);
  private _fb = inject(FormBuilder);
  private _route = inject(ActivatedRoute);

  teamFg: FormGroup = this._fb.group({
    teamNameCtrl: ['', [Validators.maxLength(50)]],
    teamLevelCtrl: ['', [Validators.maxLength(20)]],
    achievementsCtrl: ['', [Validators.maxLength(50)]],
    gamesPlayedCtrl: '',
    gamesWonCtrl: '',
    gamesLostCtrl: ''
  });

  get TeamNameCtrl(): FormControl {
    return this.teamFg.get('teamNameCtrl') as FormControl;
  }

  get TeamLevelCtrl(): FormControl {
    return this.teamFg.get('teamLevelCtrl') as FormControl;
  }

  get AchievementsCtrl(): FormControl {
    return this.teamFg.get('achievementsCtrl') as FormControl;
  }

  get GamesPlayedCtrl(): FormControl {
    return this.teamFg.get('gamesPlayedCtrl') as FormControl;
  }

  get GamesWonCtrl(): FormControl {
    return this.teamFg.get('gamesWonCtrl') as FormControl;
  }

  get GamesLostCtrl(): FormControl {
    return this.teamFg.get('gamesLostCtrl') as FormControl;
  }

  create(): void {
    let userIn: CreateTeam = {
      teamName: this.TeamNameCtrl.value,
      teamLevel: this.TeamLevelCtrl.value,
      achievements: this.AchievementsCtrl.value,
      gamesPlayed: this.GamesPlayedCtrl.value,
      gamesWon: this.GamesWonCtrl.value,
      gamesLost: this.GamesLostCtrl.value
    }

    this._teamService.create(userIn).subscribe({
      next: (res: ShowTeam) => console.log(res),
    });
  }
}
