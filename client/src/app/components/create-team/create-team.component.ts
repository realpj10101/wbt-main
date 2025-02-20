import { Component, inject } from '@angular/core';
import { TeamService } from '../../services/team.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CreateTeam } from '../../models/create.team.model';
import { ShowTeam } from '../../models/show-team.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';

@Component({
  selector: 'app-create-team',
  standalone: true,
  imports: [
    FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatStepperModule
  ],
  templateUrl: './create-team.component.html',
  styleUrl: './create-team.component.scss'
})
export class CreateTeamComponent {
  private _teamService = inject(TeamService);
  private _snack = inject(MatSnackBar);
  private _fb = inject(FormBuilder);
  private _route = inject(ActivatedRoute);

  firstFg: FormGroup = this._fb.group({
    teamNameCtrl: ['', [Validators.required, Validators.maxLength(50)]],
    teamLevelCtrl: ['', [Validators.required, Validators.maxLength(20)]],
    achievementsCtrl: ['', [Validators.required, Validators.maxLength(50)]]
  });

  secFg: FormGroup = this._fb.group({
    gamesPlayedCtrl: ['', [Validators.required]],
    gamesWonCtrl: ['', [Validators.required]],
    gamesLostCtrl: ['', [Validators.required]]
  });

  get TeamNameCtrl(): FormControl {
    return this.firstFg.get("teamNameCtrl") as FormControl;
  }

  get TeamLevelCtrl(): FormControl {
    return this.firstFg.get('teamLevelCtrl') as FormControl;
  }

  get AchievementsCtrl(): FormControl {
    return this.firstFg.get('achievementsCtrl') as FormControl;
  }

  get GamesPlayedCtrl(): FormControl {
    return this.secFg.get('gamesPlayedCtrl') as FormControl;
  }

  get GamesWonCtrl(): FormControl {
    return this.secFg.get('gamesWonCtrl') as FormControl;
  }

  get GamesLostCtrl(): FormControl {
    return this.secFg.get('gamesLostCtrl') as FormControl;
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
