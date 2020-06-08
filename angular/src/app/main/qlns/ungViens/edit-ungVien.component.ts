import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef, Directive, Input, Sanitizer, SimpleChanges, SecurityContext } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';
import { UngViensServiceProxy, CreateOrEditUngVienDto, TruongGiaoDichDto, TinhThanhDto, NoiDaoTaoDto, NoiDaoTaosServiceProxy, TinhThanhsServiceProxy, LichSuLamViecDto, LichSuLamViecsServiceProxy, SessionServiceProxy, UngVienDto, TemplatesServiceProxy, TemplateDto, HostSettingsEditDto, ComboboxItemDto, SettingScopes, SendTestEmailInput, HostSettingsServiceProxy, ConfigEmailsServiceProxy, ConfigEmailDto, GetConfigEmailForViewDto, CreateOrEditLichSuLamViecDto } from '@shared/service-proxies/service-proxies'; import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { formatDate } from '@angular/common';
import { DxHtmlEditorModule, DxCheckBoxModule } from 'devextreme-angular';
import * as $ from 'jquery';
import 'jquery-ui/ui/widgets/dialog.js';
import { CKEditorModule } from 'ng2-ckeditor';


import { ToolbarModule } from 'primeng/primeng';
@Component({
    selector: 'editUngVien',
    templateUrl: './edit-ungVien.component.html'

})
export class EditUngVienComponent extends AppComponentBase implements OnInit {

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    maxLength = null;
    truongGiaoDich: TruongGiaoDichDto[] = [];
    viTriCongViec: TruongGiaoDichDto[] = [];
    trangThai: TruongGiaoDichDto[] = [];
    kenhTuyenDung: TruongGiaoDichDto[] = [];
    listComment: LichSuLamViecDto[] = [];
    tienDoTuyenDung: TruongGiaoDichDto[] = [];
    gioiTinh: TruongGiaoDichDto[] = [];
    chuDeGsoft: TemplateDto[] = [];
    chuDeGoB: TemplateDto[] = [];

    tinhTrangHonNhan: TruongGiaoDichDto[] = [];
    congty: TruongGiaoDichDto[] = [];
    trinhDoDaoTao: TruongGiaoDichDto[] = [];
    xepLoaiHocLuc: TruongGiaoDichDto[] = [];
    tinhThanh: TinhThanhDto[] = [];
    noiDaoTao: NoiDaoTaoDto[] = [];

    ungVien: CreateOrEditUngVienDto = new CreateOrEditUngVienDto();
    ngaySinh: Date;
    ngayCap: Date;
    approvE_DT: Date;
    ungVienId: number;
    data: any;
    num: string;
    rootUrl = AppConsts.remoteServiceBaseUrl;
    link: string;
    selectedRows = [];
    userId: number;
    currentDate = new Date();
    nameArr: any[] = [];
    filedinhkem: any;
    loading = false;
    hostSettings: HostSettingsEditDto;
    editions: ComboboxItemDto[] = undefined;
    testEmailAddress: string = undefined;
    showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
    defaultTimezoneScope: SettingScopes = SettingScopes.Application;
    i: number;
    currentTime: any;
    tenChuDe: any;
    uploadUrl: string;
    value: any[] = []; comment = '';
    currentUserId = this.getCurrentUser().id;
    currentUserFullName = this.getFullNameCurrentUser();
    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
    tepDinhKemSave = '';
    isMultiline: boolean = true;
    dataDisplay = [];
    valueContent: string;
    editorValueType: string;
    mailTo: string;
    mailFrom: string;
    subject: string;
    body: string;
    fileDinhkem: string;
    onValueTypeChanged({ addedItems }) {
        this.editorValueType = addedItems[0].text.toLowerCase();
    }

