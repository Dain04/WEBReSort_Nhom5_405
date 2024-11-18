////$(document).ready(function () {
////    let currentPage = 1;
////    const pageSize = 6;

////    // Sửa lại phần filter trong PhongList.js
////    function loadPhongs(page) {
////        $.ajax({
////            url: '/api/phong/GetPhongList',
////            method: 'GET',
////            data: {
////                page: page,
////                pageSize: pageSize,
////                TinhTrangID: 1  // Chỉ lấy phòng trống
////            },
////            success: function (data) {
////                console.log('Raw API data:', data);
////                // Kiểm tra toàn bộ data trước khi filter
////                renderPhongs(data); // Bỏ filter để xem toàn bộ dữ liệu trả về
////            },
////            error: function (error) {
////                console.error('Error:', error);
////                alert('Có lỗi xảy ra khi tải danh sách phòng');
////            }
////        });
////    }

////    function renderPhongs(phongs) {
////        const container = $('#room-container');
////        container.empty(); // Xóa nội dung cũ

////        if (phongs.length === 0) {
////            container.html('<p>Không có phòng nào phù hợp</p>');
////            return;
////        }

////        phongs.forEach(function (phong) {
////            const imagePath = phong.ImagePhong ? phong.ImagePhong.replace('~', '') : '';
////            const roomCard = `
////            <div class="room-card">
////                <div class="room-image">
////                    ${imagePath ?
////                    `<img src="${imagePath}" alt="Hình ảnh phòng ${phong.PhongID}" />` :
////                    '<div class="no-image">Không có hình ảnh</div>'}
////                </div>
////                <div class="room-info">
////                    <h3>Phòng ${phong.PhongID}</h3>
////                    <p class="room-price">Giá: ${new Intl.NumberFormat('vi-VN', {
////                        style: 'currency',
////                        currency: 'VND'
////                    }).format(phong.Gia)}</p>
////                    <p class="room-status">Tình trạng: ${phong.TinhTrang}</p>
////                    ${window.userIsLoggedIn ?
////                    `<a href="#" class="btn btn-book" data-id="${phong.PhongID}" 
////                            data-price="${phong.Gia}">Đặt Phòng</a>` :
////                    `<a href="/LoginUser/Index" class="btn btn-primary">
////                            Đăng nhập để đặt phòng
////                        </a>`}
////                </div>
////            </div>
////        `;
////            container.append(roomCard);
////        });
////    }

////    // Xử lý nút đặt phòng
////    $(document).on('click', '.btn-book', function (e) {
////        e.preventDefault();
////        const phongId = $(this).data('id');
////        $.ajax({
////            url: `/api/phong/GetPhongDetails/${phongId}`,
////            method: 'GET',
////            success: function (response) {
////                if (response.Success) {
////                    const phongData = response.Data;
////                    if (confirm(`Xác nhận đặt phòng ${phongId}?\nGiá: ${phongData.GiaFormatted}`)) {
////                        window.location.href = `/Phong/DatPhong/${phongId}`;
////                    }
////                }
////            },
////            error: function () {
////                alert('Có lỗi xảy ra khi lấy thông tin phòng');
////            }
////        });
////    });

////    // Load dữ liệu ban đầu
////    loadPhongs(currentPage);
////});