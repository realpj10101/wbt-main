import { Photo } from "./photo.model";

export interface ShowTeam {
    teamName: string;
    membersUserNames: string[];
    teamLevel: string;
    achievements: string;
    gamesPlayed: number;
    gamesWon: number;
    gamesLost: number;
    createdAt: Date;
    profilePhotoUrl: string;
    photos: Photo[];
}