    valueChange(value) {
        this.valueContent = value;
    }
    chude: any;
    ungVienForCheckCMND: UngVienDto[] = [];
    chude_: TemplateDto[] = [];
    listYear: any;
    years: any;
    email :any; 
    static $this: any;
    // noiDungCD = 'Loading...'; 
    noiDungCD: any;
    tenFile: any;
    mail_From: any;
    config_Emnail: any;
    configEmnail: ConfigEmailDto;
    tenTruyCap: any;
    matKhau: any;
    tenCTY: any;
    congSMTP: any;
    diaChiIP: any;
    selected_Rows:any;
    id: any;
    lichsu: any;
    tepDinhKem_Save:any;
    constructor(
        injector: Injector,

        private elementRef: ElementRef,
        private sanitizer: Sanitizer,
        private _hostSettingService: HostSettingsServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _ungViensServiceProxy: UngViensServiceProxy,
        private router: Router,
        private _configEmailsServiceProxy: ConfigEmailsServiceProxy,
        private _templatesServiceProxy: TemplatesServiceProxy,
        private _noiDaoTaosServiceProxy: NoiDaoTaosServiceProxy,
        private _tinhThanhsServiceProxy: TinhThanhsServiceProxy,
        private _lichSuLamViecsServiceProxy: LichSuLamViecsServiceProxy
    ) {
        super(injector);

        this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/DemoUiComponents/Upload_file?currentTime=' + this.currentTime;
        (window.parent as any).closeCustomRoxy = this.closeCustomRoxy;
        (window.parent as any).onSelectCustomRoxy = this.onSelectCustomRoxy;
        EditUngVienComponent.$this = this;
    }

    importFileFromServer(event: any): void {
        ($('#roxyCustomPanel') as any).dialog({ modal: true, width: 875, height: 600 });
    }

    closeCustomRoxy(): void {
        ($('#roxyCustomPanel') as any).dialog('close');
    }

    onSelectCustomRoxy(fileSelected: any): void {
        EditUngVienComponent.$this.ungVien.noiDung = fileSelected;
    }
    troVe() {
        this.router.navigate(['/app/main/qlns/ungViens']);
    }
    //  ghep chuoi ten file 
    setFullNameFile() {
        this.dataDisplay.length = 0;
        const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        // this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        console.log(this.currentTime)
        this.value.forEach((ele) => {
            this.dataDisplay.push({ tepDinhKem: cValue + "/" + this.currentTime + "/" + ele.name });

        });


        this.value.length = 0;
        this.selectedRows = this.selectedRows.concat(this.dataDisplay);

        console.log(this.selectedRows)

        this.tepDinhKemSave = this.selectedRows.map(x => x.tepDinhKem.toString()).join(';')
    }


    uploadTDK(e: any) {

         this.filedinhkem = e.file.name;
        // const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        // this.tenFile = this.filedinhkem;
        // console.log(this.tenFile)


        this.dataDisplay.length = 0;
        const cValue = formatDate(this.currentDate, 'dd-MM-yyyy', 'en-US');
        // this.currentTime = new Date().getHours() + "-" + new Date().getMinutes();
        this.tenFile = this.filedinhkem;
        console.log(this.currentTime)
        this.tepDinhKem_Save = cValue + "/" + this.currentTime + "/" + e.file.name;
        // this.value.forEach((ele) => {
        //     this.dataDisplay.push({ tepDinhKem: cValue + "/" + this.currentTime + "/" + ele.name });

        // });


        // this.value.length = 0;
        // this.selected_Rows = this.selectedRows.concat(this.dataDisplay);

        // console.log(this.selected_Rows)

        // this.tepDinhKem_Save = this.selected_Rows.map(x => x.tepDinhKem.toString()).join(';')


    }
    deleteComment(id: number) {
        this.message.confirm(
            '', this.l('AreYouSure'),
            (isConfirmed) => {
                if (isConfirmed) {
                    this.listComment = this.listComment.filter(x => x.id !== id);
                    this._lichSuLamViecsServiceProxy.delete(id).subscribe((res) => {
                        console.log(res);
                    }, (err) => {
                        console.log(err);
                    }
                    );
                }
            }
        );
    }

