import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { GetHoSoForViewDto, HoSoDto ,CreateOrEditHoSoDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewHoSoModal',
    templateUrl: './view-hoSo-modal.component.html'
})
export class ViewHoSoModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: CreateOrEditHoSoDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new CreateOrEditHoSoDto();
       
       // this.profilePicture = AppConsts.remoteServiceBaseUrl + "\\" + cValue + "\\" + x.name;
        // this.item.hoSo = new HoSoDto();
    }

    show(item: CreateOrEditHoSoDto): void {
        this.item = item;
        console.log(this.item)
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
