class GDPR {
    constructor() {
        //this.showStatus();
        this.bindEvents();

        if (this.cookieStatus() !== "accept" && this.cookieStatus() !== "reject")
            this.showGDPR();
    }

    bindEvents() {
        let buttonAccept = document.querySelector(".gdpr-consent__button--accept");
        buttonAccept.addEventListener("click", () => {
            this.cookieStatus("accept");
            this.hideGDPR();
        });

        let buttonReject = document.querySelector(".gdpr-consent__button--reject");
        buttonReject.addEventListener("click", () => {
            this.cookieStatus("reject");
            this.hideGDPR();
        });
    }
    //showStatus() {
    //    document.getElementById("content-gdpr-consent-status").innerHTML =
    //        this.cookieStatus() == null ? "Niet gekozen" : this.cookieStatus();
    //}

    cookieStatus(status) {
        if (status) {
            const date = new Date();
            const metaData = {
                datum: `${date.getDate()}-${date.getMonth()}-${date.getFullYear()}`,
                tijd: `${date.getHours()}-${date.getMinutes()}`,
            };

            localStorage.setItem("gdpr-consent-metadata", JSON.stringify(metaData));
            localStorage.setItem("gdpr-consent-choice", status);
        }

        return localStorage.getItem("gdpr-consent-choice");
    }

    hideGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add("hide");
        document.querySelector(`.gdpr-consent`).classList.remove("show");
    }

    showGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add("show");
    }
}

const gdpr = new GDPR();
