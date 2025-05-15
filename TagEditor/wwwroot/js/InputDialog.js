class InputDialog {
    message;
    text;
    buttons = ["OK"];
    dialog;
    constructor(message, text) {
        this.message = message;
        this.text = text;
    }

    async show() {
        this.dialog = document.querySelector("#inputDialogTemplate").content.cloneNode(true).firstElementChild;

        this.dialog.querySelector("p").replaceChildren(this.message);
        this.dialog.querySelector("textarea").value = this.text;
        this.dialog.querySelector("nav").replaceChildren(...this.buttons.map(value => {
            const button = document.createElement("button");
            button.append(value);
            button.classList.add("btn", "btn-primary");
            button.value = value;
            return button;
        }));

        document.body.append(this.dialog);

        return new Promise((resolve) => {
            this.dialog.showModal();
            this.dialog.addEventListener('close', () => {
                this.dialog.remove();

                resolve(this.dialog.querySelector("textarea").value);
            }, { once: true });

            this.dialog.addEventListener('click', (event) => {
                if (event.target === this.dialog) {
                    this.dialog.close();
                    this.dialog.remove();
                    resolve();
                }
            });
        });
    }
}