'use strict';
(function () {
    //#region Utility Prototype Methods

    // Utility Methods
    Function.prototype.method = function (name, func) {
        this.prototype[name] = func;
        return this;
    };
    
    //formats a Date object to "time ago" string
    Date.method('timeAgo', function () {
        var secs = (((new Date()).getTime() - this.getTime()) / 1000),
	days = Math.floor(secs / 86400);

        return days === 0 && (
				secs < 60 && "just now" ||
				secs < 120 && "a minute ago" ||
				secs < 3600 && Math.floor(secs / 60) + " minutes ago" ||
				secs < 7200 && "an hour ago" ||
				secs < 86400 && Math.floor(secs / 3600) + " hours ago") ||
			days === 1 && "yesterday" ||
			days < 31 && days + " days ago" ||
			days < 60 && "one month ago" ||
			days < 365 && Math.ceil(days / 30) + " months ago" ||
			days < 730 && "one year ago" ||
			Math.ceil(days / 365) + " years ago";
    });

    //formats a string (similar to c# string.Format)
    String.method('format', function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    });

    //returns a shortened formatted string (ie, 2700 -> "2.7k")
    Number.method('toShortString', function () {
        var num = this,
	round = function (theNumber, digits) {
	    return Math.round(theNumber * Math.pow(10, digits)) / Math.pow(10, digits);
	};
        if (num < 1000) {
            return "" + num;
        } else if (num < 10000) {
            return round(num / 1000, 1) + "k";
        } else {
            return round(num / 1000, 0) + "k";
        }
    });
    
    //#endregion
})();

var MiniBlogApp = function (spec, my) {

    //#region Create Objects

    //object to return.  publicly scoped variables
    var self = {};

    //object for private scoped variables
    my = my || {};

    //configure local jQuery object
    var $ = spec.$;
    self.$ = $;

    self.extend = function (extendor, extendedSpec, extendedMy) {
        extendor(
			self,
			$.extend(spec, extendedSpec || {}),
			$.extend(my, extendedMy || {})
		);
    };



    //#endregion

    //#region Initialize Properties
    self.ajaxUrl = spec.ajaxUrl || "/ajax";

    my.enableStyleCode = spec.enableStyleCode || true;
    my.styleOnLoad = spec.styleOnLoad || true;
    
    //#endregion

    //#region Private Functions (for initialization?)
    
    //#region Public Functions
    
    self.styleCode = function () {
        /// <summary>
        /// uses google prettify library to prettify any code inside a "pre code" block
        /// </summary>
        if (!my.enableStyleCode) {
            return;
        }
        var a = false;

        $("pre code").parent().each(function () {
            if (!$(this).hasClass("prettyprint")) {
                $(this).addClass("prettyprint");
                a = true;
            }
        });

        if (a) { prettyPrint(); }
    };

    self.api = function (opts) {
        /// <summary>
        /// Executes AJAX request to TP API...
        /// opts = {
        ///     url: "/questions/new", //required
        ///     data: {payload-or-url-data},
        ///     method: "POST",
        ///     formEncode: false,
        ///     success: function(data, status, jqXHR) { ... },
        ///     timeout: milliseconds-to-timeout,
        ///     complete: function(jqXHR) { ... },
        ///     error: function(jqXHR,status,error) { ... }
        /// }
        /// </summary>
        if (!opts.url) {
            return;
        }
        var options = {
            url: self.apiRootUrl + opts.url,
            type: opts.method || opts.hasOwnProperty("data") ? "POST" : "GET",
            processData: opts.formEncode || false,
            contentType: (opts.formEncode || false) ? "application/x-www-form-urlencoded" : "application/json"
        };
        if (opts.hasOwnProperty("data")) {
            options.data = options.processData ? opts.data : JSON.stringify(opts.data);
            delete opts.data;
        }
        delete opts.url;
        $.extend(options, opts);
        $.ajax(options);
    };

    self.registerView = function (id) {
        /// <summary>
        /// calls /ajax/vc with id parameters to register a view in the database.
        /// </summary>
        /// <param name="id">the entity id</param>
        var url = !!id ? "{0}/vc?p={1}".format(self.ajaxUrl, id) : "{0}/vc".format(self.ajaxUrl);
        $.get(url);
    };

    self.slug = function (text) {
        /// <summary>
        /// creates an seo-slug from the input text parameter
        /// </summary>
        var remHyphenRegex = /( ?- ?)|[ ']/g;
        var remRegex = /[^0-9a-zA-Z-]/g;

        return text.replace(remHyphenRegex, "-").replace(remRegex, "").trim().toLowerCase();
    };

    self.init = function () {
        if (my.styleOnLoad) {
            self.styleCode();
        }

        $(function () {
            $.each(window.tprq || [], function (idx, el) {
                el();
            });
            my.readyHasFired = true;
        });
    };

    //#endregion

    return self;
};

