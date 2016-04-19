// site.js
(function startup() {

    ////var ele = document.getElementById("username");
    ////ele.innerHTML = "Steve T";
    //var ele = $("#username");
    //ele.text("Steve T");

    //// var main = document.getElementById("main");
    //var main = $("#main");
    ////main.onmouseenter = function() {
    ////    main.style = "background-color: #888; ";
    ////}
    ////main.on("mouseenter", function() {
    ////    main[0].style = "background-color: #888; ";
    ////});

    //main.on("mouseenter", mouseEnter);
    //main.on("mouseleave", mouseLeave);

    //var menuItems = $("ul.menu li a");
    //menuItems.on("click", menuClick);

    function mouseLeave() {
        this.style = "";
    }

    function mouseEnter() {
        this.style = "background-color: #888; ";
    }

    //function menuClick() {
    //    var me = this;
    //    alert(me.text);
    //}

    var $icon = $("#sidebarToggle i.fa");

    $("#sidebarToggle").on("click", function() {
        var $sidebarAndWrapper = $("#sidebar, #wrapper");
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });

    //$("#main").on("mouseenter", mouseEnter);
    //$("#main").on("mouseleave", mouseLeave);

})();

 