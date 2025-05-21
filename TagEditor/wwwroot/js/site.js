// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
class Api {
    /**
     * @returns {Promise<string>}
     */
    static async generateTitle(params) {
        return (await (await fetch("/Api/PromptGen/Title", {
            method: "POST",
            body: new URLSearchParams(params),
        })).json()).description;
    }
    /**
     * @returns {Promise<Array>}
     */
    static async randomPrompt() {
        return await (await fetch("/Api/PromptGen/Random")).json();
    }
    /**
     * @returns {Promise<Array>}
     */
    static async generatePrompt(params) {
        return await (await fetch("/Api/PromptGen/Generate", {
            method: "POST",
            body: new URLSearchParams(params),
        })).json();
    }
    /**
     * @returns {Promise<Array>}
     */
    static async completionTags(params) {
        return await (await fetch("/Api/PromptGen/CompletionTags", {
            method: "POST",
            body: new URLSearchParams(params),
        })).json();
    }
    /**
     * @returns {Promise<Array>}
     */
    static async getHistories() {
        return await (await fetch("/Api/History/Get")).json();
    }
    /**
     * @returns {Promise<Array>}
     */
    static async pushHistory(params) {
        return await (await fetch("/Api/History/Push", {
            method: "POST",
            body: new URLSearchParams(params),
        })).json();
    }
    /**
     * @returns {Promise<Array>}
     */
    static async clearHistory() {
        return await (await fetch("/Api/History/Clear", {
            method: "POST"
        })).json();
    }
}
document.querySelector("#save").addEventListener("click", save);
document.querySelector("#generateTitle").addEventListener("click", generateTitle);
document.querySelector("#randomBtn").addEventListener("click", randomPrompt);
document.querySelector("#complationTagBtn").addEventListener("click", completionTags);
document.querySelector("#magicBtn").addEventListener("click", generatePrompt);
document.querySelector("#candicates").addEventListener("click", e => {
    const dataset = e.target.closest(".candicate")?.dataset;
    if (dataset && dataset.prompt) {
        setPrompt(dataset.description, dataset.prompt);
    }
    if (dataset && dataset.tag) {
        addTag(dataset.tag);
    }
});
document.querySelector("#histories").addEventListener("click", e => {
    const dataset = e.target.closest(".history").dataset;
    setPrompt(dataset.description, dataset.prompt, dataset.negative);
});

document.querySelector("#clearHistory").addEventListener("click", async e => {
    await Api.clearHistory();
    document.querySelector("#histories").replaceChildren();
});

function save() {
    pushHistory(document.querySelector("#title").value, document.querySelector("#prompt").value, document.querySelector("#negative").value);
}
async function generateTitle() {
    try {
        const title = await Api.generateTitle({ "Prompt": document.querySelector("#prompt").value });
        if (title) {
            document.querySelector("#title").value = title;
            save();
        }
    } catch (e) {
        new MessageDialog("Api.generateTitle の呼び出しでエラー", "OK").show();
        console.error(e);
    }
}
async function randomPrompt() {
    document.querySelector("#candicates").replaceChildren();
    document.querySelector("#candicates").classList.add("loading");

    try {
        (await Api.randomPrompt()).map(e => {
            appendPromptCandicate(e.description, e.promptText);
        });
    } catch (e) {
        new MessageDialog("Api.randomPrompt の呼び出しでエラー", "OK").show();
        console.error(e);
    } finally {
        document.querySelector("#candicates").classList.remove("loading");
    }
}

async function generatePrompt() {
    const dialog = new InputDialog("要望を入力", "");
    const input = await dialog.show();
    if (input) {
        document.querySelector("#candicates").replaceChildren();
        document.querySelector("#candicates").classList.add("loading");
        try {
            (await Api.generatePrompt({ "BeforePrompt": document.querySelector("#prompt").value, "Request": input })).map(e => {
                appendPromptCandicate(e.description, e.promptText);
            });
        } catch (e) {
            new MessageDialog("Api.generatePrompt の呼び出しでエラー", "OK").show();
            console.error(e);
        } finally {
            document.querySelector("#candicates").classList.remove("loading");
        }
    }
}

async function completionTags() {
    document.querySelector("#candicates").replaceChildren();
    document.querySelector("#candicates").classList.add("loading");
    try {
        const categories = await Api.completionTags({ "Prompt": document.querySelector("#prompt").value });
        const w = document.createElement("div");
        for (const category of categories) {
            const c = document.createElement("strong");
            c.append(category.category);
            w.append(c);
            w.append(...category.tags.map(tag => {
                const btn = document.createElement("button");
                btn.dataset.tag = tag;
                btn.classList.add("candicate", "tag", "btn", "btn-sm");
                btn.append(tag);
                return btn;
            }));

        }
        document.querySelector("#candicates").append(w);
    } catch (e) {
        new MessageDialog("Api.generatePrompt の呼び出しでエラー", "OK").show();
        console.error(e);
    } finally {
        document.querySelector("#candicates").classList.remove("loading");
    }
}

function appendPromptCandicate(description, promptText) {
    const element = document.createElement("div");
    element.classList.add("candicate", "prompt", "mb-1", "p-1");
    element.dataset.prompt = promptText;
    element.dataset.description = description;
    const text = document.createElement("p");
    text.append(description);
    element.append(text);
    const prompt = document.createElement("code");
    prompt.append(promptText);
    element.append(prompt);
    document.querySelector("#candicates").append(element);
}

function appendPromptHistory(description, promptText, negative) {
    const element = document.createElement("div");
    element.classList.add("history", "mb-1", "p-1");
    element.dataset.prompt = promptText;
    element.dataset.description = description;
    element.dataset.negative = negative;
    const text = document.createElement("p");
    text.append(description);
    element.append(text);
    const prompt = document.createElement("code");
    prompt.append(promptText);
    element.append(prompt);
    document.querySelector("#histories").insertAdjacentElement("afterbegin", element);
}

async function setPrompt(description, prompt, negative) {
    negative = negative ?? document.querySelector("#negative").value;
    document.querySelector("#prompt").value = prompt;
    document.querySelector("#title").value = description;
    document.querySelector("#negative").value = negative;
    await pushHistory(description, prompt, negative);
}

async function addTag(tag) {
    let prompt = document.querySelector("#prompt").value;
    if (!prompt.trim().endsWith(",")) {
        prompt += (", " + tag);
    } else {
        prompt += tag;
    }
    document.querySelector("#prompt").value = prompt;
}

const histories = [];

async function pushHistory(description, prompt, negative) {

    if (histories.some(e => e.title == description && e.prompt == prompt && (e.negative ?? "") == (negative ?? ""))) {
        return;
    }
    try {
        await Api.pushHistory({
            "Title": description,
            "Prompt": prompt,
            "Negative": negative,
        });
    } catch (e) {
        new MessageDialog("Api.pushHistory の呼び出しでエラー", "OK").show();
        console.error(e);
    }
    histories.push({
        "title": description,
        "prompt": prompt,
        "negative": negative,
    });

    appendPromptHistory(description, prompt, negative);
}

setTimeout(async () => {
    try {
        (await Api.getHistories()).forEach(e => {
            appendPromptHistory(e.title ?? "", e.prompt ?? "", e.negative ?? "");
            histories.push({
                "title": e.title,
                "prompt": e.prompt,
                "negative": e.negative,
            });
        });
    } catch (e) {
        new MessageDialog("Api.getHistories の呼び出しでエラー", "OK").show();
        console.error(e);
    }
});
