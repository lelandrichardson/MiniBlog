'use strict';
(function (ko, $, window) {

    var EditViewModel = function(spec) {
        var self = this; spec = spec || { };

        //#region Data Fields

        self.title = ko.observable(spec.title || "");
        
        self.slug = ko.observable(spec.slug || "");
        
        self.id = ko.numericObservable(spec.id || 0);

        self.publishDate = ko.observable(spec.publishDate || "").extend({isoDate: "mm/dd/yyyy"});
        
        self.body = ko.observable(spec.body || "");
        
        self.dateCreated = ko.observable(spec.dateCreated || "").extend({isoDate: "mm/dd/yyyy"});

        self.tags = ko.observableArray([]);

        self.tagInput = ko.observable("");

        self.addTag = function() {
            self.tags.push(self.tagInput());
            self.tagInput("");
        };

        self.removeTag = function(data) {
            self.tags.remove(data);
        };

        self.meta = ko.observableArray([]);

        self.status = ko.observable("");

        self.type = ko.observable("B");
        self.typeOptions = [
            {
                label: "Blog Post", 
                id: "B"
            },
            {
                label: "Page", 
                id: "P"
            }
        ];

        self.allowComments = ko.observable(true);
        self.includeSocial = ko.observable(false);

        //#endregion
        
        //#region Utility Fields

        self.dirtyFlag = ko.dirtyFlag([
                self.title,
                self.slug,
                self.id,
                self.body,
                self.dateCreated,
                self.tags,
                self.meta,
                self.type,
                self.allowComments,
                self.includeSocial
            ]);
        
        self.bodyDirtyFlag = ko.dirtyFlag([self.body]);


        var getSlug = function (text) {
            /// <summary>
            /// creates an seo-slug from the input text parameter
            /// </summary>
            var remHyphenRegex = /( ?- ?)|[ ']/g;
            var remRegex = /[^0-9a-zA-Z-]/g;

            return text.replace(remHyphenRegex, "-").replace(remRegex, "").trim().toLowerCase();
        };

        self.updateSlug = function() {
            if(self.isSlugBound()) {
                self.slug(getSlug(self.title()));
            }
        };

        self.slugKeyDown = function() {
            self.isSlugBound(false);
        };

        self.isSlugBound = ko.observable(true);
        self.isSlugBound.subscribe(function() {
            //self.updateSlug();
        });

        self.allowSave = ko.dependentObservable(function() {
            return self.slug().length > 0 && self.title().length > 0;
        });

        self.save = function() {
            if(self.id() > 0) {
                self.saveAll();
            } else {
                self.create();
            }
        };
        
        self.publishOrUnpublish = ko.dependentObservable(function() {
            return self.status() == "A" ? "Unpublish" : "Publish";
        });


        var isTypingTimeout;
        self.body.subscribe(function() {
            clearTimeout(isTypingTimeout);
            isTypingTimeout = setTimeout(function() {
                self.saveBody();
            },2000);
        });



        //#endregion
        
        //#region AJAX Methods

        self.saveBody = function() {
            if(self.status() == "A" || self.id() === 0) {
                return;
            }

            $.ajax({
                url: "/ajax/posts/update",
                type: "POST",
                contentType: "application/json",
                processData: false,
                data: JSON.stringify({
                    method: "save-body",
                    data: {
                        id: self.id(),
                        body: self.body()
                    }
                }),
                success: function (response) {
                    if(response) {
                        //alert(JSON.stringify(response));
                        self.bodyDirtyFlag.reset();
                    }
                }
            });
        };

        self.create = function() {
            $.ajax({
                url: "/ajax/posts/insert",
                type: "POST",
                contentType: "application/json",
                processData: false,
                data: JSON.stringify({
                    method: "insert",
                    data: {
                        title: self.title(),
                        slug: self.slug(),
                        status: self.status(),
                        type: self.type(),
                        body: self.body(),
                        allowComments: self.allowComments(),
                        includeSocial: self.includeSocial(),
                        tags: self.tags()
                    }
                }),
                success: function (response) {
                    if(response) {
                        self.id(response.data || 0);
                        self.dirtyFlag.reset();
                        self.bodyDirtyFlag.reset();
                        //alert(JSON.stringify(response));
                    }
                }
            });
        };

        self.load = function(id) {
            self.id(id);
            $.getJSON("/ajax/posts/get?id=" + id, function(post) {
                self.title(post.title);
                self.slug(post.slug);
                self.body(post.body);
                self.status(post.status);
                self.type(post.type);
                self.dateCreated(post.dateCreated);
                self.publishDate(post.publishDate);
                self.allowComments(post.allowComments);
                self.includeSocial(post.includeSocial);
                self.tags(post.tags);

                self.isSlugBound((self.slug() === getSlug(self.title())));
                
                self.dirtyFlag.reset();
                self.bodyDirtyFlag.reset();
            });
        };

        self.saveAll = function() {
            $.ajax({
                url: "/ajax/posts/update",
                type: "POST",
                contentType: "application/json",
                processData: false,
                data: JSON.stringify({
                    method: "save-all",
                    data: {
                        id: self.id(),
                        title: self.title(),
                        slug: self.slug(),
                        status: self.status(),
                        type: self.type(),
                        body: self.body(),
                        allowComments: self.allowComments(),
                        includeSocial: self.includeSocial(),
                        tags: self.tags()
                    }
                }),
                success: function (response) {
                    if(response) {
                        //alert(JSON.stringify(response));
                        self.dirtyFlag.reset();
                        self.bodyDirtyFlag.reset();
                    }
                }
            });
        };

        self.publish = function() {
            $.getJSON("/ajax/posts/publish?id=" + self.id(), function(response) {
                if(response && response.success === 1) {
                    self.publishDate(response.data.publishDate);
                    self.status(response.data.status);
                }
            });
        };

        self.preview = function() {
            window.open("/post.cshtml?id={0}".format(self.id()));
        };

        //#endregion

        return self;
    };
    

    window.viewModel = new EditViewModel();
    ko.applyBindings(window.viewModel);
    
})(ko, jQuery, window);

