import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HopDongsServiceProxy, HopDongDto, TruongGiaoDichDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
//import { CreateOrEditHopDongModalComponent } from './create-or-edit-hopDong-modal.component';
import { ViewHopDongModalComponent } from './view-hopDong-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';
import $ from 'jquery';
import { formatDate } from '@angular/common';
declare const exportHTML: any;

@Component({
  templateUrl: './hopDongs.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class HopDongsComponent extends AppComponentBase implements OnInit {
  @ViewChild('viewHopDongmodal.component', { static: true }) viewHopDongModal: ViewHopDongModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  hoTenNhanVienFilter = '';
  viTriCongViecCodeFilter = '';
  maxNgayKyFilter: moment.Moment;
  minNgayKyFilter: moment.Moment;
  maxDonViCongTacIDFilter: number;
  maxDonViCongTacIDFilterEmpty: number;
  minDonViCongTacIDFilter: number;
  minDonViCongTacIDFilterEmpty: number;
  tenHopDongFilter = '';
  loaiHopDongCodeFilter = '';
  hinhThucLamViecCodeFilter = '';
  maxNgayCoHieuLucFilter: moment.Moment;
  minNgayCoHieuLucFilter: moment.Moment;
  maxNgayHetHanFilter: moment.Moment;
  minNgayHetHanFilter: moment.Moment;
  maxLuongCoBanFilter: number;
  maxLuongCoBanFilterEmpty: number;
  minLuongCoBanFilter: number;
  minLuongCoBanFilterEmpty: number;
  maxLuongDongBaoHiemFilter: number;
  maxLuongDongBaoHiemFilterEmpty: number;
  minLuongDongBaoHiemFilter: number;
  minLuongDongBaoHiemFilterEmpty: number;
  chucDanhFilter = '';
  trichYeuFilter = '';
  recorD_STATUSFilter = '';
  printUrl = "";
  maxMARKER_IDFilter: number;
  maxMARKER_IDFilterEmpty: number;
  minMARKER_IDFilter: number;
  minMARKER_IDFilterEmpty: number;
  autH_STATUSFilter = '';
  maxCHECKER_IDFilter: number;
  maxCHECKER_IDFilterEmpty: number;
  minCHECKER_IDFilter: number;
  minCHECKER_IDFilterEmpty: number;
  maxAPPROVE_DTFilter: moment.Moment;
  minAPPROVE_DTFilter: moment.Moment;
  thoiHanHopDongFilter = '';
  loaiHopDong: TruongGiaoDichDto[] = [];
  selected:any; 
  maHD:any; 
  tenLoaiHD :any; 
  truongGiaoDich: TruongGiaoDichDto[] = [];
  ghiChu :any;
  currentDate = new Date();
  code:any ;
  listDataHopDong:any;
  selectedRowsData: any[] = [];
  data:any;
  id:any;
   a = true ; 
   currentTime:any ; 
   uploadUrl:any; 
   value: any[] = [];
   uploadUrlImage:any;

  constructor(
    injector: Injector,
    private _hopDongsServiceProxy: HopDongsServiceProxy,
    private _notifyService: NotifyService,
    private router: Router,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
    this.uploadUrlImage = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_fileImage';
  }
  editHopDong() {
    this.router.navigate(['/app/main/qlns/hopDong/edit/'+ this.id]);
    
  }
  ngOnInit(): void {
    this.getHopDongs();
  }

  uploadImage() {
    const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
    // this.value.forEach((x) => {
    //     this.profilePicture = AppConsts.remoteServiceBaseUrl + "\\" + cValue + "\\" + x.name;
    // });
    this.value.length = 0;
}

  exportHTML() {
    let dataEXport = this.data;
    console.log(this.data)
  //  this._hopDongsServiceProxy.getInFoLHD(this.data.hopDong.loaiHopDongCode).subscribe(res =>{
  //    this.loaiHopDong = res;
  //    console.log( this.loaiHopDong) ;
  //      this.tenLoaiHD = this.loaiHopDong[0].value ; 
  //   this.ghiChu = this.loaiHopDong[0].ghiChu ; 
  
   this.code= this.data.hopDong.recorD_STATUS;
    this.printUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/GetWordFile?hopDongId='+this.data.hopDong.id+'&&path='+this.code+' ';
    console.log( this.printUrl) ;
    $.ajax({

      url:this.printUrl ,
        method: 'POST',
        xhrFields: {
            responseType: 'blob'
        },

        success: function (data1) {
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data1);
        
            a.href = url;
            a.download =  dataEXport.hopDong.loaiHopDongCode+".docx";
            document.body.append(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
    });

    
  }

  
  getHopDongs() {

    this._hopDongsServiceProxy.getAll(
      this.filterText,
      this.hoTenNhanVienFilter,
      this.viTriCongViecCodeFilter,
      this.maxNgayKyFilter,
      this.minNgayKyFilter,
      this.maxDonViCongTacIDFilter == null ? this.maxDonViCongTacIDFilterEmpty : this.maxDonViCongTacIDFilter,
      this.minDonViCongTacIDFilter == null ? this.minDonViCongTacIDFilterEmpty : this.minDonViCongTacIDFilter,
      this.tenHopDongFilter,
      this.loaiHopDongCodeFilter,
      this.hinhThucLamViecCodeFilter,
      this.maxNgayCoHieuLucFilter,
      this.minNgayCoHieuLucFilter,
      this.maxNgayHetHanFilter,
      this.minNgayHetHanFilter,
      this.maxLuongCoBanFilter == null ? this.maxLuongCoBanFilterEmpty : this.maxLuongCoBanFilter,
      this.minLuongCoBanFilter == null ? this.minLuongCoBanFilterEmpty : this.minLuongCoBanFilter,
      this.maxLuongDongBaoHiemFilter == null ? this.maxLuongDongBaoHiemFilterEmpty : this.maxLuongDongBaoHiemFilter,
      this.minLuongDongBaoHiemFilter == null ? this.minLuongDongBaoHiemFilterEmpty : this.minLuongDongBaoHiemFilter,
      this.chucDanhFilter,
      this.trichYeuFilter,
      this.recorD_STATUSFilter,
      this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
      this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
      this.autH_STATUSFilter,
      this.maxCHECKER_IDFilter == null ? this.maxCHECKER_IDFilterEmpty : this.maxCHECKER_IDFilter,
      this.minCHECKER_IDFilter == null ? this.minCHECKER_IDFilterEmpty : this.minCHECKER_IDFilter,
      this.maxAPPROVE_DTFilter,
      this.minAPPROVE_DTFilter,
      this.thoiHanHopDongFilter,
      "1",
      0,
      100000
    ).subscribe(result => {
      console.log(result)
      let count = 0;
      console.log(result);
      this.listDataHopDong = result.items; 
      for(var i = 0, len =  this.listDataHopDong.length; i < len; i++){
          this.listDataHopDong[i]["stt"] = ++count;      
      }

    });
  }

  startEdit(e:any )
    {
      
        this.data=e.data ;
        this.id= e.data.hopDong.id;
       this.a = !this.a;
       console.log(this.a)
    }

  createHopDong() {
    this.router.navigate(['/app/main/qlns/hopDong/create']);
  }

  deleteHopDong(): void {
  
      this.message.confirm(
        '', this.l('AreYouSure'),
        (isConfirmed) => {
          if (isConfirmed) {
            this._hopDongsServiceProxy.delete(this.id)
              .subscribe(() => {
              this.getHopDongs();
                this.notify.success(this.l('SuccessfullyDeleted'));
              });
          }
        }
      );
    
    
  }

  exportToExcel(): void {
    this._hopDongsServiceProxy.getHopDongsToExcel(
      this.filterText,
      this.hoTenNhanVienFilter,
      this.viTriCongViecCodeFilter,
      this.maxNgayKyFilter,
      this.minNgayKyFilter,
      this.maxDonViCongTacIDFilter == null ? this.maxDonViCongTacIDFilterEmpty : this.maxDonViCongTacIDFilter,
      this.minDonViCongTacIDFilter == null ? this.minDonViCongTacIDFilterEmpty : this.minDonViCongTacIDFilter,
      this.tenHopDongFilter,
      this.loaiHopDongCodeFilter,
      this.hinhThucLamViecCodeFilter,
      this.maxNgayCoHieuLucFilter,
      this.minNgayCoHieuLucFilter,
      this.maxNgayHetHanFilter,
      this.minNgayHetHanFilter,
      this.maxLuongCoBanFilter == null ? this.maxLuongCoBanFilterEmpty : this.maxLuongCoBanFilter,
      this.minLuongCoBanFilter == null ? this.minLuongCoBanFilterEmpty : this.minLuongCoBanFilter,
      this.maxLuongDongBaoHiemFilter == null ? this.maxLuongDongBaoHiemFilterEmpty : this.maxLuongDongBaoHiemFilter,
      this.minLuongDongBaoHiemFilter == null ? this.minLuongDongBaoHiemFilterEmpty : this.minLuongDongBaoHiemFilter,
      this.chucDanhFilter,
      this.trichYeuFilter,
      this.recorD_STATUSFilter,
      this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
      this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
      this.autH_STATUSFilter,
      this.maxCHECKER_IDFilter == null ? this.maxCHECKER_IDFilterEmpty : this.maxCHECKER_IDFilter,
      this.minCHECKER_IDFilter == null ? this.minCHECKER_IDFilterEmpty : this.minCHECKER_IDFilter,
      this.maxAPPROVE_DTFilter,
      this.minAPPROVE_DTFilter,
      this.thoiHanHopDongFilter,
    )
      .subscribe(result => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
