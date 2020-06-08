import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HoSosServiceProxy, HoSoDto, UngViensServiceProxy, CreateOrEditHoSoDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
//import { CreateOrEditHoSoModalComponent } from './create-hoSo.component';
import { ViewHoSoModalComponent } from './view-hoSo-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/components/table/table';
import { Paginator } from 'primeng/components/paginator/paginator';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import { formatDate } from '@angular/common';
import { AppConsts } from '@shared/AppConsts';
import { DxTextBoxModule, DxFileUploaderModule, DxButtonModule, DxFileUploaderComponent } from 'devextreme-angular';

@Component({
  templateUrl: './hoSos.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class HoSosComponent extends AppComponentBase implements OnInit {

  //@ViewChild('createOrEditHoSoModal', { static: true }) createOrEditHoSoModal: CreateOrEditHoSoModalComponent;
  @ViewChild('viewHoSomodal.component', { static: true }) viewHoSoModal: ViewHoSoModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('fileUploader1', { static: false }) fileUploader1: DxFileUploaderComponent;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  maNhanVienFilter = '';
  hoVaTenFilter = '';
  anhDaiDienFilter = '';
  gioiTinhCodeFilter = '';
  maxNgaySinhFilter: moment.Moment;
  minNgaySinhFilter: moment.Moment;
  mstCaNhanFilter = '';
  maxDonViCongTacIDFilter: number;
  maxDonViCongTacIDFilterEmpty: number;
  minDonViCongTacIDFilter: number;
  minDonViCongTacIDFilterEmpty: number;
  viTriCongViecCodeFilter = '';
  danTocFilter = '';
  tonGiaoFilter = '';
  quocTichFilter = '';
  soCMNDFilter = '';
  loaiHopDongIDFilter: number;
  maxNgayCapFilter: moment.Moment;
  minNgayCapFilter: moment.Moment;
  noiCapFilter = '';
  maxNgayHetHanFilter: moment.Moment;
  minNgayHetHanFilter: moment.Moment;
  trinhDoVanHoaFilter = '';
  trinhDoDaoTaoCodeFilter = '';
  noiDaoTaoIDFilter: number;
  maxNoiDaoTaoCodeFilter: number;
  maxNoiDaoTaoCodeFilterEmpty: number;
  minNoiDaoTaoCodeFilter: number;
  // noiDaoTaoCodeFilter = '';
  khoaFilter = '';
  chuyenNganhFilter = '';
  maxNamTotNghiepFilter: number;
  maxNamTotNghiepFilterEmpty: number;
  minNamTotNghiepFilter: number;
  minNamTotNghiepFilterEmpty: number;
  xepLoaiCodeFilter = '';
  tinhTrangHonNhanCodeFilter = '';
  tepDinhKemFilter = '';
  dtDiDongFilter = '';
  dtCoQuanFilter = '';
  dtNhaRiengFilter = '';
  dtKhacFilter = '';
  emailCaNhanFilter = '';
  emailCoQuanFilter = '';
  emailKhacFilter = '';
  nguyenQuanFilter = '';
  maxTinhThanhIDFilter: number;
  maxTinhThanhIDFilterEmpty: number;
  minTinhThanhIDFilter: number;
  minTinhThanhIDFilterEmpty: number;
  noiSinhFilter = '';
  skypeFilter = '';
  facebookFilter = '';
  quocGiaHKTTFilter = '';
  maxTinhThanhIDHKTTFilter: number;
  maxTinhThanhIDHKTTFilterEmpty: number;
  minTinhThanhIDHKTTFilter: number;
  minTinhThanhIDHKTTFilterEmpty: number;
  diaChiHKTTFilter = '';
  soSoHoKhauFilter = '';
  maSoHoGiaDinhFilter = '';
  laChuHoFilter = -1;
  quocGiaHNFilter = '';
  maxTinhThanhIDHNFilter: number;
  maxTinhThanhIDHNFilterEmpty: number;
  minTinhThanhIDHNFilter: number;
  minTinhThanhIDHNFilterEmpty: number;
  diaChiHNFilter = '';
  hoVaTenLHKCFilter = '';
  quanHeLHKCFilter = '';
  dtDiDongLHKCFilter = '';
  dtNhaRiengLHKCFilter = '';
  emailLHKCFilter = '';
  diaChiLHKCFilter = '';
  maChamCongFilter = '';
  chucDanhFilter = '';
  capFilter = '';
  bacFilter = '';
  trangThaiLamViecCodeFilter = '';
  quanLyTrucTiepFilter = '';
  quanLyGianTiepFilter = '';
  diaDiemLamViecCodeFilter = '';
  minLoaiHopDongCodeFilterEmpty;
  soSoQLLaoDongFilter = '';
  loaiHopDongCodeID: number;
  maxloaiHopDongCodeFilter: number;
  minloaiHopDongCodeFilter: number;
  maxloaiHopDongCodeFilterEmpty: number;
  minNoiDaoTaoCodeFilterEmpty: number;
  // loaiHopDongCodeFilter = '';
  maxNgayTapSuFilter: moment.Moment;
  minNgayTapSuFilter: moment.Moment;
  maxNgayThuViecFilter: moment.Moment;
  minNgayThuViecFilter: moment.Moment;
  maxNgayChinhThucFilter: moment.Moment;
  minNgayChinhThucFilter: moment.Moment;
  maxSoNgayPhepFilter: number;
  maxSoNgayPhepFilterEmpty: number;
  minSoNgayPhepFilter: number;
  minSoNgayPhepFilterEmpty: number;
  bacLuongCodeFilter = '';
  maxLuongCoBanFilter: number;
  maxLuongCoBanFilterEmpty: number;
  minLuongCoBanFilter: number;
  maxLoaiHopDongCodeFilterEmpty;
  minLuongCoBanFilterEmpty: number;
  maxLuongDongBHFilter: number;
  maxLuongDongBHFilterEmpty: number;
  minLuongDongBHFilter: number;
  minLuongDongBHFilterEmpty: number;
  maxSoCongChuanFilter: number;
  maxSoCongChuanFilterEmpty: number;
  minSoCongChuanFilter: number;
  minSoCongChuanFilterEmpty: number;
  donViSoCongChuanCodeFilter = '';
  tkNganHangFilter = '';
  nganHangCodeFilter = '';
  thamGiaCongDoanFilter = -1;
  maxNgayThamGiaBHFilter: moment.Moment;
  minNgayThamGiaBHFilter: moment.Moment;
  maxTyLeDongBHFilter: number;
  maxTyLeDongBHFilterEmpty: number;
  minTyLeDongBHFilter: number;
  minTyLeDongBHFilterEmpty: number;
  soSoBHXHFilter = '';
  maSoBHXHFilter = '';
  maTinhCapFilter = '';
  soTheBHYTFilter = '';
  maxNgayHetHanBHYTFilter: moment.Moment;
  minNgayHetHanBHYTFilter: moment.Moment;
  maxNoiDangKyKCBIDFilter: number;
  maxNoiDangKyKCBIDFilterEmpty: number;
  minNoiDangKyKCBIDFilter: number;
  minNoiDangKyKCBIDFilterEmpty: number;
  maSoNoiKCBFilter = '';
  autH_STATUSFilter = '';
  recorD_STATUSFilter = '';
  maxLoaiHopDongCodeFilter;
  hoSo: CreateOrEditHoSoDto = new CreateOrEditHoSoDto();
  maxMARKER_IDFilter: number;
  maxMARKER_IDFilterEmpty: number;
  minMARKER_IDFilter: number;
  minMARKER_IDFilterEmpty: number;
  maxCHECKER_IDFilter: number;
  maxCHECKER_IDFilterEmpty: number;
  minCHECKER_IDFilter: number;
  minCHECKER_IDFilterEmpty: number;
  maxAPPROVE_DTFilter: moment.Moment;
  minAPPROVE_DTFilter: moment.Moment;
  minLoaiHopDongCodeFilter;
  name: any;
  listDataHoSo:any[] = [];
  selectedRowsData: any[] = [];
  data: any;
  a = true;
  id: any;
  currentDate = new Date();
  currentTime: any;
  value: any[] = [];
  dataDisplay: any;
  uploadUrl: any;
  rootUrl: any;
  constructor(
    injector: Injector,
    private _hoSosServiceProxy: HoSosServiceProxy,
    private _ungViensServiceProxy: UngViensServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private router: Router,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.rootUrl = AppConsts.remoteServiceBaseUrl;
    this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();

    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
  }
  ngOnInit(): void {
 
 this.getHoSos();

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
  startEdit(e: any) {
    this.data = e.data;
    console.log(this.data)
    this.id = e.data.id;
    this.a = !this.a;
    console.log(this.id)
  }

  createHoSo(): void {
    this.router.navigate(['/app/main/qlns/hoSos/create']);
  }

  // getHoSos() {

  //   this._hoSosServiceProxy.getAll(
  //     this.filterText,
  //     this.maNhanVienFilter,
  //     this.hoVaTenFilter,
  //     this.anhDaiDienFilter,
  //     this.gioiTinhCodeFilter,
  //     this.maxNgaySinhFilter,
  //     this.minNgaySinhFilter,
  //     this.mstCaNhanFilter,
  //     this.maxDonViCongTacIDFilter == null ? this.maxDonViCongTacIDFilterEmpty : this.maxDonViCongTacIDFilter,
  //     this.minDonViCongTacIDFilter == null ? this.minDonViCongTacIDFilterEmpty : this.minDonViCongTacIDFilter,
  //     this.viTriCongViecCodeFilter,
  //     this.danTocFilter,
  //     this.tonGiaoFilter,
  //     this.quocTichFilter,
  //     this.soCMNDFilter,
  //     this.maxNgayCapFilter,
  //     this.minNgayCapFilter,
  //     this.noiCapFilter,
  //     this.maxNgayHetHanFilter,
  //     this.minNgayHetHanFilter,
  //     this.trinhDoVanHoaFilter,
  //     this.trinhDoDaoTaoCodeFilter,
  //     this.maxNoiDaoTaoCodeFilter == null ? this.maxNoiDaoTaoCodeFilterEmpty : this.maxNoiDaoTaoCodeFilter,
  //     this.minNoiDaoTaoCodeFilter == null ? this.minNoiDaoTaoCodeFilterEmpty : this.minNoiDaoTaoCodeFilter,
  //     // this.noiDaoTaoCodeFilter,
  //     this.khoaFilter,
  //     this.chuyenNganhFilter,
  //     this.maxNamTotNghiepFilter == null ? this.maxNamTotNghiepFilterEmpty : this.maxNamTotNghiepFilter,
  //     this.minNamTotNghiepFilter == null ? this.minNamTotNghiepFilterEmpty : this.minNamTotNghiepFilter,
  //     this.xepLoaiCodeFilter,
  //     this.tinhTrangHonNhanCodeFilter,
  //     this.tepDinhKemFilter,
  //     this.dtDiDongFilter,
  //     this.dtCoQuanFilter,
  //     this.dtNhaRiengFilter,
  //     this.dtKhacFilter,
  //     this.emailCaNhanFilter,
  //     this.emailCoQuanFilter,
  //     this.emailKhacFilter,
  //     this.nguyenQuanFilter,
  //     this.maxTinhThanhIDFilter == null ? this.maxTinhThanhIDFilterEmpty : this.maxTinhThanhIDFilter,
  //     this.minTinhThanhIDFilter == null ? this.minTinhThanhIDFilterEmpty : this.minTinhThanhIDFilter,
  //     this.noiSinhFilter,
  //     this.skypeFilter,
  //     this.facebookFilter,
  //     this.quocGiaHKTTFilter,
  //     this.maxTinhThanhIDHKTTFilter == null ? this.maxTinhThanhIDHKTTFilterEmpty : this.maxTinhThanhIDHKTTFilter,
  //     this.minTinhThanhIDHKTTFilter == null ? this.minTinhThanhIDHKTTFilterEmpty : this.minTinhThanhIDHKTTFilter,
  //     this.diaChiHKTTFilter,
  //     this.soSoHoKhauFilter,
  //     this.maSoHoGiaDinhFilter,
  //     this.laChuHoFilter,
  //     this.quocGiaHNFilter,
  //     this.maxTinhThanhIDHNFilter == null ? this.maxTinhThanhIDHNFilterEmpty : this.maxTinhThanhIDHNFilter,
  //     this.minTinhThanhIDHNFilter == null ? this.minTinhThanhIDHNFilterEmpty : this.minTinhThanhIDHNFilter,
  //     this.diaChiHNFilter,
  //     this.hoVaTenLHKCFilter,
  //     this.quanHeLHKCFilter,
  //     this.dtDiDongLHKCFilter,
  //     this.dtNhaRiengLHKCFilter,
  //     this.emailLHKCFilter,
  //     this.diaChiLHKCFilter,
  //     this.maChamCongFilter,
  //     this.chucDanhFilter,
  //     this.capFilter,
  //     this.bacFilter,
  //     this.trangThaiLamViecCodeFilter,
  //     this.quanLyTrucTiepFilter,
  //     this.quanLyGianTiepFilter,
  //     this.diaDiemLamViecCodeFilter,
  //     this.soSoQLLaoDongFilter,
  //     this.maxLoaiHopDongCodeFilter == null ? this.maxLoaiHopDongCodeFilterEmpty : this.maxLoaiHopDongCodeFilter,
  //     this.minLoaiHopDongCodeFilter == null ? this.minLoaiHopDongCodeFilterEmpty : this.minLoaiHopDongCodeFilter,
  //     // this.loaiHopDongCodeFilter,
  //     this.maxNgayTapSuFilter,
  //     this.minNgayTapSuFilter,
  //     this.maxNgayThuViecFilter,
  //     this.minNgayThuViecFilter,
  //     this.maxNgayChinhThucFilter,
  //     this.minNgayChinhThucFilter,
  //     this.maxSoNgayPhepFilter == null ? this.maxSoNgayPhepFilterEmpty : this.maxSoNgayPhepFilter,
  //     this.minSoNgayPhepFilter == null ? this.minSoNgayPhepFilterEmpty : this.minSoNgayPhepFilter,
  //     this.bacLuongCodeFilter,
  //     this.maxLuongCoBanFilter == null ? this.maxLuongCoBanFilterEmpty : this.maxLuongCoBanFilter,
  //     this.minLuongCoBanFilter == null ? this.minLuongCoBanFilterEmpty : this.minLuongCoBanFilter,
  //     this.maxLuongDongBHFilter == null ? this.maxLuongDongBHFilterEmpty : this.maxLuongDongBHFilter,
  //     this.minLuongDongBHFilter == null ? this.minLuongDongBHFilterEmpty : this.minLuongDongBHFilter,
  //     this.maxSoCongChuanFilter == null ? this.maxSoCongChuanFilterEmpty : this.maxSoCongChuanFilter,
  //     this.minSoCongChuanFilter == null ? this.minSoCongChuanFilterEmpty : this.minSoCongChuanFilter,
  //     this.donViSoCongChuanCodeFilter,
  //     this.tkNganHangFilter,
  //     this.nganHangCodeFilter,
  //     this.thamGiaCongDoanFilter,
  //     this.maxNgayThamGiaBHFilter,
  //     this.minNgayThamGiaBHFilter,
  //     this.maxTyLeDongBHFilter == null ? this.maxTyLeDongBHFilterEmpty : this.maxTyLeDongBHFilter,
  //     this.minTyLeDongBHFilter == null ? this.minTyLeDongBHFilterEmpty : this.minTyLeDongBHFilter,
  //     this.soSoBHXHFilter,
  //     this.maSoBHXHFilter,
  //     this.maTinhCapFilter,
  //     this.soTheBHYTFilter,
  //     this.maxNgayHetHanBHYTFilter,
  //     this.minNgayHetHanBHYTFilter,
  //     this.maxNoiDangKyKCBIDFilter == null ? this.maxNoiDangKyKCBIDFilterEmpty : this.maxNoiDangKyKCBIDFilter,
  //     this.minNoiDangKyKCBIDFilter == null ? this.minNoiDangKyKCBIDFilterEmpty : this.minNoiDangKyKCBIDFilter,
  //     this.maSoNoiKCBFilter,
  //     this.autH_STATUSFilter,
  //     this.recorD_STATUSFilter,
  //     this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
  //     this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
  //     this.maxCHECKER_IDFilter == null ? this.maxCHECKER_IDFilterEmpty : this.maxCHECKER_IDFilter,
  //     this.minCHECKER_IDFilter == null ? this.minCHECKER_IDFilterEmpty : this.minCHECKER_IDFilter,
  //     this.maxAPPROVE_DTFilter,
  //     this.minAPPROVE_DTFilter,
  //     "1",
  //     0,
  //     100000
  //   ).subscribe(result => {
  //     console.log(result)
  //     let count = 0;
  //     console.log(result);
  //     this.listDataHoSo = result.items;
  //     for (var i = 0, len = this.listDataHoSo.length; i < len; i++) {
  //       this.listDataHoSo[i]["stt"] = ++count;
  //     }
  //     console.log(this.listDataHoSo);
  //   });
  // }

  dowloadTemplate() {

    this.rootUrl = AppConsts.appBaseUrl;
    let link = this.rootUrl + '/assets/sampleFiles/DanHSachNhanVien.xlsx';
    console.log(link)
    window.open(link, '_blank');
  }

  importToExcel() {

    const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
    console.log(this.value)
    this.value.forEach((ele) => {
      this.dataDisplay = ele.name
      this._hoSosServiceProxy.importToExcel(this.currentTime, this.dataDisplay).subscribe(res => {
        if (res == null) {
          this.message.success("Import thành công");
          this.getHoSos();
        }
        else {
          this.message.info("Nhập liệu chưa đúng");
        }

      })
      this.value.length = 0
    });

  }
  editHoSoModal() {

    this.router.navigate(['/app/main/qlns/hoSos/edit/' + this.id]);
  }
  deleteHoSo(): void {
    this.message.confirm(
      '', this.l('AreYouSure'),
      (isConfirmed) => {
        if (isConfirmed) {
          this._hoSosServiceProxy.delete(this.id)
            .subscribe(() => {
              this.getHoSos();
              this.notify.success(this.l('SuccessfullyDeleted'));
            });
        }
      }
    );
  }

  importHoSo() {
    const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
    this.value.forEach((ele) => {
      this.dataDisplay = ele.name
      this._hoSosServiceProxy.importToExcel(this.currentTime, this.dataDisplay).subscribe(res => {
        if (res == null) {
          this.message.success("Import thành công");
          this.getHoSos();
        }
        else {
          this.message.info("Nhập liệu chưa đúng");
        }

      })
      this.value.length = 0
    });

  }

  exportToExcel(): void {
    this._hoSosServiceProxy.getHoSosToExcel(
      this.filterText,
      this.maNhanVienFilter,
      this.hoVaTenFilter,
      this.anhDaiDienFilter,
      this.gioiTinhCodeFilter,
      this.maxNgaySinhFilter,
      this.minNgaySinhFilter,
      this.mstCaNhanFilter,
      this.maxDonViCongTacIDFilter == null ? this.maxDonViCongTacIDFilterEmpty : this.maxDonViCongTacIDFilter,
      this.minDonViCongTacIDFilter == null ? this.minDonViCongTacIDFilterEmpty : this.minDonViCongTacIDFilter,
      this.viTriCongViecCodeFilter,
      this.danTocFilter,
      this.tonGiaoFilter,
      this.quocTichFilter,
      this.soCMNDFilter,
      this.maxNgayCapFilter,
      this.minNgayCapFilter,
      this.noiCapFilter,
      this.maxNgayHetHanFilter,
      this.minNgayHetHanFilter,
      this.trinhDoVanHoaFilter,
      this.trinhDoDaoTaoCodeFilter,
      this.maxNoiDaoTaoCodeFilter == null ? this.maxNoiDaoTaoCodeFilterEmpty : this.maxNoiDaoTaoCodeFilter,
      this.minNoiDaoTaoCodeFilter == null ? this.minNoiDaoTaoCodeFilterEmpty : this.minNoiDaoTaoCodeFilter,
      this.khoaFilter,
      this.chuyenNganhFilter,
      this.maxNamTotNghiepFilter == null ? this.maxNamTotNghiepFilterEmpty : this.maxNamTotNghiepFilter,
      this.minNamTotNghiepFilter == null ? this.minNamTotNghiepFilterEmpty : this.minNamTotNghiepFilter,
      this.xepLoaiCodeFilter,
      this.tinhTrangHonNhanCodeFilter,
      this.tepDinhKemFilter,
      this.dtDiDongFilter,
      this.dtCoQuanFilter,
      this.dtNhaRiengFilter,
      this.dtKhacFilter,
      this.emailCaNhanFilter,
      this.emailCoQuanFilter,
      this.emailKhacFilter,
      this.nguyenQuanFilter,
      this.maxTinhThanhIDFilter == null ? this.maxTinhThanhIDFilterEmpty : this.maxTinhThanhIDFilter,
      this.minTinhThanhIDFilter == null ? this.minTinhThanhIDFilterEmpty : this.minTinhThanhIDFilter,
      this.noiSinhFilter,
      this.skypeFilter,
      this.facebookFilter,
      this.quocGiaHKTTFilter,
      this.maxTinhThanhIDHKTTFilter == null ? this.maxTinhThanhIDHKTTFilterEmpty : this.maxTinhThanhIDHKTTFilter,
      this.minTinhThanhIDHKTTFilter == null ? this.minTinhThanhIDHKTTFilterEmpty : this.minTinhThanhIDHKTTFilter,
      this.diaChiHKTTFilter,
      this.soSoHoKhauFilter,
      this.maSoHoGiaDinhFilter,
      this.laChuHoFilter,
      this.quocGiaHNFilter,
      this.maxTinhThanhIDHNFilter == null ? this.maxTinhThanhIDHNFilterEmpty : this.maxTinhThanhIDHNFilter,
      this.minTinhThanhIDHNFilter == null ? this.minTinhThanhIDHNFilterEmpty : this.minTinhThanhIDHNFilter,
      this.diaChiHNFilter,
      this.hoVaTenLHKCFilter,
      this.quanHeLHKCFilter,
      this.dtDiDongLHKCFilter,
      this.dtNhaRiengLHKCFilter,
      this.emailLHKCFilter,
      this.diaChiLHKCFilter,
      this.maChamCongFilter,
      this.chucDanhFilter,
      this.capFilter,
      this.bacFilter,
      this.trangThaiLamViecCodeFilter,
      this.quanLyTrucTiepFilter,
      this.quanLyGianTiepFilter,
      this.diaDiemLamViecCodeFilter,
      this.soSoQLLaoDongFilter,
      this.maxLoaiHopDongCodeFilter == null ? this.maxLoaiHopDongCodeFilterEmpty : this.maxLoaiHopDongCodeFilter,
      this.minLoaiHopDongCodeFilter == null ? this.minLoaiHopDongCodeFilterEmpty : this.minLoaiHopDongCodeFilter,
      this.maxNgayTapSuFilter,
      this.minNgayTapSuFilter,
      this.maxNgayThuViecFilter,
      this.minNgayThuViecFilter,
      this.maxNgayChinhThucFilter,
      this.minNgayChinhThucFilter,
      this.maxSoNgayPhepFilter == null ? this.maxSoNgayPhepFilterEmpty : this.maxSoNgayPhepFilter,
      this.minSoNgayPhepFilter == null ? this.minSoNgayPhepFilterEmpty : this.minSoNgayPhepFilter,
      this.bacLuongCodeFilter,
      this.maxLuongCoBanFilter == null ? this.maxLuongCoBanFilterEmpty : this.maxLuongCoBanFilter,
      this.minLuongCoBanFilter == null ? this.minLuongCoBanFilterEmpty : this.minLuongCoBanFilter,
      this.maxLuongDongBHFilter == null ? this.maxLuongDongBHFilterEmpty : this.maxLuongDongBHFilter,
      this.minLuongDongBHFilter == null ? this.minLuongDongBHFilterEmpty : this.minLuongDongBHFilter,
      this.maxSoCongChuanFilter == null ? this.maxSoCongChuanFilterEmpty : this.maxSoCongChuanFilter,
      this.minSoCongChuanFilter == null ? this.minSoCongChuanFilterEmpty : this.minSoCongChuanFilter,
      this.donViSoCongChuanCodeFilter,
      this.tkNganHangFilter,
      this.nganHangCodeFilter,
      this.thamGiaCongDoanFilter,
      this.maxNgayThamGiaBHFilter,
      this.minNgayThamGiaBHFilter,
      this.maxTyLeDongBHFilter == null ? this.maxTyLeDongBHFilterEmpty : this.maxTyLeDongBHFilter,
      this.minTyLeDongBHFilter == null ? this.minTyLeDongBHFilterEmpty : this.minTyLeDongBHFilter,
      this.soSoBHXHFilter,
      this.maSoBHXHFilter,
      this.maTinhCapFilter,
      this.soTheBHYTFilter,
      this.maxNgayHetHanBHYTFilter,
      this.minNgayHetHanBHYTFilter,
      this.maxNoiDangKyKCBIDFilter == null ? this.maxNoiDangKyKCBIDFilterEmpty : this.maxNoiDangKyKCBIDFilter,
      this.minNoiDangKyKCBIDFilter == null ? this.minNoiDangKyKCBIDFilterEmpty : this.minNoiDangKyKCBIDFilter,
      this.maSoNoiKCBFilter,
      this.autH_STATUSFilter,
      this.recorD_STATUSFilter,
      this.maxMARKER_IDFilter == null ? this.maxMARKER_IDFilterEmpty : this.maxMARKER_IDFilter,
      this.minMARKER_IDFilter == null ? this.minMARKER_IDFilterEmpty : this.minMARKER_IDFilter,
      this.maxCHECKER_IDFilter == null ? this.maxCHECKER_IDFilterEmpty : this.maxCHECKER_IDFilter,
      this.minCHECKER_IDFilter == null ? this.minCHECKER_IDFilterEmpty : this.minCHECKER_IDFilter,
      this.maxAPPROVE_DTFilter,
      this.minAPPROVE_DTFilter,
    )
      .subscribe(result => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
