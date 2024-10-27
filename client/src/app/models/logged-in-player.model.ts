export interface LoggedInPlayer {
    token: string;
    userName: string;
    knownAs: string;
    gender: string;
    profilePhotoUrl: string;
    roles: string[];
}