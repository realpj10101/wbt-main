export interface RegisterPlayer {
    email: string;
    userName: string;
    password: string;
    confirmPassword: string;
    name: string;
    lastName: string;
    nationalCode: string;
    height: number;
    age: string | undefined;
    knownAs: string;
    gender: string;
    city: string;
    country: string;
}