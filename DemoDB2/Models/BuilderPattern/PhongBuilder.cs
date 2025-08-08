using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.Models.BuilderPattern
{
    public class PhongBuilder
    {
        private Phong _phong;

        public PhongBuilder()
        {
            _phong = new Phong();
        }

        //public Phong(int phongID, int idTinhTrang, int idLoai, decimal gia, string imagePhong)
        //{
        //    this.PhongID = phongID;
        //    this.IDTinhTrang = idTinhTrang;
        //    this.IDLoai = idLoai;
        //    this.Gia = gia;
        //    this.ImagePhong = imagePhong;
        //}
        public PhongBuilder SetPhongID(int phongID)
        {
            _phong.PhongID = phongID;
            return this;
        }

        public PhongBuilder SetIDTinhTrang(int? idTinhTrang)
        {
            _phong.IDTinhTrang = idTinhTrang;
            return this;
        }

        public PhongBuilder SetIDLoai(int? idLoai)
        {
            _phong.IDLoai = idLoai;
            return this;
        }

        public PhongBuilder SetGia(decimal? gia)
        {
            if (gia <= 0)
                throw new ArgumentException("Giá phòng phải lớn hơn 0");
            _phong.Gia = gia;
            return this;
        }

        public PhongBuilder SetImagePhong(string imagePhong)
        {
            if (string.IsNullOrEmpty(imagePhong))
                throw new ArgumentException("Ảnh phòng không được để trống");
            _phong.ImagePhong = imagePhong;
            return this;
        }

        public PhongBuilder AddDatPhong(DatPhong datPhong)
        {
            _phong.DatPhong.Add(datPhong);
            return this;
        }

        public PhongBuilder AddHoaDon(HoaDon hoaDon)
        {
            _phong.HoaDon.Add(hoaDon);
            return this;
        }

        public Phong Build()
        {
            return _phong;
        }
    }
}