    sendComment() {
        if (this.comment.trim().length == 0)
            return;
        else {
            let l = new LichSuLamViecDto();
            l.noiDung = this.comment;
            l.fullName = this.currentUserFullName;
            l.ungVienId = this.ungVienId;
            l.creatorUserId = this.currentUserId;
            l.creationTime = moment(new Date());
            this._lichSuLamViecsServiceProxy.createAndGetIdComment(l).subscribe(res => {
                l.id = res;
                this.listComment.unshift(l);
                console.log(res);
            }, (err) => {
                console.log(err);
            });
            this.comment = '';
        }
    }
    sendEmail(): void {

        const input = new SendTestEmailInput();
        input.filedinhkem = this.tenFile;
        input.curentTime = this.currentTime;
        // input.emailAddress = "hungnguyen23031996@gmail.com";
        input.mailForm = this.mail_From;       
        if( !this.email)
        {
            this.message.warn(" Email không được bỏ trống!");
            return;
        }
        else
        {
            input.emailAddress = this.email;
        }
        console.log(input.emailAddress) 
        // input.body = this.noiDungCD;
        if( !this.tenChuDe)
        {
            this.message.warn(" Chủ đề không được bỏ trống!");
            return;
        }
        else
        {
            input.subject = this.tenChuDe;
        }
        if( !this.noiDungCD)
        {
            this.message.warn(" Nội dung gửi mail không được bỏ trống!");
            return;
        }
        else
        {
            input.body = this.noiDungCD;
        }
       

        input.tenTruyCap = this.tenTruyCap;
        input.diaChiIP = this.diaChiIP;
        input.congSMTP = this.congSMTP;
        input.matKhau = this.matKhau;
        console.log(input)
        this._ungViensServiceProxy.sendEmailKH(input).subscribe(result => {
            this.notify.info(this.l('Đã gửi mail thành công'));
            const input1 = new CreateOrEditLichSuLamViecDto();
            input1.noiDung = this.noiDungCD;
            console.log(input1.noiDung)
            input1.tepDinhKem = this.tepDinhKem_Save;
            console.log( input1.tepDinhKem)
            input1.chuDe = this.tenChuDe;
            console.log(input1.chuDe)
            input1.ungVienId = this.ungVienId;
            this._lichSuLamViecsServiceProxy.createOrEdit(input1).pipe(finalize(() => { this.saving = false; })).subscribe(res => {
    
            })
        });

       
        
    }

    click(tenfle :string )
    {
        console.log(tenfle)
        this.link = this.rootUrl + "/" + tenfle;
        window.open(this.link, '_blank');
    }

    show(): void {
        const Id = this._activatedRoute.snapshot.paramMap.get('id');
        this.ngaySinh = null;
        this.ngayCap = null;
        this.approvE_DT = null;
        this._noiDaoTaosServiceProxy.getAllNoiDaoTao().subscribe((res) => {
            this.noiDaoTao = res;
        });
        this._tinhThanhsServiceProxy.getAllTinhThanh().subscribe((res) => {
            this.tinhThanh = res;
        });


        this.getAllTruongGiaoDich().subscribe((res) => {
            this.truongGiaoDich = res;
            this.viTriCongViec = this.truongGiaoDich.filter(x => x.code == 'VTUT');
            this.tienDoTuyenDung = this.truongGiaoDich.filter(x => x.code == 'TDTD');
            this.trangThai = this.truongGiaoDich.filter(x => x.code == 'TRTH');
            this.kenhTuyenDung = this.truongGiaoDich.filter(x => x.code == 'KTD');
            this.congty = this.truongGiaoDich.filter(x => x.code == 'CT');
            this.gioiTinh = this.truongGiaoDich.filter(x => x.code == 'GT');
            this.tinhTrangHonNhan = this.truongGiaoDich.filter(x => x.code == 'TTHN');
            this.trinhDoDaoTao = this.truongGiaoDich.filter(x => x.code == 'TDDT');
            this.xepLoaiHocLuc = this.truongGiaoDich.filter(x => x.code == 'XLHL');
        });
        if (Id != null) {
            this.ungVienId = Number.parseInt(Id);

            this._ungViensServiceProxy.getUngVienForEdit(this.ungVienId).subscribe(result => {
                this.ungVien = result.ungVien;
                this.tenCTY = this.ungVien.tenCTY;

                this.email = this.ungVien.email;
                console.log(this.tenCTY)
                this._templatesServiceProxy.getAllTemplate().subscribe((res) => {
                    this.chude_ = res;
                    this.chuDeGsoft = this.chude_.filter(x => x.maTemplate == 'GSOFT');

                    console.log(this.chuDeGsoft)
                    this.chuDeGoB = this.chude_.filter(x => x.maTemplate == 'GOBRANDING');
                    console.log(this.chuDeGoB)

                })
              
            
                    if (this.tenCTY == 'GSOFT') {
                        this._templatesServiceProxy.getAllTemplate().subscribe((res) => {

                            this.chuDeGsoft = this.chude_.filter(x => x.maTemplate == 'GSOFT');
                            this.chude = this.chuDeGsoft;
                        
                        })
                        this.id = 1;
                    }
                    else {
                        this._templatesServiceProxy.getAllTemplate().subscribe((res) => {
                            this.chuDeGoB = this.chude_.filter(x => x.maTemplate == 'GOBRANDING');
                            this.chude = this.chuDeGoB;
                           
                        })
                        this.id = 2;
                    }
                this._configEmailsServiceProxy.getConfigEmailForView(this.id).subscribe(res => {
                    this.mail_From = res.configEmail.diaChiEmail;
                    this.tenTruyCap = res.configEmail.tenTruyCap;
                    this.matKhau = res.configEmail.matKhau;
                    this.congSMTP = res.configEmail.congSMTP;
                    this.diaChiIP = res.configEmail.diaChiIP;
                    console.log(res)

                })

                if (this.ungVien.ngaySinh) {
                    this.ngaySinh = this.ungVien.ngaySinh.toDate();
                }
                if (this.ungVien.ngayCap) {
                    this.ngayCap = this.ungVien.ngayCap.toDate();
                }
                if (this.ungVien.approvE_DT) {
                    this.approvE_DT = this.ungVien.approvE_DT.toDate();
                }

                if (this.ungVien.tepDinhKem.length > 0) {

                    this.num = this.ungVien.tepDinhKem;
                    this.num.split(';').forEach(element => {
                        this.selectedRows.push({ tepDinhKem: element });

                    });
                }
            });
            this._lichSuLamViecsServiceProxy.getLichSuLamViecByUngVien(this.ungVienId).subscribe(res => {

                console.log(res)
                this.lichsu = res;

                
            })
        }
    }

