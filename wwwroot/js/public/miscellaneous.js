(function () {
    const grid = document.getElementById("misc-grid");

    if (!grid) return;

    const cards = Array.from(
        grid.querySelectorAll(".misc-card")
    );

    function activateCard(card) {
        if (card.classList.contains("active")) {
            return;
        }

        cards.forEach(function (item) {
            item.classList.remove("active");
        });

        card.classList.add("active");

        requestAnimationFrame(function () {
            card.scrollIntoView({
                behavior: "smooth",
                block: "nearest"
            });
        });
    }

    cards.forEach(function (card) {

        card.addEventListener("click", function (event) {

            if (event.target.closest("a, button")) {
                return;
            }

            activateCard(card);
        });

        card.addEventListener("keydown", function (event) {

            if (event.key !== "Enter" && event.key !== " ") {
                return;
            }

            if (event.target.closest("a, button")) {
                return;
            }

            event.preventDefault();

            activateCard(card);
        });
    });
})();