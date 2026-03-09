const themeKey = "ezz-theme";

function setTheme(theme) {
  document.documentElement.setAttribute("data-theme", theme);
  localStorage.setItem(themeKey, theme);
  const toggle = document.getElementById("themeToggle");
  if (toggle) {
    toggle.textContent = theme === "dark" ? "Light" : "Dark";
  }
}

const savedTheme = localStorage.getItem(themeKey) || "light";
setTheme(savedTheme);

document.getElementById("themeToggle")?.addEventListener("click", () => {
  const current =
    document.documentElement.getAttribute("data-theme") || "light";
  setTheme(current === "dark" ? "light" : "dark");
});

const revealTargets = document.querySelectorAll(
  ".glass-panel, .product-card, .hero-panel",
);
revealTargets.forEach((el, i) => {
  el.style.animationDelay = `${Math.min(i * 60, 420)}ms`;
  el.classList.add("reveal-in");
});
