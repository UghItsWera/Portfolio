
(function () {
    function initialiseNavigation() {
        var toggle = document.getElementById("hamburger-toggle");
        var menu = document.getElementById("public-nav-links");

        if (!toggle || !menu) {
            return;
        }

        function closeMenu() {
            menu.classList.remove("is-open");
            toggle.setAttribute("aria-expanded", "false");
            toggle.setAttribute("aria-label", "Open navigation menu");
        }

        function openMenu() {
            menu.classList.add("is-open");
            toggle.setAttribute("aria-expanded", "true");
            toggle.setAttribute("aria-label", "Close navigation menu");
        }

        toggle.addEventListener("click", function () {
            var isOpen = toggle.getAttribute("aria-expanded") === "true";

            if (isOpen) {
                closeMenu();
            } else {
                openMenu();
            }
        });

        // Close the menu after choosing a page.
        menu.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", closeMenu);
        });

        // Close the menu when clicking elsewhere.
        document.addEventListener("click", function (event) {
            if (!toggle.contains(event.target) &&
                !menu.contains(event.target)) {
                closeMenu();
            }
        });

        // Support closing the menu with Escape.
        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                closeMenu();
                toggle.focus();
            }
        });

        // Reset the menu when returning to desktop width.
        window.addEventListener("resize", function () {
            if (window.innerWidth > 1080) {
                closeMenu();
            }
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialiseNavigation);
    } else {
        initialiseNavigation();
    }
})();
