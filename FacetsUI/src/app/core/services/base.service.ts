import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, map, catchError, throwError } from "rxjs";
import { environment } from "src/environments/environment";
import { ErrorResponse } from "../models/error-response.model";

@Injectable({
    providedIn: 'root',
})

export abstract class BaseService {
    private httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json',
        }),
    };

    protected httpOptionsFormType = {
        headers: new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
        })
    };

    protected httpOptionsFormDataType = {
        headers: new HttpHeaders({
            'enctype': 'multipart/form-data'
        })
    };

    errorMessage: ErrorResponse | undefined;
    http = inject(HttpClient) as HttpClient;

    constructor() { }

    private buildURL(actionurl: string): string {
        return `${environment.baseEndPoint}/${actionurl}`;
    }

    protected get<T>(actionUrl: string): Observable<T> {
        return this.http
            .get<T>(this.buildURL(actionUrl), this.httpOptions)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected post<T>(actionUrl: string, body: object): Observable<T> {
        return this.http
            .post<T>(this.buildURL(actionUrl), body, this.httpOptions)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected put(actionUrl: string, body?: object): Observable<Object> {
        return this.http
            .put(this.buildURL(actionUrl), body, this.httpOptions)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected putWithReturn<T>(actionUrl: string, body?: object): Observable<T> {
        return this.http
            .put<T>(this.buildURL(actionUrl), body, this.httpOptions)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected delete(actionUrl: string): Observable<Object> {
        return this.http
            .delete(this.buildURL(actionUrl), this.httpOptions)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected filePost<T>(actionUrl: string, files?: File[]): Observable<object> {
        let formData: FormData = new FormData();
        if (files != null && files != undefined && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                formData.append('file', files[i]);
            }
        }
        return this.http.post(this.buildURL(actionUrl), formData)
            .pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    protected filePut<T>(actionUrl: string, files?: FileList): Observable<object> {
        let formData: FormData = new FormData();
        if (files != null && files != undefined && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                formData.append('file', files[i]);
            }
        }
        return this.http.put(this.buildURL(actionUrl), formData).pipe(map((response) => (response)), catchError(this.handleServerError));
    }

    private handleServerError(error: Response) {
        // if (error.status === 0) {
        //   this.errorMessage = {
        //     traceId: null,
        //     success: false,
        //     errors: [
        //       {
        //         key: 'ReqCancelled',
        //         value: ['Please check your internet connection'],
        //       },
        //     ],
        //   };
        // }
        return throwError(() => error);
    }
}
