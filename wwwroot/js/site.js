document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("sidebar");
    const menuToggle = document.getElementById("menuToggle");
    const menuToggleMobile = document.getElementById("menuToggleMobile");

    const isMobile = window.matchMedia("(max-width: 768px)").matches;

    // ===== DESKTOP (hover) =====
    if (!isMobile) {
        sidebar.addEventListener("mouseenter", function () {
            sidebar.classList.add("expanded");
        });

        sidebar.addEventListener("mouseleave", function () {
            sidebar.classList.remove("expanded");
        });
    }

    // ===== MOBIL (click) =====
    function toggleMenu() {
        sidebar.classList.toggle("expanded");
    }

    if (menuToggle) {
        menuToggle.addEventListener("click", toggleMenu);
    }

    if (menuToggleMobile) {
        menuToggleMobile.addEventListener("click", toggleMenu);
    }

    document.addEventListener("click", function (e) {
        if (isMobile && sidebar.classList.contains("expanded")) {
            if (!sidebar.contains(e.target) &&
                !menuToggleMobile.contains(e.target)) {
                sidebar.classList.remove("expanded");
            }
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {

    const materialBtn = document.getElementById("materialPdfBtn");

    if (materialBtn) {
        materialBtn.addEventListener("click", function () {
            materialBtn.textContent = "Materiallista redan nedladdad. Ladda ner igen.";
            materialBtn.classList.remove("rosa");
            materialBtn.style.backgroundColor = "#aaa";
            materialBtn.style.color = "white";
        });
    }

});
