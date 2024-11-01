import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { RegisterPlayer } from './models/register-player.model';
import { RegisterPlayerService } from './services/register-player.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { Subscription } from 'rxjs';
import { AutoFocusDirective } from './directives/auto-focus.directive';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,  CommonModule, FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatSnackBarModule, MatRadioModule,
    MatDatepickerModule, MatNativeDateModule,
    AutoFocusDirective
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  //#region enjects and vars
  registerPlayerService = inject(RegisterPlayerService);
  fb = inject(FormBuilder);

  minDate = new Date();
  maxDate = new Date();

  passwordsNotMatch: boolean | undefined;
  subscibedRegisterPlayer: Subscription | undefined;
  emailExistError: string | undefined;
  //#endregion

  //#region auto-run-methods
  ngOnInit(): void {
    // set datePicker year limitations
    const currentYear = new Date().getFullYear();
    this.minDate = new Date(currentYear - 99, 0, 1); // not older than 99 years
    this.maxDate = new Date(currentYear - 18 - 0 - 1); // not earlier than 18 years
  }

  ngOnDestroy(): void {
    this.subscibedRegisterPlayer?.unsubscribe();
  }
  //#endregion

  //#region FormGroup
  registerFg = this.fb.group({
    genderCtrl: ['female', [Validators.required]],
    emailCtrl: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$/)]],
    userNameCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    passwordCtrl: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(20)]],
    confirmPasswordCtrl: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(20)]],
    nameCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    lastNameCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    nationalCodeCtrl: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10)]],
    heightCtrl: ['', [Validators.required]],
    ageCtrl: ['', [Validators.required]],
    knownAsCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    cityCtrl: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
    countryCtrl: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
  })

  get GenderCtrl(): FormControl {
    return this.registerFg.get('genderCtrl') as FormControl;
  }
  get EmailCtrl(): FormControl {
    return this.registerFg.get('emailCtrl') as FormControl;
  }
  get UserNameCtrl(): FormControl {
    return this.registerFg.get('userNameCtrl') as FormControl;
  }
  get PasswordCtrl(): FormControl {
    return this.registerFg.get('passwordCtrl') as FormControl;
  }
  get ConfirmPasswordCtrl(): FormControl {
    return this.registerFg.get('confirmPasswordCtrl') as FormControl;
  }
  get NameCtrl(): FormControl {
    return this.registerFg.get('nameCtrl') as FormControl;
  }
  get LastNameCtrl(): FormControl {
    return this.registerFg.get('lastNameCtrl') as FormControl;
  }
  get NationalCodeCtrl(): FormControl {
    return this.registerFg.get('nationalCodeCtrl') as FormControl;
  }
  get HeightCtrl(): FormControl {
    return this.registerFg.get('heightCtrl') as FormControl;
  }
  get AgeCtrl(): FormControl {
    return this.registerFg.get('ageCtrl') as FormControl;
  }
  get KnownAsCtrl(): FormControl {
    return this.registerFg.get('knownAsCtrl') as FormControl;
  }
  get CityCtrl(): FormControl {
    return this.registerFg.get('cityCtrl') as FormControl;
  }
  get CountryCtrl(): FormControl {
    return this.registerFg.get('countryCtrl') as FormControl;
  }
  //#endregion

  //#region methods
  /**
   * Create RegisterPlayer Object
   * call registerPlayerService.registerPlayer to send data to api
   */
  register(): void {
    const dob: string |undefined = this.getDateOnly(this.AgeCtrl.value);

    if (this.PasswordCtrl.value === this.ConfirmPasswordCtrl.value) {
      this.passwordsNotMatch = false;
      
      let registerPlayer: RegisterPlayer = {
        gender: this.GenderCtrl.value,
        email: this.EmailCtrl.value,
        userName: this.UserNameCtrl.value,
        password: this.PasswordCtrl.value,
        confirmPassword: this.ConfirmPasswordCtrl.value,
        name: this.NameCtrl.value,
        lastName: this.LastNameCtrl.value,
        nationalCode: this.NationalCodeCtrl.value,
        height: this.HeightCtrl.value,
        age: dob,
        knownAs: this.KnownAsCtrl.value,
        city: this.CityCtrl.value,
        country: this.CountryCtrl.value
      }

      this.subscibedRegisterPlayer = this.registerPlayerService.registerPlayer(registerPlayer).subscribe({
        next: player => console.log(player),
        error: err => this.emailExistError = err.error
      });
    }
    else {
      this.passwordsNotMatch = true;
    }
  }

  /**
   * conver Angular Data t C# DateOnly
   * @param dob // yyyy/mm/dd/mm/ss. Takes DateOfBirth
   * @return yyyy/mm/dd
   */

  private getDateOnly(dob: string | null): string | undefined {
    if (!dob) return undefined;

    let theDob: Date = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0, 10);
  }

  getState(): void {
    console.log(this.registerFg);
  }
  //#endregion
}
