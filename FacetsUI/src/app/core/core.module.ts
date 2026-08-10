import { CommonModule } from "@angular/common";
import { ModuleWithProviders, NgModule, Optional, SkipSelf } from "@angular/core";
import { LayoutComponent } from './layout/layout/layout.component';
import { SharedModule } from "../shared/shared.module";

@NgModule({
    declarations: [
        LayoutComponent,
    ],
    imports: [
        CommonModule,
        SharedModule
    ]
})

export class CoreModule {
    constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
        if (parentModule) {
            throw new Error('CoreModule has already been loaded. You should only import Core modules in the AppModule only.');
        }
    }
    static forRoot(): ModuleWithProviders<CoreModule> {
        return {
            ngModule: CoreModule
        };
    }
}