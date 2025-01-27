import { Pagination } from "./pagination.model";

export class PaginatedResult<T> {
    pagination?: Pagination; // api's response pagination values;
    body?: T; // api response body 
}