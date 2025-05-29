import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { Member } from '../../../models/member.model';
import { MemberService } from '../../../services/member.service';
import { UserService } from '../../../services/user.service';
import {
  AbstractControl, FormBuilder,
  FormControl, FormGroup,
  Validators, ReactiveFormsModule,
  FormsModule
} from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { isPlatformBrowser } from '@angular/common';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { take } from 'rxjs';
import { UserUpdate } from '../../../models/user-update.model';
import { ApiResponse } from '../../../models/helpers/apiResponse.model';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatRadioModule } from '@angular/material/radio';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [
    MatCardModule, MatIconModule, MatButtonModule,
    MatTabsModule, MatFormFieldModule, MatInputModule,
    MatDividerModule, MatRadioModule, ReactiveFormsModule, FormsModule,
    PhotoEditorComponent, MatSelectModule
  ],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss'
})
export class UserEditComponent {
  apiUrl = environment.apiUrl;
  member: Member | undefined;
  readonly maxTextAreaChars: number = 1000;
  readonly minInputCahrs: number = 1;
  readonly maxInpuChars: number = 50;
  private _memberService = inject(MemberService);
  private _userService = inject(UserService);
  private platFormId = inject(PLATFORM_ID);
  private fb = inject(FormBuilder);

  userEditFg: FormGroup = this.fb.group({
    nameCtrl: ['', [Validators.maxLength(this.maxInpuChars)]],
    lastNameCtrl: ['', [Validators.maxLength(this.maxInpuChars)]],
    heightCtrl: '',
    weightCtrl: '',
    genderCtrl: 'female',
    positionCtrl: '',
    exprienceLevelCtrl: ['', [Validators.maxLength(this.maxInpuChars)]],
    skillsCtrl: ['', [Validators.maxLength(this.maxInpuChars)]],
    gamesPlayedCtrl: 0,
    pointsPerGameCtrl: 0,
    reboundsPerGameCtrl: 0,
    assistsPerGameCtrl: 0,
    bioCtrl: ['', [Validators.maxLength(this.maxTextAreaChars)]],
    achievementsCtrl: ['', [Validators.maxLength(this.maxInpuChars)]],
    cityCtrl: ['', [Validators.minLength(this.minInputCahrs), Validators.maxLength(this.maxInpuChars)]],
    regionCtrl: ['', [Validators.minLength(this.minInputCahrs), Validators.maxLength(this.maxInpuChars)]],
    countryCtrl: ['', [Validators.minLength(this.minInputCahrs), Validators.maxLength(this.maxInpuChars)]]
  })

  private matSnackBar = inject(MatSnackBar);

  //#region Value getter Methods

  get NameCtrl(): AbstractControl {
    return this.userEditFg.get('nameCtrl') as FormControl;
  }

  get LastNameCtrl(): AbstractControl {
    return this.userEditFg.get('lastNameCtrl') as FormControl;
  }

  get HeightCtrl(): AbstractControl {
    return this.userEditFg.get('heightCtrl') as FormControl;
  }

  get WeightCtrl(): AbstractControl {
    return this.userEditFg.get('weightCtrl') as FormControl;
  }

  get GenderCtrl(): AbstractControl {
    return this.userEditFg.get('genderCtrl') as FormControl;
  }

  get PositionCtrl(): AbstractControl {
    return this.userEditFg.get('positionCtrl') as FormControl;
  }

  get ExprienceLevelCtrl(): AbstractControl {
    return this.userEditFg.get('exprienceLevelCtrl') as FormControl;
  }

  get SkillsCtrl(): AbstractControl {
    return this.userEditFg.get('skillsCtrl') as FormControl;
  }

  get GamesPlayedCtrl(): AbstractControl {
    return this.userEditFg.get('gamesPerPlayedCtrl') as FormControl;
  }

  get PointsPerGameCtrl(): AbstractControl {
    return this.userEditFg.get('pointsPerGameCtrl') as FormControl;
  }

  get ReboundsPerGameCtrl(): AbstractControl {
    return this.userEditFg.get('reboundsPerGameCtrl') as FormControl;
  }

