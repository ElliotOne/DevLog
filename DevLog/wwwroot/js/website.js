$(function () {

    /*======================================================
                           Active AOS
    ======================================================*/

    AOS.init({
        once: true
    });

    /*======================================================
                           Active Jarallax
    ======================================================*/

    if ($.fn.jarallax) {
        $('.jarallax').jarallax({
            speed: 0.5
        });
    }

    /*======================================================
               Top Scroll Button Start
    ======================================================*/

    var browserWindow = $(window);
    browserWindow.scroll(function () {
        var GotoTop = $(window).scrollTop();
        if (GotoTop >= 400) {
            $("#btn-go-top").slideDown(500);
        } else {
            $("#btn-go-top").slideUp(500);
        }

    });

    /*======================================================
            Top Scroll Button End
    ======================================================*/

});

var Website = (function () {


    //===================================
    //          Text Reduce
    //===================================
    var textReduce = function shave(textSelector, numberOfChars) {
        return $(textSelector).shave(`${numberOfChars}`);
    }

    //===================================
    //          Water effect
    //===================================
    var waterEffect = function water(elementSelector) {
        $(elementSelector).ripples();
    }

    //===================================
    //          Json Result Handler
    //===================================
    var jsonResultHandler = function jsonHandler(jsonResult) {

        if (jsonResult !== undefined && jsonResult !== null) {

            //Not found
            if (jsonResult.statusCode === 404) {
                window.location = "/Error/404";
            }
            //Success
            else if (jsonResult.statusCode === 200) {
                $.alert({
                    title: '',
                    content: `<p>${jsonResult.message}</p>`,
                    type: 'green',
                    typeAnimated: true,
                    buttons: {
                        Ok: {
                            text: "close"
                        }
                    }
                });
            }
            //Error range 400 to 600
            else if ((jsonResult.statusCode >= 400 && jsonResult.statusCode < 600) && jsonResult.url === null) {
                $.alert({
                    title: '',
                    content: `<p>${jsonResult.message}</p>`,
                    type: 'red',
                    typeAnimated: true,
                    buttons: {
                        Ok: {
                            text: "close"
                        }
                    }
                });
            }
            //Redirect
            else if (jsonResult.statusCode === 301 || jsonResult.statusCode === 302)
                window.location = jsonResult.url;
        }
    }


    return {
        TextReduce: textReduce,
        WaterEffect: waterEffect,
        JsonResultHandler: jsonResultHandler
    }
})();
