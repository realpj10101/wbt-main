export class FollowParams {
    pageNumber: number = 1;
    pageSize: number = 3;
    predicate: number = 0; // 0 => 'followings'; 1 => 'followers'
}