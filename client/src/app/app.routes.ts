import { Routes } from '@angular/router';
import { RegisterComponent } from './components/account/register/register.component';
import {HomeComponent} from './components/home/home.component';
import {FooterComponent} from './components/footer/footer.component';
import {LoginComponent} from './components/account/login/login.component';
import { MemberListComponent } from './components/members/member-list/member-list.component';

export const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'login', component: LoginComponent},
  {path: 'members', component: MemberListComponent}
];
