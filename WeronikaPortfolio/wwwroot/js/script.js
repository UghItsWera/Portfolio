// Theme toggle logic
const toggleBtn = document.getElementById('theme-toggle');
const html = document.documentElement;

function setTheme(mode) {
  html.setAttribute('data-theme', mode);
  localStorage.setItem('theme', mode);
  toggleBtn.textContent = mode === 'dark' ? '🌞' : '🌓';
}

toggleBtn.addEventListener('click', () => {
  const current = html.getAttribute('data-theme') || 'light';
  setTheme(current === 'light' ? 'dark' : 'light');
});

// On load
document.addEventListener('DOMContentLoaded', () => {
  const saved = localStorage.getItem('theme') || 'light';
  setTheme(saved);

  // Re-run typewriter title
  const typewriter = document.querySelector('.typewriter');
  const titles = ['a Software Engineer', 'a Creative Coder', 'a Problem Solver'];
  let i = 0;
  setInterval(() => {
    typewriter.textContent = titles[i % titles.length];
    i++;
  }, 4000);
});
// Scroll animations using Intersection Observer
const fadeElements = document.querySelectorAll('.fade-in');

const observer = new IntersectionObserver(entries => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add('visible');
    }
  });
}, {
  threshold: 0.1
});

fadeElements.forEach(el => observer.observe(el));

// Form feedback (optional, works even without backend)
const form = document.getElementById('contact-form');
const status = document.getElementById('form-status');

if (form) {
  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const data = new FormData(form);
    const action = form.action;

    status.textContent = "Sending...";

    try {
      const response = await fetch(action, {
        method: 'POST',
        body: data,
        headers: { 'Accept': 'application/json' }
      });

      if (response.ok) {
        status.textContent = "Thanks for your message! 🌸";
        form.reset();
      } else {
        status.textContent = "Oops! Something went wrong.";
      }
    } catch (error) {
      status.textContent = "Network error. Try again later.";
    }
  });
}
