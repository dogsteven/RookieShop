/**
 * 
 * @type {HTMLElement[]}
 */
const ratingStars = [];

/**
 * 
 * @type {HTMLInputElement}
 */
const scoreInput = document.getElementById("review-score");

/**
 * 
 * @type {HTMLSpanElement}
 */
const scoreText = document.getElementById("review-score-text");

for (let i = 0; i < 5; ++i) {
    const ratingStar = document.getElementById(`rating-star-${i + 1}`);
    
    ratingStar.addEventListener("click", (event) => {
        event.preventDefault();
        updateRatingStars(i + 1);
    });
    
    ratingStars.push(ratingStar);
}

/**
 * 
 * @param stars {number}
 */
function updateRatingStars(stars) {
    for (let i = 0; i < stars; ++i) {
        ratingStars[i].classList.remove("text-gray-300", "dark:text-gray-500");
        ratingStars[i].classList.add("text-yellow-300");
    }
    
    for (let i = stars; i < 5; ++i) {
        ratingStars[i].classList.add("text-gray-300", "dark:text-gray-500");
        ratingStars[i].classList.remove("text-yellow-300");
    }
    
    scoreInput.value = `${stars}`;
    scoreText.innerText = `${stars}`;
}

document.addEventListener("DOMContentLoaded", () => {
    updateRatingStars(1);
});