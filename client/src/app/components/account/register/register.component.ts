import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterPlayer } from '../../../models/register-player.model';
import { RegisterPlayerService } from '../../../services/register-player.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { Subscription } from 'rxjs';
import { AutoFocusDirective } from '../../../directives/auto-focus.directive';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    MatSnackBarModule, MatRadioModule,
    MatDatepickerModule, MatNativeDateModule,
    AutoFocusDirective
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit, OnDestroy {
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
    name: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    lastNameCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    nationalCodeCtrl: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10)]],
    heightCtrl: ['', [Validators.required]],
    age: ['', [Validators.required]],
    knownAsCtrl: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(30)]],
    cityCtrl: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
    countryCtrl: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
  })
  //#endregion

}
