import { Component, ViewChild, Injector, Output, EventEmitter, ViewEncapsulation, OnInit} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { UngViensServiceProxy, CreateOrEditUngVienDto, TruongGiaoDichDto, TinhThanhDto, NoiDaoTaoDto, NoiDaoTaosServiceProxy, TinhThanhsServiceProxy, SessionServiceProxy, UngVienDto, LichSuLamViecDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import {formatDate} from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
import { DxFileUploaderComponent } from 'devextreme-angular';
import { Router } from '@angular/router';


@Component({
    selector: 'createUngVien',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './create-ungVien.component.html'
})
export class CreateUngVienComponent extends AppComponentBase implements OnInit{
  
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    
  
    active = false;
    saving = false;
    rootUrl : string ; 
    link : string; 
    truongGiaoDich: TruongGiaoDichDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
    trangThai: TruongGiaoDichDto[] = [];
    kenhTuyenDung: TruongGiaoDichDto[] = [];
    listComment: LichSuLamViecDto[] = [];
    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
    gioiTinh: TruongGiaoDichDto[] = [];
    congty: TruongGiaoDichDto[] = [];
    tinhTrangHonNhan: TruongGiaoDichDto[] = [];
    trinhDoDaoTao: TruongGiaoDichDto[] = [];
    xepLoaiHocLuc: TruongGiaoDichDto[] = [];
    tienDoTuyenDung: TruongGiaoDichDto[] = [];
    tinhThanh: TinhThanhDto[] = [];
    noiDaoTao: NoiDaoTaoDto[] = [];
    ungVien: CreateOrEditUngVienDto = new CreateOrEditUngVienDto();
    ngaySinh: Date;
    ngayCap: Date;
    approvE_DT: Date;
    time : string ; 
    uploadUrl: string;
    selectedRows = [];
    userId: number;
    currentDate = new Date();
    nameArr: any[]= []; 
    dataDisplay = [];
    value: any[] = [] ;
    currentTime: any;
    tepDinhKemSave = '';
    years:any;
    cmndExists = false;
    ungVienForCheckCMND: UngVienDto[] = [];
    tinhThanhIdSelected: number;
    listYear:any;
    defaultCbbOption: TruongGiaoDichDto[] = [];
    constructor(
        injector: Injector,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private router: Router,
        private _noiDaoTaosServiceProxy: NoiDaoTaosServiceProxy,
        private _tinhThanhsServiceProxy: TinhThanhsServiceProxy,
        private _session: SessionServiceProxy
    ) {
        super(injector);
      
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime ;

    }

    ngOnInit(): void {
        this.show();
        this.years = function(startYear) {
            var currentYear = new Date().getFullYear(), years = [];
            startYear = startYear || 1980;  
            while ( startYear <= currentYear ) {
                years.push(startYear++);
            }   
            return years;
        }
         this.listYear = this.years(2019-20);
          console.log(this.listYear)
    }

    troVe()
    {
        this.router.navigate(['/app/main/qlns/ungViens']);
    }
   
    //  ghep chuoi ten file 
    setFullNameFile(){
      
        this.dataDisplay.length = 0;
         const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        //  this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        console.log(this.currentTime)
        console.log( this.value)
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.value.forEach((ele)=>{        
            this.dataDisplay.push({tepDinhKem :cValue +"/"+ this.currentTime + "/"+ ele.name});  
        });
        this.value.length = 0;
        this.selectedRows = this.selectedRows.concat(this.dataDisplay);
        this.tepDinhKemSave = this.selectedRows.map(x => x.tepDinhKem.toString()).join(';');
    }

    show(): void {
        // this.ngaySinh = null;
        // this.ngayCap = null;
        // this.approvE_DT = null;
        this._noiDaoTaosServiceProxy.getAllNoiDaoTao().subscribe((res)=>{
            this.noiDaoTao = res;
        });
        this._tinhThanhsServiceProxy.getAllTinhThanh().subscribe((res)=>{
            this.tinhThanh = res;
            this.tinhThanhIdSelected = this.tinhThanh.filter(x => x.maTinhThanh === 'HCM')[0].id;
        });
        this.getAllTruongGiaoDich().subscribe((res)=>{
            this.truongGiaoDich = res;
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');
            this.trangThai = this.truongGiaoDich.filter(x => x.code == 'TRTH');
            this.kenhTuyenDung = this.truongGiaoDich.filter(x => x.code == 'KTD');
            this.gioiTinh = this.truongGiaoDich.filter(x => x.code == 'GT');
            this.congty = this.truongGiaoDich.filter(x => x.code == 'CT');
            this.tinhTrangHonNhan = this.truongGiaoDich.filter(x => x.code == 'TTHN');
            this.trinhDoDaoTao = this.truongGiaoDich.filter(x => x.code == 'TDDT');
            this.xepLoaiHocLuc = this.truongGiaoDich.filter(x => x.code == 'XLHL');
            this.tienDoTuyenDung = this.truongGiaoDich.filter(x => x.code == 'TDTD');
            this.defaultCbbOption = this.truongGiaoDich.filter(x => x.setDefault == true);
            console.log(this.defaultCbbOption);
            this.ungVien = new CreateOrEditUngVienDto();
            this.initUngvien();
        });

    }

    initUngvien(){
        this.ungVien.trangThaiCode = this.defaultCbbOption.find(x => x.code == 'TRTH') ? this.defaultCbbOption.find(x => x.code == 'TRTH').cdName : '';
        this.ungVien.gioiTinhCode = this.defaultCbbOption.find(x => x.code == 'GT') ? this.defaultCbbOption.find(x => x.code == 'GT').cdName : '';
        this.ungVien.tinhThanhID = this.tinhThanhIdSelected;
        this.ungVien.tinhTrangHonNhanCode = this.defaultCbbOption.find(x => x.code == 'TTHN') ? this.defaultCbbOption.find(x => x.code == 'TTHN').cdName : '';
        this.ungVien.trinhDoVanHoa = '12/12';
        this.ungVien.trinhDoDaoTaoCode = this.defaultCbbOption.find(x => x.code == 'TDDT') ? this.defaultCbbOption.find(x => x.code == 'TDDT').cdName : '';
        this.ungVien.tienDoTuyenDungCode = this.defaultCbbOption.find(x => x.code == 'TDTD') ? this.defaultCbbOption.find(x => x.code == 'TDTD').cdName : '';
    }

    save(): void {
        // this._ungViensServiceProxy.getAllCMND().subscribe((res)=>{
        //     if(this.ungVien.soCMND){
        //         console.log(this.ungVien.soCMND);
        //         this.ungVienForCheckCMND = res;
        //         if(this.ungVienForCheckCMND.findIndex(x => x.soCMND == this.ungVien.soCMND && this.ungVien.soCMND !== null) > -1){
        //             this.message.warn("Số CMND đã bị trùng!");
        //             return;
        //         }
        //     }
            // this.ungVienForCheckCMND = res;
            // if(this.ungVienForCheckCMND.findIndex(x => x.soCMND == this.ungVien.soCMND && this.ungVien.soCMND !== null) > -1){
            //     this.message.warn("Số CMND đã bị trùng!");
            //     return;
            // }
            // else{
                
               
                this.saving = true;
                if(!this.ungVien.hoVaTen  )
                {
                   this.message.warn(" Họ và tên không được bỏ trống!");
                   return;
                }
			
                if (this.ngaySinh) {

                        this.ungVien.ngaySinh = moment(this.ngaySinh);
                    
                }
                else {
                    this.ungVien.ngaySinh = null;
                }
               
                if (this.ngayCap) {

                        this.ungVien.ngayCap = moment(this.ngayCap);
      
                    
                }
                else {
                    this.ungVien.ngayCap = null;
                }
                if (this.approvE_DT) {
     
                        this.ungVien.approvE_DT = moment(this.approvE_DT);
                 
                }
                else {
                    this.ungVien.approvE_DT = null;
                }
                this.ungVien.tepDinhKem = this.tepDinhKemSave;
                    this._ungViensServiceProxy.createOrEdit(this.ungVien)
                     .pipe(finalize(() => { this.saving = false;}))
                     .subscribe(() => {
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.ungVien = new CreateOrEditUngVienDto();
                        this.initUngvien();
                        //this.close();
                        this.modalSave.emit(null);
                     });
            // }
        // });
        
    }
// chức năng  xóa file
    xoaFile(e:any)
    {
     
        console.log( this.selectedRows)
       this.selectedRows.splice(  this.selectedRows.indexOf(e.row.data.tepDinhKem), 1 );
       console.log( this.selectedRows)
           this.tepDinhKemSave = this.selectedRows.map(x => {return x.tepDinhKem.toString()}).join(';');

    }
    // chức năng xem file chi tiết
    showDetail(e:any)
    { 
        console.log(e)
       this.rootUrl = AppConsts.remoteServiceBaseUrl;
       this.link = this.rootUrl + "/" + e.data.tepDinhKem;
        console.log( this.link)
        window.open(this.link, '_blank');
    }

    close(): void {

        this.active = false;
    }
}
