import { Injectable } from '@angular/core';
import { Workbook } from 'exceljs';
import * as fs from "file-saver";
import { CollectionReportModel } from 'src/app/modules/report/models/collection-report.model';
import { EventDateSelectionModel } from 'src/app/modules/report/models/event-date-selection.model';
import { convertOnlyDate } from '../extensions/helpers';

@Injectable({
    providedIn: 'root',
})

export class ExcelService {
    generateExcel(fileName: string, headers: string[], data: any) {
        let workbook = new Workbook();
        let worksheet = workbook.addWorksheet("Sheet1");
        worksheet.addRow(headers);
        for (let x of data) {
            let x2 = Object.keys(x);
            let temp: any[] = [];
            for (let y of x2) {
                temp.push(x[y as keyof Object])
            }
            worksheet.addRow(temp)
        }

        workbook.xlsx.writeBuffer().then((data) => {
            let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            fs.saveAs(blob, fileName + '-' + new Date().valueOf() + '.xlsx');
        });
    }

    generateCollectionExcelRepert(collectionReportModel: CollectionReportModel[], eventDates: EventDateSelectionModel[]) {
        // let workbook = new Workbook();
        // let worksheet = workbook.addWorksheet("Sheet1");

        // var excelColumns = [
        //     'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH'
        // ]

        // worksheet.getCell("A1").value = "Description";
        // worksheet.mergeCells("A1:A2");

        // let eventStartingIndex = 1;
        // eventDates.filter(p => p.isSelected).forEach(date => {
        //     worksheet.getCell(`${excelColumns[eventStartingIndex]}1`).value = convertOnlyDate(date.value);
        //     worksheet.mergeCells(`${excelColumns[eventStartingIndex]}1:${excelColumns[eventStartingIndex + 1]}1`);

        //     worksheet.getCell(`${excelColumns[eventStartingIndex]}2`).value = "Count";
        //     worksheet.getCell(`${excelColumns[eventStartingIndex + 1]}2`).value = "Amount"
        //     eventStartingIndex += 2;
        // });

        // let descriptionStartingIndex = 3;
        // let valueStartingNumericIndex = 3;
        // collectionReportModel.forEach(element => {
        //     worksheet.getCell(`A${descriptionStartingIndex}`).value = element.description;
        //     let valueStartingAlphbetIndex = 1;

        //     element.dateWiseCollection.forEach(col => {
        //         worksheet.getCell(`${excelColumns[valueStartingAlphbetIndex]}${valueStartingNumericIndex}`).value = col.registeredCount;
        //         worksheet.getCell(`${excelColumns[valueStartingAlphbetIndex + 1]}${valueStartingNumericIndex}`).value = col.amount;
        //         valueStartingAlphbetIndex += 2;
        //     });

        //     valueStartingNumericIndex += 1;
        //     descriptionStartingIndex += 1;
        // });

        // workbook.xlsx.writeBuffer().then((data) => {
        //     let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        //     fs.saveAs(blob, "Collection Report" + '-' + new Date().valueOf() + '.xlsx');
        // });
    }
}