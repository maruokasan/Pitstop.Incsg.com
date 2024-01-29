$(document).ready(function () {
    $('#carouselExample').carousel({
        interval: 4000
    });
});

document.addEventListener('DOMContentLoaded', function () {
    var swiper = new Swiper('.new-swiper', {
        // Enable Swiper only for screens smaller than or equal to 768 pixels
        breakpoints: {
            768: {
                slidesPerView: 'auto',
                spaceBetween: 20,
                enabled: true,
            }
        },
        // Swiper controls
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        // Adds pagination (optional)
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        // When window width is >= 769px, disable swiper
        on: {
            init: function () {
                if (window.innerWidth >= 769) {
                    this.destroy(true, true);
                }
            },
            resize: function () {
                if (window.innerWidth >= 769) {
                    this.destroy(true, true);
                } else if (window.innerWidth <= 768 && !this.initialized) {
                    this.init();
                }
            }
        }
    });
});