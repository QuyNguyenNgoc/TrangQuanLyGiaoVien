import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { HoSosServiceProxy, CreateOrEditHoSoDto, TruongGiaoDichDto, TinhThanhDto, NoiDaoTaoDto, NoiDaoTaosServiceProxy, TinhThanhsServiceProxy, OrganizationUnitDto, OrganizationUnitServiceProxy, ListResultDtoOfOrganizationUnitDto, DangKyKCBsServiceProxy, HopDongsServiceProxy, UngViensServiceProxy, TemplatesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { OrganizationUnitsTreeComponent, IOrganizationUnitsTreeComponentData } from '@app/admin/shared/organization-unit-tree.component';
import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { CustomInputDropdownComponent } from '@app/shared/common/customControl/custom-inputDropdown.component';
import { AppConsts } from '@shared/AppConsts';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';

@Component({
    selector: 'createHoSo',
    templateUrl: './create-hoSo.component.html'
})
export class CreateHoSoComponent extends AppComponentBase implements OnInit {


    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @ViewChild('organizationUnitTree', { static: false }) organizationUnitTree: OrganizationUnitsTreeComponent;
    @ViewChild('inputDropdown', { static: true }) private inputDropdown: CustomInputDropdownComponent;
    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
    active = false;
    currentTime: any;
    uploadUrl: any;
    saving = false;
    truongGiaoDich: TruongGiaoDichDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
    trangThai: TruongGiaoDichDto[] = [];
    kenhTuyenDung: TruongGiaoDichDto[] = [];
    gioiTinh: TruongGiaoDichDto[] = [];
    tinhTrangHonNhan: TruongGiaoDichDto[] = [];
    trinhDoDaoTao: TruongGiaoDichDto[] = [];
    xepLoaiHocLuc: TruongGiaoDichDto[] = [];
    tienDoTuyenDung: TruongGiaoDichDto[] = [];
    tinhThanh: TinhThanhDto[] = [];
    noiDaoTao: NoiDaoTaoDto[] = [];
    giongHKTT = false;
    allOrganizationUnits: OrganizationUnitDto[];
    memberedOrganizationUnits: string[];
    totalUnitCount = 0;
    treeData: any;
    uploadUrlImage: any;
    currentDate = new Date();
    rootUrl: string;
    profileImage: any;
    link: string;
    nameArr: any[] = [];
    dataDisplay = [];
    idDV: any;
    pathFileTemplate: any;
    value: any[] = [];
    cValue: any;
    name: any;
    selectedRows = [];
    popupVisible = false;
    valueImage: any;
    noiDangKy: any;
    hopDong: any;
    tepDinhKemSave = '';
    angular: any;
    publisherPopupVisible = false;
    tinhThanhIdSelected: number;
    noiDaoTaoIdSelected: number;
    defaultCbbOption: TruongGiaoDichDto[] = [];
    hoSo: CreateOrEditHoSoDto = new CreateOrEditHoSoDto();
    yearArr : any[];
    ngaySinh: Date;
    ngayCap: Date;
    // ngay_Sinh : any;
    ngayHetHan: Date;
    list_year:any[] ;
    ngayTapSu: Date;
    ngayThuViec: Date;
    ngayChinhThuc: Date;
    ngayThamGiaBH: Date;
    ngayHetHanBHYT: Date;
    ngay_Sinh :Date ; 
    approvE_DT: Date;
    dataRowDetail:any ; 
    nameKCB: any; 
    idKCB: any;
    popupVisibleKCB = false ;
    popupVisibleLHD= false ;
    loaiHopDong:any; 
    congTY:any; 
    selected:any;
    linkHD:any;
    nameHopDong:any; 
    idHopDong:any; 

    constructor(
        injector: Injector,
        private _arrayToTreeConverterService: ArrayToTreeConverterService,
        private _noiDaoTaosServiceProxy: NoiDaoTaosServiceProxy,
        private _tinhThanhsServiceProxy: TinhThanhsServiceProxy,
        private _hopDongsServiceProxy: HopDongsServiceProxy,
        private _templatesServiceProxy: TemplatesServiceProxy,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private _dangKyKCBsServiceProxy: DangKyKCBsServiceProxy,
        private router: Router,
        private _organizationUnitService: OrganizationUnitServiceProxy,
        private _hoSosServiceProxy: HoSosServiceProxy
    ) {
        super(injector);
        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
        this.uploadUrlImage = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_fileImage';
    }

    ngOnInit(): void {
        this.getTreeDataFromServer();
        this.show();
        // var start = 1900;
        // var end = new Date().getFullYear();
        // var options = "";
        // for(var year = start ; year <=end; year++){
        //   options += year +";";
        // }
        // console.log(options)
        // this.list_year = options.split(';');
        // console.log(this.list_year)
        // this.list_year.forEach((ele) =>{        
        //     this.yearArr.push({nam: ele});  
        // });
     
    
        //  console.log(this.yearArr)

    }

    chonThongTinKCB()
    {
        this.popupVisibleKCB = true ;
    }

    chonDV() {
        this.popupVisible = true;
    }

    uploadImage() {
        const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        this.value.forEach((x) => {
            this.profilePicture = AppConsts.remoteServiceBaseUrl + "\\" + cValue + "\\" + x.name;
        });
        this.value.length = 0;
    }

    onValueChanged (e) {
        console.log(e)
          this.selected =  e.value ; 
        //   this.loaiHopDong = this.truongGiaoDich.filter(x => x.ghiChu ==  this.selected);
        //   console.log( this.loaiHopDong)
          this._templatesServiceProxy.getListTemplate( this.selected).subscribe(res =>{

            this.loaiHopDong= res;
            let count = 0;
        
            for(var i = 0, len =  this.loaiHopDong.length; i < len; i++){
                this.loaiHopDong[i]["stt"] = ++count;      
                console.log( this.loaiHopDong)
            }
          })
      }

    chonLoaiHopDong(e:any)
    {
        console.log(e)
        this.hoSo.loaiHopDongID = e.data.id;
        this.idHopDong = e.data.id;
        this.nameHopDong = e.data.tenTemplate;
        this.linkHD= e.data.linkTemplate;
        this.popupVisibleLHD = false;
    }
    chonLoaiHD()
    {
        this.popupVisibleLHD =true ;
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

    nodeSelect(event: any) {
        console.log(event);
    }
    // chức năng  xóa file
    xoaFile(e: any) {

        console.log(this.selectedRows)
        this.selectedRows.splice(this.selectedRows.indexOf(e.row.data.tepDinhKem), 1);
        console.log(this.selectedRows)
        this.tepDinhKemSave = this.selectedRows.map(x => { return x.tepDinhKem.toString() }).join(';');

    }
    // chức năng xem file chi tiết
    showDetail(e: any) {
        console.log(e)
        this.rootUrl = AppConsts.remoteServiceBaseUrl;
        this.link = this.rootUrl + "/" + e.data.tepDinhKem;
        console.log(this.link)
        window.open(this.link, '_blank');
    }

    getIdSelected(event: any) {
        console.log(event);
        this.hoSo.donViCongTacID = Number.parseInt(event[0].id);
        this.idDV = this.hoSo.donViCongTacID;
        this.hoSo.donViCongTacName = event[0].name;

        console.log(this.hoSo.donViCongTacID)
        console.log(name)
    }
    showPublisherPopup() {
        this.publisherPopupVisible = true;
    }

    private getTreeDataFromServer(): void {
        let self = this;
        this._organizationUnitService.getOrganizationUnits().subscribe((result: ListResultDtoOfOrganizationUnitDto) => {
            this.totalUnitCount = result.items.length;
            console.log(result)
            this.treeData = this._arrayToTreeConverterService.createTree(result.items,
                'parentId',
                'id',
                null,
                'children',
                [
                    {
                        target: 'label',
                        targetFunction(item) {
                            return item.displayName;
                        }
                    }, {
                        target: 'expandedIcon',
                        value: 'fa fa-folder-open m--font-warning'
                    },
                    {
                        target: 'collapsedIcon',
                        value: 'fa fa-building-o m--font-warning'
                    },
                    {
                        target: 'selectable',
                        value: true
                    },
                    {
                        target: 'memberCount',
                        targetFunction(item) {
                            return item.memberCount;
                        }
                    },
                    {
                        target: 'roleCount',
                        targetFunction(item) {
                            return item.roleCount;
                        }
                    }
                ]);
            console.log(this.treeData);
        });
    }
    startEdit(e:any ) {
      
        this.dataRowDetail = e.data ;
        this.hoSo.maSoNoiKCB =  this.dataRowDetail.maNoiKCB;
        this.hoSo.noiDangKyKCBID = this.dataRowDetail.id;
        this.idKCB = this.dataRowDetail.id;
        this.nameKCB = this.dataRowDetail.tenNoiKCB;
        this.hoSo.maTinhCap = this.dataRowDetail.tinhThanhID;
        this.popupVisibleKCB = false ;
    }

    onChangeGiongHKTTCheckBox() {
        if (!this.giongHKTT)
            return false;
        this.hoSo.diaChiHN = this.hoSo.diaChiHKTT;
        this.hoSo.tinhThanhIDHN = this.hoSo.tinhThanhIDHKTT;
        this.hoSo.quocGiaHN = this.hoSo.quocGiaHKTT;
    }
    troVe()
    {
        this.router.navigate(['/app/main/qlns/hoSos']);
    }
  

    show(): void {
        // this.ngaySinh = null;
        // this.ngayCap = null;
        // this.ngayHetHan = null;
        // this.ngayTapSu = null;
        // this.ngayThuViec = null;
        // this.ngayChinhThuc = null;
        // this.ngayThamGiaBH = null;
        // this.ngayHetHanBHYT = null;
        // this.approvE_DT = null;
        this._noiDaoTaosServiceProxy.getAllNoiDaoTao().subscribe((res) => {
            this.noiDaoTao = res;
        });
        this._tinhThanhsServiceProxy.getAllTinhThanh().subscribe((res) => {
            this.tinhThanh = res;
        });
        this._dangKyKCBsServiceProxy.getAllNoiDangKy().subscribe((res) => {
            this.noiDangKy = res;
            console.log(this.noiDangKy)
        });
        // this._hopDongsServiceProxy.getAllHopDong().subscribe((res) => {
        //     this.hopDong = res;
        // });
        this.getAllTruongGiaoDich().subscribe((res) => {
            this.truongGiaoDich = res;
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');

            console.log(this.viTriCongViec)
            this.hopDong= this.truongGiaoDich.filter(x => x.code == 'LHD');
            this.congTY= this.truongGiaoDich.filter(x => x.code == 'CT'); 
            console.log( this.hopDong)
            this.trangThai = this.truongGiaoDich.filter(x => x.code == 'TRTH');
            this.kenhTuyenDung = this.truongGiaoDich.filter(x => x.code == 'KTD');
            this.gioiTinh = this.truongGiaoDich.filter(x => x.code == 'GT');
            this.tinhTrangHonNhan = this.truongGiaoDich.filter(x => x.code == 'TTHN');
            this.trinhDoDaoTao = this.truongGiaoDich.filter(x => x.code == 'TDDT');
            this.xepLoaiHocLuc = this.truongGiaoDich.filter(x => x.code == 'XLHL');
            this.tienDoTuyenDung = this.truongGiaoDich.filter(x => x.code == 'TDTD');
            this.defaultCbbOption = this.truongGiaoDich.filter(x => x.setDefault == true);
            console.log(this.defaultCbbOption);
            this.hoSo = new CreateOrEditHoSoDto();
            this.initUngvien();
        });


    }

    initUngvien() {

        this.hoSo.gioiTinhCode = this.defaultCbbOption.find(x => x.code == 'GT') ? this.defaultCbbOption.find(x => x.code == 'GT').cdName : '';
        this.hoSo.tinhThanhID = this.tinhThanhIdSelected;
        this.hoSo.tinhTrangHonNhanCode = this.defaultCbbOption.find(x => x.code == 'TTHN') ? this.defaultCbbOption.find(x => x.code == 'TTHN').cdName : '';
        this.hoSo.trinhDoVanHoa = '12/12';
        this.hoSo.noiDaoTaoID = this.noiDaoTaoIdSelected;
        this.hoSo.trinhDoDaoTaoCode = this.defaultCbbOption.find(x => x.code == 'TDDT') ? this.defaultCbbOption.find(x => x.code == 'TDDT').cdName : '';

    }
    save(): void {
        this.saving = true;
        this.hoSo.recorD_STATUS =this.linkHD;
        this.hoSo.anhDaiDien = this.profilePicture;
        this.hoSo.donViCongTacID = this.idDV;
        this.hoSo.tepDinhKem = this.tepDinhKemSave;
        this.hoSo.loaiHopDongID= this.idHopDong;
        this.hoSo.noiDangKyKCBID =this.idKCB ;

        if(!this.hoSo.hoVaTen)
        {
            this.message.warn("Họ tên không được bỏ trống!");
            return;
        }
        if(!this.hoSo.soCMND)
        {
            this.message.warn("CMND không được bỏ trống!");
            return;
        }
        if(!this.hoSo.donViCongTacID)
        {
            this.message.warn("Đơn vị công tác không được bỏ trống!");
            return;
        }
        if(!this.hoSo.viTriCongViecCode)
        
        {
            this.message.warn("Vị trí công việc không được bỏ trống!");
            return;
        }
        if(!this.hoSo.gioiTinhCode)
        {
            this.message.warn("Giới tính không được bỏ trống!");
            return;
        }
        if(!this.hoSo.quocTich)
        {
            this.message.warn("Quốc tịch không được bỏ trống!");
            return;
        }
        if(!this.hoSo.danToc)
        {
            this.message.warn("Dân tộc không được bỏ trống!");
            return;
        }
        if(!this.ngaySinh)
        {
            this.message.warn("Ngày sinh không được bỏ trống!");
            return;
        }
        if(!this.ngayCap)
        {
            this.message.warn("Ngày cấp CMND không được bỏ trống!");
            return;
        }

        if(!this.hoSo.nguyenQuan)
        {
            this.message.warn("Nguyên quán không được bỏ trống!");
            return;
        }
        if(!this.hoSo.diaChiHKTT)
        {
            this.message.warn("Địa chỉ HKTT không được bỏ trống!");
            return;
        }
        if(!this.hoSo.loaiHopDongID)
        {
            this.message.warn("Loại HD không được bỏ trống!");
            return;
        }
        if(!this.hoSo.tkNganHang)
        {
            this.message.warn("Tài khoản ngân hàng không được bỏ trống!");
            return;
        }
        if(!this.hoSo.luongCoBan)
        {
            this.message.warn("Lương cơ bản không được bỏ trống!");
            return;
        }
        if(!this.hoSo.nganHangCode)
        {
            this.message.warn("Ngân hàng không được bỏ trống!");
            return;
        }

      
        
        if (this.ngaySinh) {
      
                this.hoSo.ngaySinh = moment(this.ngaySinh);
         
        }
        else {
         
            this.hoSo.ngaySinh = null;
        }
        if (this.ngayCap) {
          
                this.hoSo.ngayCap = moment(this.ngayCap);
          
        }
        else {
            this.hoSo.ngayCap = null;
        }
        if (this.ngayHetHan) {
          
                this.hoSo.ngayHetHan = moment(this.ngayHetHan);
           
        }
        else {
            this.hoSo.ngayHetHan = null;
        }
        if (this.ngayTapSu) {
         
                this.hoSo.ngayTapSu = moment(this.ngayTapSu);
      
        }
        else {
            this.hoSo.ngayTapSu = null;
        }
        if (this.ngayThuViec) {
          
                this.hoSo.ngayThuViec = moment(this.ngayThuViec);
            
        }
        else {
            this.hoSo.ngayThuViec = null;
        }
        if (this.ngayChinhThuc) {

                this.hoSo.ngayChinhThuc = moment(this.ngayChinhThuc);
            
        }
        else {
            this.hoSo.ngayChinhThuc = null;
        }
        if (this.ngayThamGiaBH) {

                this.hoSo.ngayThamGiaBH = moment(this.ngayThamGiaBH);
            
        }
        else {
            this.hoSo.ngayThamGiaBH = null;
        }
        if (this.ngayHetHanBHYT) {

                this.hoSo.ngayHetHanBHYT = moment(this.ngayHetHanBHYT);
            
        }
        else {
            this.hoSo.ngayHetHanBHYT = null;
        }
        if (this.approvE_DT) {

                this.hoSo.approvE_DT = moment(this.approvE_DT);
            
        }
        else {
            this.hoSo.approvE_DT = null;
        }
        console.log(this.hoSo)
        this._hoSosServiceProxy.createOrEdit(this.hoSo)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.hoSo = new CreateOrEditHoSoDto();
                this.initUngvien();
                //this.close();
                this.modalSave.emit(null);
            });
    }


}
