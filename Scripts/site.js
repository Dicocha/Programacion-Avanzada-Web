(function () {
    "use strict";

    $(function () {
        var path = window.location.pathname.toLowerCase();
        var activeLink = null;
        var activeLength = 0;

        $(".navbar-nav .nav-link").each(function () {
            var href = ($(this).attr("href") || "").toLowerCase();

            if (href && href !== "/" && path.indexOf(href) === 0 && href.length > activeLength) {
                activeLink = this;
                activeLength = href.length;
            }
        });

        if (activeLink) {
            $(activeLink).addClass("active");
        }
    });
})(jQuery);
