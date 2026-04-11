(function () {
    var toggle = document.getElementById('theme-toggle');
    var html = document.documentElement;
    if (!toggle) return;

    var stored = localStorage.getItem('theme');

    if (stored) {
        html.setAttribute('data-theme', stored);
    } else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
        html.setAttribute('data-theme', 'dark');
    }

    toggle.addEventListener('click', function () {
        var current = html.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        localStorage.setItem('theme', next);
    });
})();
