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
import { NoAccessComponent } from './components/errors/no-access/no-access.component';
import { ServerErrorComponent } from './components/errors/server-error/server-error.component';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { UserEditComponent } from './components/user/user-edit/user-edit.component';
import { CommentComponent } from './components/comment/comment.component';
import { LikesComponent } from './components/likes/likes.component';
import { CreateTeamComponent } from './components/create-team/create-team.component';
import { TeamListComponent } from './components/teams/team-list/team-list.component';
import { TeamDetailsComponent } from './components/teams/team-details/team-details.component';
import { ChooseComponent } from './components/choose/choose.component';
import { RegisterCoachComponent } from './components/coach-account/register-coach/register-coach.component';
import { LoginCoachComponent } from './components/coach-account/login-coach/login-coach.component';
import { UsersComponent } from './components/admin/users/users.component';
import { CoachPanelComponent } from './components/coach-panel/coach-panel.component';
import { authLoggedInGuard } from './guards/auth-logged-in.guard';
import { authGuard } from './guards/auth.guard';
import { preventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'member-card', component: MemberCardComponent },
      { path: 'member-details/:userName', component: MemberDetailsComponent },
      { path: 'message', component: MessagesComponent },
      { path: 'friends', component: FriendsComponent },
      { path: 'no-access', component: NoAccessComponent },
      { path: 'user/user-edit', component: UserEditComponent, canDeactivate: [preventUnsavedChangesGuard] },
      { path: 'comment/:userName', component: CommentComponent },
      { path: 'likes', component: LikesComponent },
      { path: 'create-team', component: CreateTeamComponent },
      { path: 'teams', component: TeamListComponent },
      { path: 'team-details/:teamName', component: TeamDetailsComponent },
      { path: 'choose', component: ChooseComponent },
      { path: 'users', component: UsersComponent },
      { path: 'coach-panel', component: CoachPanelComponent },
    ]
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authLoggedInGuard],
    children: [
      { path: 'account/register', component: RegisterComponent },
      { path: 'account/login', component: LoginComponent },
      { path: 'coachaccount/register', component: RegisterCoachComponent },
      { path: 'coachaccount/login', component: LoginCoachComponent },
    ]
  },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: NotFoundComponent }
];
