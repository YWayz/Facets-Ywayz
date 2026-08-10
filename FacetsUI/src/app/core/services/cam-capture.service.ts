import { Injectable } from "@angular/core";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { Observable, take, map } from "rxjs";
import { CamCaptureComponent } from "src/app/shared/components/cam-capture/cam-capture.component";

@Injectable({
    providedIn: 'root'
})

export class CamCaptureService {
    constructor(private dialog: MatDialog) { }
    dialogRef: MatDialogRef<CamCaptureComponent>;

    public open(options: any) {
        this.dialogRef = this.dialog.open(CamCaptureComponent, {
            data: {
                isCameraOpen: options.isCameraOpen
            }
        });
    }
    public confirmed(): Observable<any> {
        return this.dialogRef.afterClosed().pipe(take(1), map(res => {
            return res;
        }
        ));
    }
}