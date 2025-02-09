import { PaginationParams } from "./paginationParams.model";

export class FollowParams extends PaginationParams{
    predicate: number = 0; // 0 => 'followings'; 1 => 'followers'
}