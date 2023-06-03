const form = document.getElementById('register-form');
const email = document.getElementById('email');
const password = document.getElementById('password');
const confirmpassword = document.getElementById('ConfirmPassword');
const errorField = form.querySelector('.error');
const url = "https://localhost:7298/api/Accounts";


form.addEventListener('submit', async (event) => {
    event.preventDefault();

    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Email: email.value,
            Password: password.value,
            ConfirmPassword: confirmpassword.value
        })
    });

    const responseData = await response.json();
    console.log(responseData);

    if (responseData.successful != null) {

        window.location.href = '/Login/Login';

    } else {
        Object.keys(responseData.errors).forEach(key => {
            responseData.errors[key].forEach(errorMessage => {
                const error = document.createElement('span');
                error.className = 'error';
                error.textContent = errorMessage;
                form.appendChild(error);
            });
        });
    }


});