  get AssistsPerGameCtrl(): AbstractControl {
    return this.userEditFg.get('assistsPerGameCtrl') as FormControl;
  }

  get BioCtrl(): AbstractControl {
    return this.userEditFg.get('bioCtrl') as FormControl;
  }

  get AchievementsCtrl(): AbstractControl {
    return this.userEditFg.get('achievementsCtrl') as FormControl;
  }

  get CityCtrl(): AbstractControl {
    return this.userEditFg.get('cityCtrl') as FormControl;
  }

  get RegionCtrl(): AbstractControl {
    return this.userEditFg.get('regionCtrl') as FormControl;
  }

  get CountryCtrl(): AbstractControl {
    return this.userEditFg.get('countryCtrl') as FormControl;
  }

  ngOnInit(): void {
    this.getMember();
  }

  // //#endregion

  getMember(): void {
    if (isPlatformBrowser(this.platFormId)) {
      const loggedInplayerStr: string | null = localStorage.getItem('loggedInUser');

      if (loggedInplayerStr) {
        const loggedInPlayer: LoggedInUser = JSON.parse(loggedInplayerStr);

        this._memberService.getByUserName(loggedInPlayer.userName)?.pipe(take(1)).subscribe(member => {
          if (member) {
            this.member = member;

            // this.initControllersValues(member);
          }
        });
      }
    }
  }

  initControllersValues(member: Member) {
    this.NameCtrl.setValue(member.name.toUpperCase());
    this.LastNameCtrl.setValue(member.lastName.toUpperCase());
    this.HeightCtrl.setValue(member.height);
    this.WeightCtrl.setValue(member.weight);
    this.GenderCtrl.setValue(member.gender.toUpperCase());
    this.PositionCtrl.setValue(member.position.toUpperCase());
    this.ExprienceLevelCtrl.setValue(member.experienceLevel.toUpperCase());
    this.SkillsCtrl.setValue(member.skills.toUpperCase());
    this.GamesPlayedCtrl.setValue(member.gamesPlayed);
    this.PointsPerGameCtrl.setValue(member.pointsPerGame);
    this.ReboundsPerGameCtrl.setValue(member.reboundsPerGame);
    this.AssistsPerGameCtrl.setValue(member.assistsPerGame);
    this.BioCtrl.setValue(member.bio.toUpperCase());
    this.AchievementsCtrl.setValue(member.achievements.toUpperCase());
    this.CityCtrl.setValue(member.city.toUpperCase());
    this.RegionCtrl.setValue(member.region.toUpperCase());
    this.CountryCtrl.setValue(member.country.toUpperCase());
  }

  updatePlayer(): void {
    console.log('ok');

    if (this.member) {
      let updateUser: UserUpdate = {
        name: this.NameCtrl.value,
        // lastName: this.LastNameCtrl.value,
        // height: this.HeightCtrl.value,
        // weight: this.WeightCtrl.value,
        // gender: this.GenderCtrl.value,
        // position: this.PositionCtrl.value,
        // exprienceLevel: this.ExprienceLevelCtrl.value,
        // skills: this.SkillsCtrl.value,
        // gamesPlayed: this.GamesPlayedCtrl.value,
        // pointsPerGame: this.PointsPerGameCtrl.value,
        // reboundsPerGame: this.ReboundsPerGameCtrl.value,
        // assistsPerGame: this.AssistsPerGameCtrl.value,
        // bio: this.BioCtrl.value,
        // achievements: this.AchievementsCtrl.value,
        // city: this.CityCtrl.value,
        // region: this.RegionCtrl.value,
        // country: this.CountryCtrl.value
      }

      this._userService.updateUser(updateUser)
        .pipe(take(1))
        .subscribe({
          next: (res: ApiResponse) => {
            if (res.message) {
              this.matSnackBar.open(res.message, 'close', {
                horizontalPosition: 'center',
                verticalPosition: 'bottom',
                duration: 7000
              })
            }
          }
        });
    }
  }
}

