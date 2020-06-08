import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { DxFormComponent, DxDataGridComponent, DxButtonComponent, DxSwitchComponent, DxNumberBoxComponent, DxDateBoxComponent, DxSelectBoxComponent } from 'devextreme-angular';
import { HopDongsServiceProxy, CreateOrEditHopDongDto, TruongGiaoDichDto, UngViensServiceProxy, UngVienDto, HoSosServiceProxy, DangKyKCBsServiceProxy, TemplatesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { formatDate } from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'editHopDong',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './edit-hoDong.component.html'
})
export class EditHopDongComponent extends AppComponentBase implements OnInit {
   
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    popupVisibleHoSo =false ; 
    truongGiaoDich: TruongGiaoDichDto[] = [];
    ungvien: UngVienDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
    loaiHopDong: any;
    hinhThucLamViec: TruongGiaoDichDto[] = [];
    thoiHanHopDong: TruongGiaoDichDto[] = [];
    linkHD:any;
    ngaySinh: Date;
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
    hopDongId:any; 
    dataRowDetail: any; 
    currentTime: any;
    nameNhanVien: any;
    tepDinhKemSave = '';
    cmndExists = false;
    hopDong: CreateOrEditHopDongDto = new CreateOrEditHopDongDto();

    ngayKy:Date;
    ngayCoHieuLuc:Date;
    ngayHetHan: Date;
    vitriCV:any;
    approvE_DT: Date;
    donViCongtac:any; 
    isReadonly = true;
    congTY:any ;
    num:any ;
    donViCOngTacID:any; 
    popupVisibleViTriCV= false ; 
    popupVisibleLHD =false ;
    selected:any; 
    maHD:any; 
    tenHD:any;
    tenLoaiHD :any; 
    listDataHoSo:any;

    constructor(
        injector: Injector,
        private router: Router,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private _hoSosServiceProxy: HoSosServiceProxy,
        private _dangKyKCBsServiceProxy: DangKyKCBsServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _templatesServiceProxy: TemplatesServiceProxy,
        private _hopDongsServiceProxy: HopDongsServiceProxy
    ) {
        super(injector);
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
    }



    ngOnInit(): void {

      
        this.getHoSos();
        this.show();
    }

