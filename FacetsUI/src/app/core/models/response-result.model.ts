export class ResponseResult<T> {
    data!: T;
    success!: boolean;
    totalRecordCount!: number;
    pageInfo!: {
        totalRecordCount: number;
        hasPreviousPage: boolean;
        hasNextPage: boolean;
        pageSize: number;
        previousPageReference: string;
        nextPageReference: string;
    }
}
