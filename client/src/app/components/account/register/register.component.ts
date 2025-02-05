import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterPlayer } from '../../../models/register-player.model';
import { AccountService } from '../../../services/account.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { max, Subscription } from 'rxjs';
import { AutoFocusDirective } from '../../../directives/auto-focus.directive';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatSnackBarModule, MatRadioModule,
    MatDatepickerModule, MatNativeDateModule,
    AutoFocusDirective, RouterLink
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit, OnDestroy {
  registerPlayerService = inject(AccountService);
  fb = inject(FormBuilder);

  minDate = new Date(); //yyyy/mm/dd/hh/mm/ss
  maxDate = new Date();

  passwordsNotMatch: boolean | undefined;
  subscribedRegisterPlayer: Subscription | undefined;
  emailExistError: string | undefined;

  ngOnInit(): void {
      const currentYear = new Date().getFullYear();
      this.minDate = new Date(currentYear - 99, 0, 1);
      this.maxDate = new Date(currentYear - 6, 0, 1);
  }

  ngOnDestroy(): void {
    this.subscribedRegisterPlayer?.unsubscribe();
  }

  registerFg = this.fb.group({
    genderCtrl: ['female', [Validators.required]],
    emailCtrl: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$/)]],
    userNameCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    dateOfBirthCtrl: ['', [Validators.required]],
    passwordCtrl: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(20)]],
    confirmPasswordCtrl: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(20)]]
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

  get DateOfBirthCtrl(): FormControl {
    return this.registerFg.get('dateOfBirthCtrl') as FormControl;
  }

  get PasswordCtrl(): FormControl {
    return this.registerFg.get('passwordCtrl') as FormControl;
  }

  get ConfirmPasswordCtrl(): FormControl {
    return this.registerFg.get('confirmPasswordCtrl') as FormControl;
  }

  register(): void {
    const dob: string | undefined = this.getDateOnly(this.DateOfBirthCtrl.value); 

    if (this.PasswordCtrl.value === this.ConfirmPasswordCtrl.value) {
      this.passwordsNotMatch = false;

      let regiterPlayer: RegisterPlayer = {
        gender: this.GenderCtrl.value,
        email: this.EmailCtrl.value,
        userName: this.UserNameCtrl.value,
        dateOfBirth: dob,
        password: this.PasswordCtrl.value,
        confirmPassword: this.ConfirmPasswordCtrl.value
      }

      this.subscribedRegisterPlayer = this.registerPlayerService.registerPlayer(regiterPlayer).subscribe({
        next: player => console.log(player),
        error: err => this.emailExistError = err.error
      });

      console.log(this.subscribedRegisterPlayer);
    }
    else {
      this.passwordsNotMatch = true;
    }
  }

  getDateOnly(dob: string | null): string | undefined {
    if (!dob) return undefined;

    let theDob: Date = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0, 10);
  }
}
