export interface LoggedInUser {
    token: string;
    userName: string;
    gender: string;
    profilePhotoUrl: string;
    roles: string[];
}