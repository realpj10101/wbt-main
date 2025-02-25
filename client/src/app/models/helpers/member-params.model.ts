import { PaginationParams } from "./paginationParams.model";

export class MemberParams extends PaginationParams {
    search: string | undefined;
    orderBy = 'lastAcive';
}