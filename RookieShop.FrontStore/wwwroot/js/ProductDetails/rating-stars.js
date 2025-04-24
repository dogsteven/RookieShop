class RatingStarsManager {
    /**
     * 
     * @param ratingStarPrefix {string}
     */
    constructor(ratingStarPrefix) {
        /**
         *
         * @type {((number) => void)[]}
         */
        this.listeners = [];

        /**
         *
         * @type {HTMLElement[]}
         */
        this.ratingStars = [];
        
        for (let i = 0; i < 5; i++) {
            const ratingStar = document.getElementById(`${ratingStarPrefix}-${i + 1}`);
            
            ratingStar.addEventListener("click", () => {
                for (const listener of this.listeners) {
                    listener(i + 1);
                }
            });
            
            this.ratingStars.push(ratingStar);
        }
        
        this.listeners.push((score) => {
            for (let i = 0; i < score; ++i) {
                this.ratingStars[i].classList.remove("text-gray-300", "dark:text-gray-500");
                this.ratingStars[i].classList.add("text-yellow-300");
            }

            for (let i = score; i < 5; ++i) {
                this.ratingStars[i].classList.add("text-gray-300", "dark:text-gray-500");
                this.ratingStars[i].classList.remove("text-yellow-300");
            }
        });
    }

    /**
     * 
     * @param score {number}
     */
    setScore(score) {
        for (const listener of this.listeners) {
            listener(score);
        }
    }

    /**
     * 
     * @param listener {(number) => void}
     */
    addListener(listener) {
        this.listeners.push(listener);
    }
}