'use strict';
(function (ko, $, window) {
    
    function getParameterByName(name) {
        var match = RegExp('[?&]' + name + '=([^&]*)')
                        .exec(window.location.search);
        return match ?
            decodeURIComponent(match[1].replace(/\+/g, ' '))
            : null;
    }

    var PostItem = (function() {
        return function(spec) {
            var self = this;
            spec = spec || { };

            self.title = spec.title || "";
        
            self.slug = spec.slug || "";
        
            self.id = spec.id || "";

            self.datePublished = ko.observable(spec.datePublished || "").extend({isoDate: "mm/dd/yyyy"});
        
            self.preview = spec.preview || "";
        
            self.dateCreated = ko.observable(spec.dateCreated || "").extend({isoDate: "mm/dd/yyyy"});

            self.type = spec.type || "";

            self.status = spec.status || "";

            self.navigateToPost = function() {
                window.location.href = self.url;
            };

            self.url = (function() {
                return "/admin/posts/edit?id={0}".format(self.id);
            })();

            return self;
        };
    })();
    
    var PostListViewModel = function(spec) {
        var self = this; spec = spec || { };

        self.posts = ko.observableArray([]);

        //#region AJAX Methods

        self.load = function(data) {
            $.getJSON("/ajax/posts/list",
                data || {
                    pg: getParameterByName("pg") || 1,
                    filter: getParameterByName("filter") || "all"
                },
                function(post) {

                    var arr = self.posts();
                    arr.push.apply(arr, $.map(post, function(el) { return new PostItem(el); }));
                    self.posts.valueHasMutated();
                });
        };

        //#endregion

        return self;
    };
    

    window.viewModel = new PostListViewModel();
    window.viewModel.load();
    ko.applyBindings(window.viewModel);
    
})(ko, jQuery, window);

