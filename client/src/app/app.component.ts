import { Component, Inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RegisterComponent } from "./components/account/register/register.component";
import { FormBuilder, FormControl, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    ReactiveFormsModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule,
    RegisterComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  fB = Inject(FormBuilder);

  registerFg = this.fB.group({
    emailCtrl: ['']
  })
}
