import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent, DxSwitchComponent, DxNumberBoxComponent, DxDateBoxComponent, DxSelectBoxComponent } from 'devextreme-angular';
import { HopDongsServiceProxy, CreateOrEditHopDongDto, TruongGiaoDichDto, UngViensServiceProxy, UngVienDto, HoSosServiceProxy, TemplatesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { formatDate } from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
import { Router } from '@angular/router';

@Component({
    selector: 'createHopDong',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './create-hoDong.component.html'
})
export class CreateHopDongComponent extends AppComponentBase implements OnInit {
   
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    popupVisibleHoSo =false ; 
    truongGiaoDich: TruongGiaoDichDto[] = [];
    ungvien: UngVienDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
   
    hinhThucLamViec: TruongGiaoDichDto[] = [];
    thoiHanHopDong: TruongGiaoDichDto[] = [];
    ngaySinh: Date;
    selected:any;
    ngayCap: Date;
    popupVisible = false;
    time: string;
    uploadUrl: string;
    thoihanHD:any;
    rootUrl: any;
    link: any;
    hoso:any;
    name: any;
    idDV: any;
    selectedRows = [];
    userId: number;
    currentDate = new Date();
    nameArr: any[] = [];
    dataDisplay = [];
    value: any[] = [];
    dataRowDetail: any; 
    currentTime: any;
    nameNhanVien: any;
    tepDinhKemSave = '';
    cmndExists = false;
    hopDong: CreateOrEditHopDongDto = new CreateOrEditHopDongDto();

    ngayKy : Date;
    ngayCoHieuLuc: Date;
    ngayHetHan: Date;
    vitriCV: any; 
    approvE_DT: Date;
    tenHD:any;
    congTY:any; 
    donViCongtac:any; 
    tenNV: any; 
    loaiHopDong:any;
    loaiHD:any;
    donViCOngTacID:any; 
    popupVisibleViTriCV= false ; 
    isReadonly = true;
    popupVisibleLHD= false ;
    maHD:any; 
    linkHD:any;
    tenLoaiHD :any; 
    listDataHoSo:any; 
    constructor(
        injector: Injector,
        private router: Router,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private _hoSosServiceProxy: HoSosServiceProxy,
        private _templatesServiceProxy: TemplatesServiceProxy,
        private _hopDongsServiceProxy: HopDongsServiceProxy
    ) {
        super(injector);
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
    }

  

    ngOnInit(): void {

        // this._hoSosServiceProxy.getListInFoHoSo().subscribe(res => {
        //     this.hoso = res;
        //     console.log(this.hoso)
        // });
        this.show();
        this.getHoSos();
    }
    chonLoaiHD()
    {
        this.popupVisibleLHD =true ;
    }
 

 
       
       
         getHoSos()
         {
           this._hoSosServiceProxy.getAllHoSO().subscribe(res => {
             // this.listDataHoSo = res;
             // console.log( this.listDataHoSo)
             let count = 0;
             console.log(res);
             this.listDataHoSo = res; 
             for(var i = 0, len =  this.listDataHoSo.length; i < len; i++){
                 this.listDataHoSo[i]["stt"] = ++count;      
             }
             console.log( this.listDataHoSo)
            });
           
         }
    getIdSelected(event: any) {
        console.log(event);
        this.hopDong.donViCongTacID = Number.parseInt(event[0].id);
        this.donViCOngTacID = this.hopDong.donViCongTacID;
        this.donViCongtac = event[0].name;
    }
    startEdit(e:any ) {
      
        console.log(e)
        this.dataRowDetail = e.data ;
        this.isReadonly = !this.isReadonly;
        console.log(this.dataRowDetail)
        this.name = this.dataRowDetail.hoVaTen ;
        this.hopDong.nhanVienId = this.dataRowDetail.maNhanVien;
         this.hopDong.hoTenNhanVien = this.dataRowDetail.hoVaTen;
        this.hopDong.viTriCongViecCode = this.dataRowDetail.viTriCongViecCode;
        this.hopDong.luongDongBaoHiem = this.dataRowDetail.luongDongBH;
        this._templatesServiceProxy.getTemplateForView(this.dataRowDetail.loaiHopDongID).subscribe(res =>{
            console.log(res)

            this.tenLoaiHD = res.template.tenTemplate;
            // this.idHopDong= res.template.id;
           })
        // this.tenLoaiHD = this.dataRowDetail.loaiHopDongCode;
        this.hopDong.loaiHopDongCode = this.dataRowDetail.kyHieuHD;
        this.maHD = this.dataRowDetail.maHD;
        this.tenHD = this.dataRowDetail.tenHopDong;
        this.hopDong.hinhThucLamViecCode = this.dataRowDetail.trangThaiLV;
        this.hopDong.tyLeHuongLuong = this.dataRowDetail.tyLeDongBH;
        this.donViCongtac  = this.dataRowDetail.donViCongTacName ;
        this.donViCOngTacID = this.dataRowDetail.donViCongTacID;
         this.hopDong.luongCoBan = this.dataRowDetail.luongCoBan;
         this.linkHD = this.dataRowDetail.recorD_STATUS;
        this.popupVisibleHoSo = false ;
      
    }
    chonViTri(e:any)
    {
      
        this.hopDong.viTriCongViecCode = e.data.value;
        this.popupVisibleViTriCV = false;
    }
    chonLoaiHopDong(e:any)
    {
        console.log(e)
        this.hopDong.loaiHopDongCode = e.data.tenTemplate;
        this.tenLoaiHD= this.hopDong.loaiHopDongCode;
         this.maHD = e.data.maTemplate ; 
         this.linkHD= e.data.linkTemplate;
        this.popupVisibleLHD = false;
       
    }
    chonViTriCV()
    {
        this.popupVisibleViTriCV = true;
    }

    chonName()
    {
        this.popupVisibleHoSo = true ;
        this.isReadonly = !this.isReadonly;
        console.log(this.isReadonly)
    }
    chonDV() {
        this.popupVisible = true;
    }
    show(): void {
        // this.ngayKy = null;
        // this.ngayCoHieuLuc = null;
        // this.ngayHetHan = null;
        // this.approvE_DT = null;
       
        this.getAllTruongGiaoDich().subscribe((res) => {
            this.truongGiaoDich = res;
            this.congTY= this.truongGiaoDich.filter(x => x.code == 'CT');      
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');
            console.log(this.viTriCongViec)
            // this.loaiHopDong = this.truongGiaoDich.filter(x => x.code == 'LHDGS');
            this.hinhThucLamViec = this.truongGiaoDich.filter(x => x.code == 'HTLV');
            this.thoiHanHopDong = this.truongGiaoDich.filter(x => x.code == 'THHD');
        }); 
     
        this.hopDong = new CreateOrEditHopDongDto();


    }

    onValueChanged (e) {
      console.log(e)
        this.selected =  e.value ; 
        this.loaiHopDong = this.truongGiaoDich.filter(x => x.ghiChu ==  this.selected);
        console.log( this.loaiHopDong)

        this._templatesServiceProxy.getListTemplate( this.selected).subscribe(res =>{

            this.loaiHopDong= res;
            let count = 0;
        
            for(var i = 0, len =  this.loaiHopDong.length; i < len; i++){
                this.loaiHopDong[i]["stt"] = ++count;      
                console.log( this.loaiHopDong)
            }
          })
       
    }
    setFullNameFile() {
        this.dataDisplay.length = 0;
        const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        this.value.forEach((ele) => {
            this.dataDisplay.push({ tepDinhKem: cValue + "/" + this.currentTime + "/" + ele.name });

        });
        this.value.length = 0;
        this.selectedRows = this.selectedRows.concat(this.dataDisplay);
        this.tepDinhKemSave = this.selectedRows.map(x => x.tepDinhKem.toString()).join(';');
    }
    showDetail(e: any) {
        console.log(e)
        this.rootUrl = AppConsts.remoteServiceBaseUrl;
        this.link = this.rootUrl + "/" + e.data.tepDinhKem;
        console.log(this.link)
        window.open(this.link, '_blank');
    }
    xoaFile(e: any) {

        console.log(this.selectedRows)
        this.selectedRows.splice(this.selectedRows.indexOf(e.row.data.tepDinhKem), 1);
        console.log(this.selectedRows)
        this.tepDinhKemSave = this.selectedRows.map(x => { return x.tepDinhKem.toString() }).join(';');

    }
    troVe()
    {
        this.router.navigate(['/app/main/qlns/hopDongs']);
    }

    save(): void {
        this.saving = true;
        this.hopDong.donViCongTacID = this.donViCOngTacID;
          this.hopDong.tepDinhKem = this.tepDinhKemSave; 
          this.hopDong.loaiHopDongCode =  this.tenLoaiHD;
          this.hopDong.recorD_STATUS =this.linkHD;

          console.log(this.hopDong.recorD_STATUS)
        // this.hopDong.loaiHopDongCode =  this.maHD ; 
     if(!this.hopDong.tenHopDong  )
     {
        this.message.warn(" Tên hợp đồng không được bỏ trống!");
        return;
     }
     if(!this.hopDong.luongCoBan  )
     {
        this.message.warn(" Lương cơ bản không được bỏ trống!");
        return;
     }
    if( !this.hopDong.nhanVienId)
     {
        this.message.warn(" Mã nhân viên không được bỏ trống!");
        return; 
     }
     if(!this.ngayKy)
     {
        this.message.warn(" Ngày ký HD không được bỏ trống!");
        return; 
     }
     if(!this.ngayCoHieuLuc)
     {
        this.message.warn(" Ngày có hiệu lực không được bỏ trống!");
        return; 
     }
     if(!this.ngayHetHan)
     {
        this.message.warn(" Ngày hết hạn không được bỏ trống!");
        return; 
     }
      if(!this.hopDong.loaiHopDongCode)
     {
        this.message.warn(" Loại hợp đồng không được bỏ trống!");
        return; 
     }
     if(!this.hopDong.hoTenNhanVien)
     {
        this.message.warn(" Họ tên không được bỏ trống!");
        return; 
     }
     if(!this.hopDong.donViCongTacID)
     {
        this.message.warn("Đơn vị công tác không được bỏ trống!");
        return; 
     }
     if(!this.hopDong.viTriCongViecCode)
     {
        this.message.warn("Vị trí công việc không được bỏ trống!");
        return; 
     }
    //  this.hopDong.ngayKy = moment(this.ngayKy)
    //  this.hopDong.ngayHetHan = moment(this.ngayHetHan)
    //  this.hopDong.ngayCoHieuLuc = moment(this.ngayCoHieuLuc)
     
        if (this.ngayKy) {

                this.hopDong.ngayKy = moment(this.ngayKy);
            
        }
        else {
            this.hopDong.ngayKy = null;
        }
        if (this.ngayCoHieuLuc) {

                this.hopDong.ngayCoHieuLuc = moment(this.ngayCoHieuLuc);
            
        }
        // else {
        //     this.hopDong.ngayCoHieuLuc = null;
        // }
        if (this.ngayHetHan) {

                this.hopDong.ngayHetHan = moment(this.ngayHetHan);
            
        }
        // else {
        //     this.hopDong.ngayHetHan = null;
        // }
        if (this.approvE_DT) {

                this.hopDong.approvE_DT = moment(this.approvE_DT);
            }
        
        else {
            this.hopDong.approvE_DT = null;
        }
  
        console.log(this.hopDong)
        this._hopDongsServiceProxy.createOrEdit(this.hopDong)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.hopDong = new CreateOrEditHopDongDto();
                this.modalSave.emit(null);
       
            });
    

      
    }







    close(): void {

        this.active = false;
        // this.modal.hide();
    }
}
