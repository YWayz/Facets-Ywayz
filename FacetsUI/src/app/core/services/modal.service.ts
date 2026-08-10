import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { Observable, take, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ModalService {

  dialogRef: any;

  constructor(private dialog: MatDialog) { }

  public confirmed(): Observable<any> {
      return this.dialogRef.afterClosed().pipe(take(1), map(res => {
          return res;
      }
      ));
  }

  public displayDialog<T>(TCtor: new (...args: any[]) => T, data?: any): MatDialogRef<T, any> {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = data;
      dialogConfig.disableClose = true;
      this.dialogRef = this.dialog.open(TCtor, dialogConfig);
      return this.dialogRef;
  }
}
