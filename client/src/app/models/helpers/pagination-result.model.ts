import { Pagination } from "./pagination.model";

export class PaginationResult<T> {
    pagination?: Pagination; // api's response pagination values;
    body?: T; // api response body 
}