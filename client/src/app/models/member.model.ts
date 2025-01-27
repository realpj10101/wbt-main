import { Photo } from "./photo.model"

export interface Member {
    userName: string;
    name: string;
    lastName: string;
    nationalCode: string;
    height: number;
    age: number;
    knownAs: string;
    created: Date;
    lastActive: Date;
    gender: string;
    lookingFor?: string;
    city: string;
    country: string;
    photos: Photo[];
    isFollowing: boolean;
    isCaptain: boolean;
}