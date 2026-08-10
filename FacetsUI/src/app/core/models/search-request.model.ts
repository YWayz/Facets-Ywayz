export class SearchRequestModel {
    pageSize: number;
    pageNumber: number;
    searchTerm: string;
    totalRecords: number;
    currentPage: number;
    orderBy: string;

    constructor(pageSize: number, pageNumber: number) {
        this.pageSize = pageSize;
        this.pageNumber = pageNumber;
        this.searchTerm = '';
        this.orderBy = "";
    }
}