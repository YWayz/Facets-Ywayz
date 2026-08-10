import { NgModule } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import {MatExpansionModule} from '@angular/material/expansion';

@NgModule({
    declarations: [
    ],
    imports: [
        MatMenuModule,
        MatButtonModule,
        MatIconModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatDialogModule,
        MatTabsModule,
        MatFormFieldModule,
        MatSlideToggleModule,
        MatInputModule,
        MatExpansionModule
    ],
    exports: [
        MatMenuModule,
        MatListModule,
        MatSelectModule,
        MatTableModule,
        MatAutocompleteModule,
        MatButtonModule,
        MatIconModule,
        MatTabsModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatDialogModule,
        MatSlideToggleModule,
        MatFormFieldModule,
        MatInputModule,
        MatExpansionModule
    ]
})
export class MaterialModule { }
