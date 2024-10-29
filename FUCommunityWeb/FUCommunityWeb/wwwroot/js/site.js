document.querySelector('.avatar-container').addEventListener('mouseover', function () {
    document.querySelector('.avatar-hover').classList.remove('d-none');
});

document.querySelector('.avatar-container').addEventListener('mouseout', function () {
    document.querySelector('.avatar-hover').classList.add('d-none');
});

function updateCustomAmount(value) {
    const customAmountGroup = document.getElementById('custom-amount-group');
    const customAmountInput = document.getElementById('custom-amount');

    if (value === 'custom') {
        customAmountGroup.style.display = 'block';
        customAmountInput.value = '';
        updatePoints();
    } else {
        customAmountGroup.style.display = 'none';
        document.getElementById('points').innerText = Math.floor(value / 1000);
    }
}

function updatePoints() {
    const customAmount = document.getElementById('custom-amount').value;
    document.getElementById('points').innerText = customAmount ? Math.floor(customAmount / 1000) : 0;
}

function confirmDeposit() {
    const amount = document.getElementById('preset-amount').value === 'custom'
        ? document.getElementById('custom-amount').value
        : document.getElementById('preset-amount').value;

    const points = document.getElementById('points').innerText;
    const paymentMethod = document.getElementById('payment-method').selectedOptions[0].text;

    // Update confirmation modal
    document.getElementById('confirm-amount').innerText = amount;
    document.getElementById('confirm-points').innerText = points;
    document.getElementById('confirm-payment-method').innerText = paymentMethod;

    // Show the confirmation modal
    const confirmationModal = new bootstrap.Modal(document.getElementById('confirmation-modal'));
    confirmationModal.show();
}

// Function to show/hide passwords
document.getElementById('showPassword').addEventListener('change', function () {
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    if (this.checked) {
        passwordInput.type = 'text'; // Show password
        confirmPasswordInput.type = 'text'; // Show confirm password
    } else {
        passwordInput.type = 'password'; // Hide password
        confirmPasswordInput.type = 'password'; // Hide confirm password
    }
});