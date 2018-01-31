$(function () {

    // define tour
    var tour = new Tour({
        debug: true,
        basePath: location.pathname.slice(0, location.pathname.lastIndexOf('/')),
        steps: [{
            path: "/index",
            element: "#badgefooter",
            title: "Title of my step",
            content: "Content of my step"
        }, {
                path: "/index",
            element: "#badgefooter",
            title: "Title of my step",
            content: "Content of my step"
        }]
    });

    // init tour
    tour.init();

    // start tour
    $('#start-tour').click(function () {
        tour.restart();
    });


});
