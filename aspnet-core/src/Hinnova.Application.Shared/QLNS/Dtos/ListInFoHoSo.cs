
using System;
using Abp.Application.Services.Dto;

namespace Hinnova.QLNSDtos
{
    public class ListInfoHoSo : EntityDto
    {
        public string MaNhanVien { get; set; }

        public string HoVaTen { get; set; }

        public double LuongDongBH { get; set; }
        public double LuongCB { get; set; }

        public float TyLeDongBH{ get; set; }
        public string ViTriCV { get; set; }

        public string TrangThaiLV { get; set; }
       
        public string DonViCongTac { get; set; }
        public string TenHopDong { get; set; }
        public int DonViCongTacID { get; set; }
        public string KyHieuHD { get; set; }

        public DateTime NgayThuViec { get; set; }
        public string GioiTinh { get; set; }
        public string STT { get; set; }
    }
}
