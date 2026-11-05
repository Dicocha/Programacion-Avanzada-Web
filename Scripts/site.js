(function () {
    "use strict";

    $(function () {
        var path = window.location.pathname.toLowerCase();

        $(".navbar-nav .nav-link").each(function () {
            var href = ($(this).attr("href") || "").toLowerCase();

            if (href && href !== "/" && path.indexOf(href) === 0) {
                $(this).addClass("active");
            }
        });
    });
})(jQuery);
