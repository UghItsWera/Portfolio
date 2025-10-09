let slideIndices = {};

function changeSlide(n, projectId) {
    if (!slideIndices[projectId]) slideIndices[projectId] = 1;
    showSlide(slideIndices[projectId] += n, projectId);
}

function showSlide(n, projectId) {
    const slides = document.querySelectorAll(`#slideshow-${projectId} .slide`);
    if (slides.length === 0) return;

    if (n > slides.length) slideIndices[projectId] = 1;
    if (n < 1) slideIndices[projectId] = slides.length;

    slides.forEach((s, i) => s.style.display = "none");
    slides[slideIndices[projectId] - 1].style.display = "block";
}

window.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".slideshow-container").forEach(container => {
        const id = container.id.split('-')[1];
        slideIndices[id] = 1;
        showSlide(1, id);
    });
});