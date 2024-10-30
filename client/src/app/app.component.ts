import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorsComponent } from "./components/errors/errors.component";
import { RegisterComponent } from "./components/account/register/register.component";
import { LoginComponent } from "./components/account/login/login.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorsComponent, RegisterComponent, LoginComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client';
}