    mouseClick( tenCTY :string ){
        console.log(tenCTY)    
        if (tenCTY == 'GSOFT') {
            this._templatesServiceProxy.getAllTemplate().subscribe((res) => {

                this.chuDeGsoft = this.chude_.filter(x => x.maTemplate == 'GSOFT');
                this.chude = this.chuDeGsoft;
            
            })
            this.id = 1;
        }
        else {
            this._templatesServiceProxy.getAllTemplate().subscribe((res) => {
                this.chuDeGoB = this.chude_.filter(x => x.maTemplate == 'GOBRANDING');
                this.chude = this.chuDeGoB;
               
            })
            this.id = 2;
        }
        this._configEmailsServiceProxy.getConfigEmailForView(this.id).subscribe(res => {
            this.mail_From = res.configEmail.diaChiEmail;
            this.tenTruyCap = res.configEmail.tenTruyCap;
            this.matKhau = res.configEmail.matKhau;
            this.congSMTP = res.configEmail.congSMTP;
            this.diaChiIP = res.configEmail.diaChiIP;
            console.log(res)

        })
       }  
    
    trangChuUngVien() {
        window.location.reload();
    }
    getDaTa(e: any) {
       
        this._templatesServiceProxy.getTemplateForView(e.value).subscribe(res => {
            console.log(res);
            this.tenChuDe = res.template.tenTemplate;
            this.noiDungCD = res.template.noiDung;

            console.log(this.noiDungCD)
        })
    }
    xoaFile(e: any) {
        this.selectedRows.splice(this.selectedRows.indexOf(e.row.data.tepDinhKem), 1);
        this.tepDinhKemSave = this.selectedRows.map(x => { return x.tepDinhKem.toString() }).join(';');
    }

    showDetail(e: any) {
        this.link = this.rootUrl + "/" + e.row.data.tepDinhKem;
        window.open(this.link, '_blank');
    }

    ngOnInit() {
        this.show();

        this.years = function (startYear) {
            var currentYear = new Date().getFullYear(), years = [];
            startYear = startYear || 1980;
            while (startYear <= currentYear) {
                years.push(startYear++);
            }
            return years;
        }
        this.listYear = this.years(2019 - 20);

    }

    save(): void {
        this._ungViensServiceProxy.getAllCMND().subscribe((res) => {
            if (this.ungVien.soCMND) {
                this.ungVienForCheckCMND = res;
                if (this.ungVienForCheckCMND.findIndex(x => x.soCMND == this.ungVien.soCMND && x.id !== this.ungVienId) > -1) {
                    this.message.warn("Số CMND đã bị trùng!");
                    return;
                }
            }
            this.saving = true;

            if (!this.ungVien.hoVaTen) {
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

            //nếu không thao tác với file thì lưu file cũ
            if (this.tepDinhKemSave) {
                this.ungVien.tepDinhKem = this.tepDinhKemSave;
            }
            // if (this.tenCTY) {
            //     this.ungVien.tenCTY = this.tenCTY;
            // }
            this._ungViensServiceProxy.createOrEdit(this.ungVien)
                .pipe(finalize(() => { this.saving = false; }))
                .subscribe(() => {
                    this.notify.info(this.l('SavedSuccessfully'));
                  this.show();
                });

        });
    }


}



