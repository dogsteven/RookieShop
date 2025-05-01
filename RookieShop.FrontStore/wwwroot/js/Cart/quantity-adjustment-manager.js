class QuantityAdjustmentManager {
    constructor() {
        /**
         * 
         * @type {InputCounter[]}
         */
        this.inputCounters = [];
        /**
         * 
         * @type {HTMLInputElement[]}
         */
        this.quantityInputs = [];
        /**
         * 
         * @type {number[]}
         */
        this.initialQuantities = [];
        /**
         * 
         * @type {HTMLFormElement}
         */
        this.quantityAdjustmentForm = document.getElementById('quantity-adjustment-form');
    }

    /**
     * 
     * @param sku {string}
     * @param initialQuantity {number}
     */
    addCounter(sku, initialQuantity) {
        /**
         *
         * @type {HTMLInputElement}
         */
        const counter = document.getElementById(`quantity-counter-${sku}`);
        const increaseButton = document.getElementById(`increase-quantity-counter-button-${sku}`);
        const decreaseButton = document.getElementById(`decrease-quantity-counter-button-${sku}`);
        /**
         *
         * @type {HTMLInputElement}
         */
        const quantity = document.getElementById(`quantity-${sku}`);

        const options = {
            minValue: 0,
            maxValue: null,
            onIncrement: () => {
                quantity.value = counter.value;
                this.checkQuantities();
            },
            onDecrement: () => {
                quantity.value = counter.value;
                this.checkQuantities();
            }
        };

        const instanceOptions = {
            id: `counter-${sku}`,
            override: true
        };

        const inputCounter = new InputCounter(counter, increaseButton, decreaseButton, options, instanceOptions);

        this.inputCounters.push(inputCounter);
        this.quantityInputs.push(quantity);
        this.initialQuantities.push(initialQuantity);
    }
    
    checkQuantities() {
        let changed = false;
        
        for (let i = 0; i < this.quantityInputs.length; i++) {
            const quantityInput = this.quantityInputs[i];
            const initialQuantity = this.initialQuantities[i];
            
            if (parseInt(quantityInput.value) !== initialQuantity) {
                changed = true;
                break;
            }
        }
        
        if (changed) {
            this.quantityAdjustmentForm.classList.remove("hidden");
        } else {
            this.quantityAdjustmentForm.classList.add("hidden");
        }
    }
}