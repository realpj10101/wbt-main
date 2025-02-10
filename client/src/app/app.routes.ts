import { Routes } from '@angular/router';
import { RegisterComponent } from './components/account/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { FooterComponent } from './components/footer/footer.component';
import { LoginComponent } from './components/account/login/login.component';
import { MemberListComponent } from './components/members/member-list/member-list.component';
import { MemberCardComponent } from './components/members/member-card/member-card.component';
import { MemberDetailsComponent } from './components/members/member-details/member-details.component';
// import { authGuard } from './guards/auth.guard';
import { MessagesComponent } from './components/messages/messages.component';
import { FriendsComponent } from './components/friends/friends.component';
import { AdminComponent } from './components/admin/admin.component';
import { NoAccessComponent } from './components/errors/no-access/no-access.component';
import { ServerErrorComponent } from './components/errors/server-error/server-error.component';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
// import { authLoggedInGuard } from './guards/auth-logged-in.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  // {
  //   path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [authGuard],
  //   children: [
      { path: 'members', component: MemberListComponent },
      { path: 'member-card', component: MemberCardComponent },
      { path: 'member-details/:userName', component: MemberDetailsComponent },
      { path: 'message', component: MessagesComponent },
      { path: 'friends', component: FriendsComponent },
      { path: 'admin', component: AdminComponent },
      { path: 'no-access', component: NoAccessComponent },
  //   ]
  // },
  // {
  //   path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [authLoggedInGuard],
  //   children: [
      { path: 'account/register', component: RegisterComponent },
      { path: 'account/login', component: LoginComponent },
  //   ]
  // },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: NotFoundComponent }
];
