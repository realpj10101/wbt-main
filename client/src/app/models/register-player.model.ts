export interface RegisterPlayer {
    email: string;
    userName: string;
    dateOfBirth: string | undefined;
    gender: string;
    password: string;
    confirmPassword: string;
}