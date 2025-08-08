let slideIndex = 1;

function changeSlide(n) {
    showSlides(slideIndex += n);
}

function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let slides = document.getElementsByClassName("slides");
    let dots = document.getElementsByClassName("dot");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (let i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (let i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    setTimeout(() => {
        slides[slideIndex - 1].style.opacity = 1;
    }, 50);
}

document.addEventListener('DOMContentLoaded', function () {
    showSlides(slideIndex);

    // Tự động chuyển slide
    setInterval(() => {
        changeSlide(1);
    }, 5000);
});

// Thêm các hàm vào đối tượng window để có thể gọi từ HTML
window.changeSlide = changeSlide;
window.currentSlide = currentSlide;