import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetSqlConfigDetailForViewDto, SqlConfigDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewSqlConfigDetailModal',
    templateUrl: './view-sqlConfigDetail-modal.component.html'
})
export class ViewSqlConfigDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetSqlConfigDetailForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetSqlConfigDetailForViewDto();
        this.item.sqlConfigDetail = new SqlConfigDetailDto();
    }

    show(item: GetSqlConfigDetailForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
