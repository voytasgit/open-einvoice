document.addEventListener("DOMContentLoaded", () => {

    const dropZone = document.getElementById("dropZone");
    const fileInput = document.getElementById("fileInput");

    // 👉 Script nur ausführen, wenn wir auf der Import-Seite sind
    if (!dropZone || !fileInput) {
        return;
    }

    // --- Verhindern, dass Browser Datei öffnet
    ["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => {
        dropZone.addEventListener(eventName, e => {
            e.preventDefault();
            e.stopPropagation();
        });
    });

    // --- Visuelles Feedback
    dropZone.addEventListener("dragover", () => {
        dropZone.classList.add("bg-light");
    });

    dropZone.addEventListener("dragleave", () => {
        dropZone.classList.remove("bg-light");
    });

    // --- Datei ablegen
    dropZone.addEventListener("drop", e => {
        dropZone.classList.remove("bg-light");

    const files = e.dataTransfer.files;
        if (files.length > 0) {
        fileInput.files = files;
        }
    });

});