    chonName()
    {
        this.popupVisibleHoSo = true ;
        this.isReadonly = !this.isReadonly;
     
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

        console.log(this.hopDong.donViCongTacID)
        console.log(name)
    }
    startEdit(e:any ) {
      
        console.log(e)
        this.dataRowDetail = e.data ;
        this.isReadonly = !this.isReadonly;
        console.log(this.dataRowDetail)
        this.name = this.dataRowDetail.hoVaTen ;
        this.hopDong.nhanVienId = this.dataRowDetail.maNhanVien;
         this.hopDong.hoTenNhanVien = this.dataRowDetail.hoVaTen;
        this.hopDong.viTriCongViecCode = this.dataRowDetail.chucDanh;
        this.hopDong.luongDongBaoHiem = this.dataRowDetail.luongDongBH;
        this.tenLoaiHD = this.dataRowDetail.loaiHopDongCode;

        console.log( this.tenLoaiHD)
        this.hopDong.loaiHopDongCode = this.dataRowDetail.kyHieuHD;
        this.maHD = this.dataRowDetail.maHD;
        this.tenHD = this.dataRowDetail.tenHopDong;
        this.hopDong.hinhThucLamViecCode = this.dataRowDetail.trangThaiLV;
        this.hopDong.tyLeHuongLuong = this.dataRowDetail.tyLeDongBH;
        this.donViCongtac  = this.dataRowDetail.donViCongTacName ;
        this.donViCOngTacID = this.dataRowDetail.donViCongTacID;
         this.hopDong.luongCoBan = this.dataRowDetail.luongCoBan;
         this.hopDong.recorD_STATUS = this.dataRowDetail.recorD_STATUS;
        this.popupVisibleHoSo = false ;
      
    }
    chonViTri(e:any)
    {
      
        this.hopDong.viTriCongViecCode = e.data.value;
        this.popupVisibleViTriCV = false;
    }
    chonViTriCV()
    {
        this.popupVisibleViTriCV = true;
    }
    chonLoaiHD()
    {
        this.popupVisibleLHD =true ;
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

    chonDV() {
        this.popupVisible = true;
    }
    show(): void {
        const Id = this._activatedRoute.snapshot.paramMap.get('id');
        // this.ngayKy = null;
        // this.ngayCoHieuLuc = null;
        // this.ngayHetHan = null;
        // this.approvE_DT = null;
       
        this.getAllTruongGiaoDich().subscribe((res) => {
            this.truongGiaoDich = res;
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');
            // console.log(this.viTriCongViec)
            this.congTY= this.truongGiaoDich.filter(x => x.code == 'CT'); 
            this.loaiHopDong = this.truongGiaoDich.filter(x => x.code == 'LHD');
            this.hinhThucLamViec = this.truongGiaoDich.filter(x => x.code == 'HTLV');
            this.thoiHanHopDong = this.truongGiaoDich.filter(x => x.code == 'THHD');

            this.hopDongId = Number.parseInt(Id);
            this._hopDongsServiceProxy.getHopDongForEdit(this.hopDongId).subscribe(result => {
                this.hopDong = result.hopDong;
                console.log(this.hopDong)
                if(this.hopDong.donViCongTacID != null)
                {
                    this._hoSosServiceProxy.getNameUnit(this.hopDong.donViCongTacID).subscribe(res =>{
                        this.donViCongtac= res;
                       
                    });
                }
                else
                {
                    this.hopDong.donViCongTacID = null;
                }

            

                this.tenLoaiHD = this.hopDong.loaiHopDongCode;
                // this.maHD = hopDong.cdName;


                if (this.hopDong.ngayKy) {
                  this.ngayKy = this.hopDong.ngayKy.toDate(); 
                }
                if (this.hopDong.ngayCoHieuLuc) {
                    this.ngayCoHieuLuc = this.hopDong.ngayCoHieuLuc.toDate(); 

                }
                if (this.hopDong.ngayHetHan) {
                    this.ngayHetHan = this.hopDong.ngayHetHan.toDate(); 
                }
                if (this.hopDong.approvE_DT) {
                    this.approvE_DT = this.hopDong.approvE_DT.toDate(); 
                }
                if(this.hopDong.tepDinhKem.length > 0){
                   
                    this.num = this.hopDong.tepDinhKem; 
                    this.num.split(';').forEach(element => {
                    this.selectedRows.push({tepDinhKem : element });
                
                    });
                }  

                this.active = true;

            });

        }); 
       
        
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
      troVe()
    {
        this.router.navigate(['/app/main/qlns/hopDongs']);
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

    save(): void {
        this.saving = true;   
        this.hopDong.recorD_STATUS =this.linkHD;
     if(this.idDV){
        this.hopDong.donViCongTacID  = this.donViCOngTacID;
    }
    if(this.tepDinhKemSave){
        this.hopDong.tepDinhKem = this.tepDinhKemSave;
    }
    if(this.maHD){
         this.hopDong.loaiHopDongCode =  this.tenLoaiHD;
    }
     
     if(!this.hopDong.tenHopDong  )
     {
        this.message.warn(" Tên hợp đồng không được bỏ trống!");
        return;
     }
     if( !this.hopDong.nhanVienId)
     {
        this.message.warn(" Mã nhân viên không được bỏ trống!");
        return; 
     }
     if(!this.hopDong.luongCoBan  )
     {
        this.message.warn(" Lương cơ bản không được bỏ trống!");
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

     if(this.ngayKy)
     {
        this.hopDong.ngayKy = moment(this.ngayKy);
     }
     if(this.ngayCoHieuLuc)
     {
        this.hopDong.ngayCoHieuLuc = moment(this.ngayCoHieuLuc);
     }
     if(this.ngayHetHan)
     {
        this.hopDong.ngayHetHan = moment(this.ngayHetHan);
     }
        this._hopDongsServiceProxy.createOrEdit(this.hopDong)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                // this.hopDong = new CreateOrEditHopDongDto();
                this.modalSave.emit(null);
       
            });
     
       
    }


    close(): void {

        this.active = false;
    
    }
}
