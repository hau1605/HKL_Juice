/*

document.addEventListener("DOMContentLoaded", function () {
    updateActiveLink();

    var navbarLinks = document.querySelectorAll("#main-nav .nav-link");

    navbarLinks.forEach(function (link) {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            var url = this.getAttribute("href");
            loadContent(url);
            updateActiveClass(navbarLinks, this);
        });
    });
});
// =============================================================================================
// =========================================LOAD CONTENT========================================
// =============================================================================================
function loadContent(url) {
    fetch(url)
        .then(function (response) {
            return response.text();
        })
        .then(function (html) {
            var parser = new DOMParser();
            var doc = parser.parseFromString(html, "text/html");
            var newContent = doc.getElementById("content").innerHTML;
            document.getElementById("content").innerHTML = newContent;
            history.pushState(null, "", url);
        })
        .catch(function (err) {
            console.warn("Something went wrong.", err);
        });
}
// =============================================================================================
// ==================================UPDATE ACTIVE CLASS HEADER=================================
// =============================================================================================
function updateActiveClass(links, activeLink) {
    links.forEach(function (link) {
        link.classList.remove("active");
    });
    activeLink.classList.add("active");
}

window.addEventListener("popstate", function (event) {
    var currentPath = window.location.pathname;
    updateContent(currentPath);
    updateActiveLink();
});

function updateActiveLink() {
    var currentPath = window.location.pathname;
    var navbarLinks = document.querySelectorAll("#main-nav .nav-link");
    navbarLinks.forEach(function (link) {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        } else {
            link.classList.remove("active");
        }
    });
}*/