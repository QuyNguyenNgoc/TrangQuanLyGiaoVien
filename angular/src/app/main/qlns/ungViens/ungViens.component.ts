import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UngViensServiceProxy, UngVienDto, TruongGiaoDichDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { ViewUngVienModalComponent } from './view-ungVien-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from 'primeng/fileupload';
import { PermissionTreeModalComponent } from '@app/admin/shared/permission-tree-modal.component';
import { finalize } from 'rxjs/operators';
import { formatDate } from '@angular/common';
import { DxFileUploaderComponent } from 'devextreme-angular';
import { DxFileManagerModule, DxPopupModule, DxDataGridComponent } from 'devextreme-angular';
import { UtilityService } from '@shared/utils/UltilityService.service';

@Component({
    templateUrl: './ungViens.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class UngViensComponent extends AppComponentBase implements OnInit {
    
    @ViewChild(DxDataGridComponent, { static: false }) gridContainer: DxDataGridComponent;
    @ViewChild('viewUngVienmodal.component', { static: true }) viewUngVienModal: ViewUngVienModalComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('ExcelFileUpload', { static: true }) excelFileUpload: FileUpload;
    @ViewChild('fileUploader1', { static: false }) uploader1: DxFileUploaderComponent;  
    @ViewChild('permissionFilterTreeModal', { static: true }) permissionFilterTreeModal: PermissionTreeModalComponent;
    advancedFiltersAreShown = false;
    filterText = '';
    maUngVienFilter = '';
    hoVaTenFilter = '';
    viTriUngTuyenCodeFilter = '';
    kenhTuyenDungCodeFilter = '';
    gioiTinhCodeFilter = '';
    trangThaiCodeFilter = '';
    maxNgaySinhFilter: moment.Moment;
    minNgaySinhFilter: moment.Moment;
    soCMNDFilter = '';
    trinhDoVanHoaFilter = '';
    maxNamTotNghiepFilter: number;
    maxNamTotNghiepFilterEmpty: number;
    minNamTotNghiepFilter: number;
    minNamTotNghiepFilterEmpty: number;
    tienDoTuyenDungCodeFilter = '';
    recorD_STATUSFilter = '';
    maxMARKER_IDFilter: number;
    currentDate = new Date();
    maxMARKER_IDFilterEmpty: number;
    minMARKER_IDFilter: number;
    minMARKER_IDFilterEmpty: number;
    autH_STATUSFilter = '';
    dienThoaiFilter = '';
    emailFilter = '';
    diaChiFilter = '';
    listDataUngVien :any; 
    time1Filter = '';
    time2Filter = '';
    time3Filter = '';
    noteFilter = '';
    maxDay1Filter: moment.Moment;
    minDay1Filter: moment.Moment;
    maxDay2Filter: moment.Moment;
    minDay2Filter: moment.Moment;
    maxDay3Filter: moment.Moment;
    minDay3Filter: moment.Moment;

    dataDisplay: any;
    selectedRows = [];
    currentTime: any;
    truongGiaoDich: TruongGiaoDichDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
    loaiHopDong: TruongGiaoDichDto[] = [];
    hinhThucLamViec: TruongGiaoDichDto[] = [];
    trangThai: TruongGiaoDichDto[] = [];
    kenhTuyenDung: TruongGiaoDichDto[] = [];
    tienDoTuyenDung: TruongGiaoDichDto[] = [];
    gioiTinh: TruongGiaoDichDto[] = [];
    tinhTrangHonNhan: TruongGiaoDichDto[] = [];
    trinhDoDaoTao: TruongGiaoDichDto[] = [];
    xepLoaiHocLuc: TruongGiaoDichDto[] = [];
    frozenCols: any[];
    selectedRowsData: any[] = [];
    value: any[] = [];
    rootUrl = '';
    tepDinhKemSave = '';
    uploadUrl: any;
    id: any;
    data:any;
    a = true ; 

    constructor(
        injector: Injector,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private _notifyService: NotifyService,
        private ultility: UtilityService,
        private _tokenAuth: TokenAuthServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private router: Router,
        private _httpClient: HttpClient,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
        this.rootUrl = AppConsts.remoteServiceBaseUrl;
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();

        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;

    }
    getSelectedRowKeys() {
        this.selectedRowsData = this.gridContainer.instance.getSelectedRowsData();
        if (this.selectedRowsData.length == 0) {
            return;
        }
        this.selectedRowsData.forEach(element => {
            this.selectedRows.push(element["Id"]);
        });
        this.ultility.selectedRows = this.selectedRows;
        console.log(this.selectedRows)
    }

    ngOnInit(): void {
        this.getAllTruongGiaoDich().subscribe((res) => {
            this.truongGiaoDich = res;
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');
            this.tienDoTuyenDung = this.truongGiaoDich.filter(x => x.code == 'TDTD');
            this.trangThai = this.truongGiaoDich.filter(x => x.code == 'TRTH');
            this.kenhTuyenDung = this.truongGiaoDich.filter(x => x.code == 'KTD');
            this.gioiTinh = this.truongGiaoDich.filter(x => x.code == 'GT');
            this.tinhTrangHonNhan = this.truongGiaoDich.filter(x => x.code == 'TTHN');
            this.trinhDoDaoTao = this.truongGiaoDich.filter(x => x.code == 'TDDT');
            this.xepLoaiHocLuc = this.truongGiaoDich.filter(x => x.code == 'XLHL');
        });
      this.getUngViens();
    
    }
    getUngViens() {
        this._ungViensServiceProxy.getAll(
            this.filterText,
            this.maUngVienFilter,
            this.hoVaTenFilter,
            this.viTriUngTuyenCodeFilter,
            this.kenhTuyenDungCodeFilter,
            this.gioiTinhCodeFilter,
            this.trangThaiCodeFilter,
            this.maxNgaySinhFilter,
            this.minNgaySinhFilter,
            this.soCMNDFilter,
            this.trinhDoVanHoaFilter,
            this.maxNamTotNghiepFilter == null ? this.maxNamTotNghiepFilterEmpty : this.maxNamTotNghiepFilter,
            this.minNamTotNghiepFilter == null ? this.minNamTotNghiepFilterEmpty : this.minNamTotNghiepFilter,
            this.tienDoTuyenDungCodeFilter,
            this.recorD_STATUSFilter,
            this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
            this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
            this.autH_STATUSFilter,
            this.dienThoaiFilter,
            this.emailFilter,
            this.diaChiFilter,
            this.maxDay1Filter,
            this.minDay1Filter,
            this.maxDay2Filter,
            this.minDay2Filter,
            this.maxDay3Filter,
            this.minDay3Filter,
            this.time1Filter,
            this.time2Filter,
            this.time3Filter,
            this.noteFilter,
             "1",
             0,
             100000
        ).subscribe(result => {
            let count = 0;
            console.log(result);
            this.listDataUngVien = result.items; 
            for(var i = 0, len =  this.listDataUngVien.length; i < len; i++){
                this.listDataUngVien[i]["stt"] = ++count;

            }
            console.log(this.listDataUngVien);
        });
    }


    createUngVien(): void {
        this.router.navigate(['/app/main/qlns/ungViens/create']);
    }

    editUngVien(): void {
     
        this.router.navigate(['/app/main/qlns/ungViens/edit/' + this.id]);
    }

    deleteUngVien(): void {
        this.message.confirm(
            '', this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this._ungViensServiceProxy.delete(this.id)
                        .subscribe(() => {
                            // this.reloadPage();
                            this.getUngViens();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }
    startEdit(e:any )
    {
        this.data=e.data ;
        console.log(this.data)
        this.id= e.data.ungVien.id;
        this.a = !this.a;
        
    }

    importToExcel() {
       
        const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        this.value.forEach((ele) => {
            this.dataDisplay = ele.name
            this._ungViensServiceProxy.importToExcel(this.currentTime, this.dataDisplay).subscribe(res => { 
                if( res == null)
                {
                    this.message.success("Import thành công");
                    this.getUngViens();              
                }
                else
                {
                    this.message.info("Nhập liệu chưa đúng");
                }
              
            })
         this.value.length = 0
        });

    }

    dowloadTemplate() {

        this.rootUrl = AppConsts.appBaseUrl;
        let link = this.rootUrl + '/assets/sampleFiles/FIleMauImportUngVien.xlsx';
        console.log(link)
        window.open(link, '_blank');
    }
    exportToExcel(): void {
        this._ungViensServiceProxy.getUngViensToExcel(
            this.filterText,
            this.maUngVienFilter,
            this.hoVaTenFilter,
            this.viTriUngTuyenCodeFilter,
            this.kenhTuyenDungCodeFilter,
            this.gioiTinhCodeFilter,
            this.trangThaiCodeFilter,
            this.maxNgaySinhFilter,
            this.minNgaySinhFilter,
            this.soCMNDFilter,
            this.trinhDoVanHoaFilter,
            this.maxNamTotNghiepFilter == null ? this.maxNamTotNghiepFilterEmpty : this.maxNamTotNghiepFilter,
            this.minNamTotNghiepFilter == null ? this.minNamTotNghiepFilterEmpty : this.minNamTotNghiepFilter,
            this.tienDoTuyenDungCodeFilter,
            this.recorD_STATUSFilter,
            this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
            this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
            this.autH_STATUSFilter,
            this.dienThoaiFilter,
            this.emailFilter,
            this.diaChiFilter,
            this.maxDay1Filter,
            this.minDay1Filter,
            this.maxDay2Filter,
            this.minDay2Filter,
            this.maxDay3Filter,
            this.minDay3Filter,
            this.time1Filter,
            this.time2Filter,
            this.time3Filter,
            this.noteFilter,
        )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }
}
