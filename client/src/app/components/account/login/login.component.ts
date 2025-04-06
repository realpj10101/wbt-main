import { Component, inject } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginPlayer } from '../../../models/login-player.model';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AutoFocusDirective } from '../../../directives/auto-focus.directive';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    AutoFocusDirective, MatSnackBarModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  registerPlayerService = inject(AccountService);
  fb = inject(FormBuilder);
  private router = inject(Router);

  wrongUserNameOrPassword: string | undefined;

  loginFg: FormGroup = this.fb.group({
    emailCtrl: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$/)]],
    passwordCtrl: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(20)]]
  })

  get EmailCtrl(): FormControl {
    return this.loginFg.get('emailCtrl') as FormControl;
  }

  get PasswordCtrl(): FormControl {
    return this.loginFg.get('passwordCtrl') as FormControl;
  }

  login(): void {
    let loginPlayer: LoginPlayer = {
      email: this.EmailCtrl.value,
      password: this.PasswordCtrl.value
    }

    this.registerPlayerService.loginPlayer(loginPlayer).subscribe({
      next: (loggedInPlayer: LoggedInUser | null) => {
        console.log(loggedInPlayer);
      },
      // show wrong username or password error.
      error: err => {
        this.wrongUserNameOrPassword = err.error;
      }
    })
  }

  getState(): void {
    console.log(this.PasswordCtrl);
  }
}
