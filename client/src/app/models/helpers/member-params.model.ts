import { PaginationParams } from "./paginationParams.model";

export class MemberParams extends PaginationParams {
    search: string | undefined;
    orderBy = 'lastAcive';
    minAge = 6;
    maxAge = 99;
    gender: string | undefined;

    constructor(gender: string) {
        super(); // Constructors of derived classes must contain a 'super' call to properly set up the inheritance chain.
        this.gender = gender === 'female' ? 'male' : 'female';
    }
}