(function () {
    function normaliseHex(value) {
        value = value.trim();

        if (!value.startsWith("#")) {
            value = "#" + value;
        }

        if (/^#[0-9a-fA-F]{6}$/.test(value)) {
            return value.toUpperCase();
        }

        return null;
    }

    function updatePreview(targetId, value) {
        var colourInput = document.getElementById(targetId);

        if (!colourInput) {
            return;
        }

        var mode = targetId.startsWith("light-")
            ? document.getElementById("light-theme-preview")
            : document.getElementById("dark-theme-preview");

        if (!mode) {
            return;
        }

        if (targetId.endsWith("background")) {
            mode.style.setProperty(
                "--theme-preview-background",
                value
            );
        }

        if (targetId.endsWith("accent")) {
            mode.style.setProperty(
                "--theme-preview-accent",
                value
            );
        }

        if (targetId.endsWith("card")) {
            mode.style.setProperty(
                "--theme-preview-card",
                value
            );
        }
    }

    function connectColourPicker(picker) {
        var targetId = picker.id;
        var hexInput = document.querySelector(
            '[data-colour-target="' + targetId + '"]'
        );

        if (!hexInput) {
            return;
        }

        picker.addEventListener("input", function () {
            var value = picker.value.toUpperCase();

            hexInput.value = value;

            updatePreview(targetId, value);
        });

        hexInput.addEventListener("input", function () {
            var value = normaliseHex(hexInput.value);

            if (!value) {
                return;
            }

            picker.value = value.toLowerCase();

            updatePreview(targetId, value);
        });

        hexInput.addEventListener("blur", function () {
            var value = normaliseHex(hexInput.value);

            if (value) {
                hexInput.value = value;
                picker.value = value.toLowerCase();

                updatePreview(targetId, value);
            } else {
                hexInput.value = picker.value.toUpperCase();
            }
        });

        updatePreview(targetId, picker.value.toUpperCase());
    }

    function initialise() {
        var pickers = document.querySelectorAll(".colour-picker");

        pickers.forEach(function (picker) {
            connectColourPicker(picker);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialise);
    } else {
        initialise();
    }
})();