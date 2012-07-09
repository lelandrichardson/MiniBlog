'use strict';
window.MiniBlog = (function (spec, my) {
    //#region Public Functions

    self.styleCode = function () {
        /// <summary>
        /// uses google prettify library to prettify any code inside a "pre code" block
        /// </summary>
        var a = false;
        //        $("pre code").parent().each(function() {
        //            if (!$(this).hasClass("prettyprint")) {
        //                $(this).addClass("prettyprint");
        //                a = true;
        //            }
        //        });
        var allPre = document.getElementsByTagName('PRE');
        for (var i = 0; i < allPre.length; i++) {
            if (allPre[i].className.indexOf("prettyprint") == -1) {
                allPre[i].className += ' prettyprint';
                a = true;
            }
        }

        if (a) {
            prettyPrint();
        }
    };

    self.registerView = function (id) {
        /// <summary>
        /// calls /ajax/vc with id parameters to register a view in the database.
        /// </summary>
        /// <param name="id">the entity id</param>
        var beacon = new Image();
        beacon.src = '/ajax/vc' + (!!id ? '?p=' + id : '');
    };

    return self;
})();

