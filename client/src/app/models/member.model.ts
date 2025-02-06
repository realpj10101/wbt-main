import { Photo } from "./photo.model"

export interface Member {
    userName: string;
    name: string;
    lastName: string;
    age: number;
    height: number;
    weight: number;
    experienceLevel: string;
    skills: string;
    gamesPlayed: number;
    pointsPerGame: number;
    reboundsPerGame: number;
    assistsPerGame: number;
    bio: string
    achievements: string;
    created: Date;
    lastActive: Date;
    gender: string;
    city: string;
    region: string;
    country: string;
    photos: Photo[];
    isFollowing: boolean;
    isCaptain: boolean;
}