import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetUngVienForViewDto, UngVienDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewUngVienModal',
    templateUrl: './view-ungVien-modal.component.html'
})
export class ViewUngVienModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetUngVienForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetUngVienForViewDto();
        this.item.ungVien = new UngVienDto();
    }

    show(item: GetUngVienForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
