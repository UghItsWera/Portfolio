(function () {
    var toggle = document.getElementById('theme-toggle');
    var html = document.documentElement;
    var header = document.querySelector('.site-header');

    /* =========================================================
       THEME TOGGLE
       ========================================================= */

    if (toggle) {
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
    }


    /* =========================================================
       AUTO-HIDING NAVIGATION
       ========================================================= */

    if (header) {
        var lastScrollY = window.scrollY;
        var ticking = false;

        function updateNavigation() {
            var currentScrollY = window.scrollY;
            var scrollDifference = currentScrollY - lastScrollY;

            /*
             * Keep the navigation visible near the top of the page.
             */
            if (currentScrollY <= 30) {
                header.classList.remove('nav-hidden');
                lastScrollY = currentScrollY;
                ticking = false;
                return;
            }

            /*
             * Scrolling down → hide navigation.
             */
            if (scrollDifference > 5) {
                header.classList.add('nav-hidden');
            }

            /*
             * Scrolling up → show navigation.
             */
            else if (scrollDifference < -5) {
                header.classList.remove('nav-hidden');
            }

            lastScrollY = currentScrollY;
            ticking = false;
        }

        window.addEventListener('scroll', function () {
            if (!ticking) {
                window.requestAnimationFrame(updateNavigation);
                ticking = true;
            }
        }, { passive: true });
    }

})();