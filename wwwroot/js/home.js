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

// Featured Section
document.querySelectorAll('.featured__button').forEach(button => {
    button.addEventListener('click', function () {
        const fileInput = this.parentElement.querySelector('.featured__file-input');
        const title = this.parentElement.querySelector('.featured__title').textContent;
        const price = this.parentElement.querySelector('.featured__price').textContent;

        if (fileInput && fileInput.files && fileInput.files.length > 0) {
            const formData = new FormData();
            formData.append('imageFile', fileInput.files[0]);
            formData.append('title', title);
            formData.append('price', price);

            fetch('/Home/UploadFeaturedImage', {
                method: 'POST',
                body: formData,
            })
            .then(response => {
                if (response.ok) {
                    // File uploaded successfully
                } else {
                    // Error handling
                }
            })
            .catch(error => {
                // Error handling
            });
        } else {
            // Error handling: No file selected
        }
    });
});

// // Product Section
// document.addEventListener('DOMContentLoaded', function () {
//     const productsContainer = document.getElementById('products-container');

//     fetchProducts();

//     function fetchProducts() {
//         fetch('/Home/GetProductMedia')
//             .then(response => response.json())
//             .then(data => {
//                 // Limit to maximum 6 products
//                 const productsToShow = data.slice(0, 6);
//                 displayProducts(productsToShow);
//             })
//             .catch(error => {
//                 console.error('Error fetching products:', error);
//             });
//     }

//     function displayProducts(products) {
//         products.forEach(product => {
//             const article = document.createElement('article');
//             article.classList.add('products__card');

//             const img = document.createElement('img');
//             img.src = product.image; // Assuming your product data has an image field
//             img.alt = product.title; // Assuming your product data has a title field
//             img.classList.add('products__img');
//             article.appendChild(img);

//             const title = document.createElement('h3');
//             title.textContent = product.title;
//             title.classList.add('products__title');
//             article.appendChild(title);

//             const price = document.createElement('span');
//             price.textContent = '$' + product.price; // Assuming your product data has a price field
//             price.classList.add('products__price');
//             article.appendChild(price);

//             const button = document.createElement('button');
//             button.classList.add('products__button');
//             button.innerHTML = '<i class="bx bx-shopping-bag"></i>';
//             article.appendChild(button);

//             productsContainer.appendChild(article);
//         });
//     }
// });

document.addEventListener('DOMContentLoaded', function () {
    const storyTitle = document.getElementById('storyTitle');
    const storyDescription = document.getElementById('storyDescription');
    const storyImagePreview = document.getElementById('storyImagePreview');

    fetch('/Home/GetOurStory')
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch story data');
            }
            return response.json();
        })
        .then(data => {
            if (data) { // Check if data is not null
                // Populate static elements with fetched data
                storyTitle.textContent = data.title;
                storyDescription.textContent = data.description;
                // Construct the URL for the image
                const imageUrl = `/OurStoryImages/${data.imageUrl}`;
                storyImagePreview.src = imageUrl;
            } else {
                console.error('No story data found');
            }
        })
        .catch(error => {
            console.error('Error fetching or parsing story data:', error);
            // Optionally, update the DOM to display an error message
        });
});

