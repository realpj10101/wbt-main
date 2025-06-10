import { AfterViewChecked, Component, effect, ElementRef, inject, OnDestroy, OnInit, signal, Signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamService } from '../../../services/team.service';
import { environment } from '../../../../environments/environment.development';
import { ActivatedRoute } from '@angular/router';
import { Gallery } from 'ng-gallery';
import { MatSnackBar } from '@angular/material/snack-bar';
import { take } from 'rxjs';
import { Member } from '../../../models/member.model';
import { ShowTeam } from '../../../models/show-team.model';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { MemberCardComponent } from "../../members/member-card/member-card.component";
import { TeamMembersCardComponent } from "../../team-members-card/team-members-card.component";
import { TeamPhotoEditorComponent } from "../team-photo-editor/team-photo-editor.component";
import { CoachAccountService } from '../../../services/coach-account.service';
import { TeamMessagingService } from '../../../services/team-messaging.service';
import { LoggedInUser } from '../../../models/logged-in-player.model';
import { FormsModule } from '@angular/forms';
import { IntlModule } from 'angular-ecmascript-intl';

@Component({
  selector: 'app-team-details',
  imports: [
    FormsModule, CommonModule,
    MatTabsModule, MatExpansionModule,
    TeamMembersCardComponent, TeamPhotoEditorComponent,
    IntlModule
  ],
  templateUrl: './team-details.component.html',
  styleUrl: './team-details.component.scss'
})
export class TeamDetailsComponent implements OnInit, AfterViewChecked {
  @ViewChild('messageContainer') private messageContainer!: ElementRef;
  private _teamService = inject(TeamService);
  apiUrl = environment.apiUrl;
  private _route = inject(ActivatedRoute);
  private _gallery = inject(Gallery);
  private _snack = inject(MatSnackBar);
  teamMessagingService = inject(TeamMessagingService);
  photoUrl = inject(CoachAccountService).profilePhotoUrl;
  team: ShowTeam | undefined;
  members: Member[] | undefined;
  currentTeamSig: Signal<ShowTeam | null> | undefined;
  isTeamLoaded = signal(false);
  teamSig = this._teamService.currentTeamSig;
  messageText = '';
  messages = this.teamMessagingService.messages;

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }

  isTeamLoadedSignal(): boolean {
    return this.isTeamLoaded();
  }

  ngOnInit(): void {
    this.getTeam();
    this.getTeamMembers();
    this.teamMessagingService.startConnection();
    this.teamMessagingService.loadMessage().subscribe();
  }

  getTeam(): void {
    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    if (teamName)
      this._teamService.getByTeamName(teamName)
        .pipe(
          take(1))
        .subscribe({
          next: (res: ShowTeam | null) => {
            if (res) {
              this.team = res;
              console.log(this.team);
            }
          }
        });
  }

  getTeamMembers(): void {
    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    if (teamName)
      this._teamService.getTeamMembersAsync(teamName)
        .pipe(take(1))
        .subscribe({
          next: (res: Member[]) => {
            if (res) {
              console.log(res);
              this.members = res;
            }
          }
        })
  }

  getCurrentUser(): LoggedInUser | null {
    const currentUser: string | null = localStorage.getItem('loggedInUser');

    return currentUser ? JSON.parse(currentUser) : null;
  }

  sendMessage(): void {
    const currentUser = this.getCurrentUser();

    const teamName: string | null = this._route.snapshot.paramMap.get('teamName');

    console.log('text', this.messageText);
    console.log('user', currentUser?.userName);
    console.log('team', teamName);

    if (this.messageText.trim() && currentUser && teamName) {
      this.teamMessagingService.sendMessage(currentUser.userName.toLowerCase(), this.messageText, teamName);
      this.messageText = '';
    }
  }

  isOnline(userName: string): boolean {
    // console.log('Checking online status for:', userName);
    let userOnlineStatus: boolean = this.teamMessagingService.isUserOnline(userName);

    console.log(userOnlineStatus);

    return userOnlineStatus;
  }

  getUserOnlineSignal(userName: string) {
    return this.teamMessagingService.getUserStatusSignal(userName);
  }

  getLastSeen(userName: string): string {
    const lastSeen = this.teamMessagingService.getLastSeen(userName);
    console.log('Getting last seen for:', userName, 'Result:', lastSeen);
    return lastSeen
      ? new Intl.DateTimeFormat('en-Us', {
        dateStyle: 'medium',
        timeStyle: 'short'
      }).format(lastSeen)
      : 'Unknown'; // Default to 'Unknown' if lastSeen is undefined
  }
}
