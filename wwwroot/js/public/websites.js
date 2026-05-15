(function () {
    const grid = document.getElementById('websites-grid');
    if (!grid) return;

    const cards = Array.from(grid.querySelectorAll('.website-card'));

    cards.forEach(card => {
        card.addEventListener('click', () => {
            const isAlreadyActive = card.classList.contains('active');
            if (isAlreadyActive) return;

            // Deactivate all
            cards.forEach(c => c.classList.remove('active'));

            // Activate clicked
            card.classList.add('active');

            // Scroll card into view smoothly
            card.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        });
    });
})();