const form = document.getElementById('myForm');
const email = document.getElementById('email');
const emailError = document.querySelector("#email + span.error");
const subjectError = document.querySelector("#subject + span.error");
const messageError = document.querySelector("#message + span.error");
const subject = document.getElementById('subject');
const message = document.getElementById('message');
const modal = document.getElementById('modal');
const url = "https://localhost:7298/api/Contact";


function onSubmit() {
    if (!email.validity.valid) {
        showError();
        return;
    }
    if (!subject.validity.valid) {
        showError();
        return;
    }
    if (!message.validity.valid) {
        showError();
        return;
    }

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Subject: subject.value,
            Email: email.value,
            Message: message.value,
        })

    }).then(response => {
        if (response.ok) {
            modal.className = "show";
            modal.textContent = "Thanks for Contacting us!";
            setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
        } else {
            modal.className = "show";
            modal.textContent = "Something went wrong!";
            setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
        }
    });
}

form.addEventListener('submit', function (event) {
    event.preventDefault();
    onSubmit();
});

function showError() {
    if (email.validity.valueMissing) {
        modal.className = "show";
        modal.textContent = "You need to enter an e-mail adress";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);

    } else if (email.validity.typeMismatch) {
        modal.className = "show";
        modal.textContent = "Enter value needs to be an email acdress";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
    } else if (email.validity.tooShort) {
        modal.className = "show";
        modal.textContent = "E-mail should be at least ${email.minLength} characters; you entered ${email.value.length}.";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
    }
    if (subject.validity.valueMissing) {
        modal.className = "show";
        modal.textContent = "You need to enter a Name";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);

    } else if (subject.validity.tooShort) {
        modal.className = "show";
        modal.textContent = "Name should be ${subject.minLength} characters; you entered ${subject.value.length}.";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
    }
    if (message.validity.valueMissing) {
        modal.className = "show";
        modal.textContent = "You need to enter a Message";
        setTimeout(function () { modal.className = modal.className.replace("show", ""); }, 3000);
    }

}
