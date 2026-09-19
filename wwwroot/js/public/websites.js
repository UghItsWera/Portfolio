(function () {
    const grid = document.getElementById("websites-grid");

    if (!grid) return;

    const cards = Array.from(
        grid.querySelectorAll(".website-card")
    );

    function activateCard(card) {
        const isAlreadyActive = card.classList.contains("active");

        if (isAlreadyActive) {
            return;
        }

        cards.forEach(function (item) {
            item.classList.remove("active");
            item.setAttribute("aria-expanded", "false");
        });

        card.classList.add("active");
        card.setAttribute("aria-expanded", "true");

        card.scrollIntoView({
            behavior: "smooth",
            block: "nearest"
        });
    }

    cards.forEach(function (card) {

        card.addEventListener("click", function (event) {
            const interactiveElement = event.target.closest("a, button");

            if (interactiveElement) {
                return;
            }

            activateCard(card);
        });

        card.addEventListener("keydown", function (event) {
            if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                activateCard(card);
            }
        });
    });
})();