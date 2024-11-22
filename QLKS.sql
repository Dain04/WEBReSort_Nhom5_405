USE master;
GO
-- Tạo cơ sở dữ liệu QLKS (nếu chưa tồn tại)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'QLKS')
BEGIN
    CREATE DATABASE QLKS;
END
GO
USE QLKS;
GO

-- Tạo bảng Xe
CREATE TABLE Xe (
    XeID INT PRIMARY KEY IDENTITY,
    HieuXe NVARCHAR(255),
    BienSoXe CHAR(10),
    TaiXe NVARCHAR(255),
    SoChoNgoi INT,
    GiaXe DECIMAL(18, 2),
    ImageXe NVARCHAR(MAX) NULL
);
-- Tạo bảng MonAn
CREATE TABLE MonAn (
    MonAnID INT PRIMARY KEY IDENTITY,
    TenMon NVARCHAR(255),
    GiaMon DECIMAL(18, 2),
	SoLuongMon int,
    ImageMonAn NVARCHAR(MAX) NULL
);
-- Tạo bảng Spa
CREATE TABLE Spa (
    SpaID INT PRIMARY KEY IDENTITY,
    TenDichVu NVARCHAR(255),
    MoTa NVARCHAR(MAX),
    GiaDichVu DECIMAL(18, 2),
    ThoiGianDichVu INT, 
    ImageSpa NVARCHAR(MAX) NULL
);
-- Tạo bảng LoaiPhong
CREATE TABLE LoaiPhong (
    IDLoai INT PRIMARY KEY IDENTITY,
    TenLoai NCHAR(20) UNIQUE NOT NULL
);
Create table TinhTrangPhong(
	IDTinhTrang Int primary key identity,
	TenTinhTrang Nchar(20) Unique not null);

-- Tạo bảng Phong
CREATE TABLE Phong (
    PhongID INT PRIMARY KEY IDENTITY,
      IDTinhTrang INT,
    IDLoai INT,
    Gia DECIMAL(18, 2),
    ImagePhong NVARCHAR(MAX) NULL,
	 FOREIGN KEY (IDTinhTrang) REFERENCES TinhTrangPhong(IDTinhTrang),
    FOREIGN KEY (IDLoai) REFERENCES LoaiPhong(IDLoai)
);

-- Tạo bảng NguoiDung
CREATE TABLE NguoiDung (
    NguoiDungID INT PRIMARY KEY IDENTITY,
    TenNguoiDung NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(20),
    Email NVARCHAR(255) NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
	ImageUser NVARCHAR(MAX) NULL
);
-- Tạo bảng ChucVu
CREATE TABLE ChucVu (
    ChucVuID INT PRIMARY KEY IDENTITY,
    TenChucVu NVARCHAR(255),
    MoTaChucVu NVARCHAR(255)
);
-- Tạo bảng NhanVien
CREATE TABLE NhanVien (
    NhanVienID INT PRIMARY KEY IDENTITY,
    ChucVuID INT,
    Luong DECIMAL(18, 2),
    Ten NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(20),
    Email NVARCHAR(255) NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
	ImageNhanVien NVARCHAR(MAX) NULL,
    FOREIGN KEY (ChucVuID) REFERENCES ChucVu(ChucVuID)
);
-- Tạo bảng HoaDon
CREATE TABLE HoaDon (
    HoaDonID INT PRIMARY KEY IDENTITY,
    KhachHangID INT,
    PhongID INT,
    NgayNhanPhong DATE,
    NgayTraPhong DATE,
    TongTien DECIMAL(18, 2),
    IDDichVuSuDung INT,
    NguoiDungID INT,
    NhanVienID INT,
	Code VARCHAR(50),
	TenNguoiDung VARCHAR(100),
	NgayTaoHD DATE,
	NgayChinhSua DATE,
	OrderVnPayID int,
    TrangThaiThanhToan VARCHAR(20),

    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(NguoiDungID),
    FOREIGN KEY (PhongID) REFERENCES Phong(PhongID)
);
-- Tạo bảng DichVu
CREATE TABLE DichVuSuDung (
    IDDichVuSuDung INT PRIMARY KEY IDENTITY,
    XeID INT,
    MonAnID INT,
    GiatUiID INT,
    SpaID INT,
    HoaDonID INT,
    NhanVienID INT,
    NguoiDungID INT,
    FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID),
    FOREIGN KEY (XeID) REFERENCES Xe(XeID),
    FOREIGN KEY (MonAnID) REFERENCES MonAn(MonAnID),
    FOREIGN KEY (SpaID) REFERENCES Spa(SpaID),
    FOREIGN KEY (HoaDonID) REFERENCES HoaDon(HoaDonID),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(NguoiDungID)
);
-- Tạo bảng DatPhong
CREATE TABLE DatPhong (
    DatPhongID INT PRIMARY KEY IDENTITY,
    PhongID INT,
    NgayDatPhong DATE,
    NgayNhanPhong DATE,
    NgayTraPhong DATE,
    NguoiDungID INT,
    IDDichVuSuDung INT,
    IDTinhTrang INT,
    ImagePhong NVARCHAR(MAX) NULL,
    FOREIGN KEY (IDDichVuSuDung) REFERENCES DichVuSuDung(IDDichVuSuDung),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(NguoiDungID),
    FOREIGN KEY (PhongID) REFERENCES Phong(PhongID),
    FOREIGN KEY (IDTinhTrang) REFERENCES TinhTrangPhong(IDTinhTrang)
);
-- Tạo bảng Luong
CREATE TABLE Luong (
    LuongID INT PRIMARY KEY IDENTITY,
    NhanVienID INT NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    LuongMotGio DECIMAL(18, 2) NOT NULL,
    TongLuong DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID)
);
-- Tạo bảng LichLamViec
CREATE TABLE LichLamViec (
    LichLamViecID INT PRIMARY KEY IDENTITY,
    NhanVienID INT NOT NULL,
    Ngay INT NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    SoCaLamViec INT NOT NULL,
    CaSang BIT NOT NULL DEFAULT 0,
    CaChieu BIT NOT NULL DEFAULT 0,
    CaToi BIT NOT NULL DEFAULT 0,
    CaDem BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID),
    CONSTRAINT CK_SoCaLamViec CHECK (SoCaLamViec BETWEEN 1 AND 2)
);

