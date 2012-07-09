'use strict';
(function (ko, $, window) {

    var Setting = (function() {

        var typeLookup = {
            'S': "string",
            'D': "double",
            'I': "int",
            'B': "bool"
        };
        return function(spec) {
            var self = this;
            spec = spec || { };

            self.name = ko.observable(spec.name || "");
            self.description = ko.observable(spec.description || "");
            self.type = ko.observable(spec.type || "S");
            self.value = ko.observable(spec.value || "");

            self.displayType = ko.dependentObservable(function() {
                return typeLookup[self.type()];
            });

            self.dirtyFlag = ko.dirtyFlag(self);

            self.isValid = ko.dependentObservable(function() {
                return self.name().length > 0
                    && self.description().length > 0
                    && typeLookup.hasOwnProperty(self.type())
                    && self.value().length > 0;
            });

            return self;
        };
    })();
    
    var SettingsViewModel = function(spec) {
        var self = this; spec = spec || { };

        self.settings = ko.observableArray([]);

        self.newSetting = ko.observable(new Setting());

        //#region AJAX Methods

        self.load = function() {
            $.getJSON("/ajax/settings/list", function(post) {

                var arr = self.settings();
                arr.push.apply(arr, $.map(post, function(el) { return new Setting(el); }));
                self.settings.valueHasMutated();

            });
        };

        self.update = function(setting) {
            $.ajax({
                url: "/ajax/settings/update",
                type: "POST",
                contentType: "application/json",
                processData: false,
                data: JSON.stringify({
                    method: "update",
                    data: {
                        name: setting.name(),
                        type: setting.type(),
                        value: setting.value()
                    }
                }),
                success: function (response) {
                    if(response && response.success) {
                        setting.dirtyFlag.reset();
                    }
                }
            });
        };

        self.remove = function(setting) {
            if(confirm("Are you sure you want to delete {0}? This cannot be undone.".format(setting.name()))) {
                $.get("/ajax/settings/remove?name={0}".format(setting.name()), function (response) {
                    if(response) {
                        self.settings.remove(setting);
                    }
                });
            }
        };

        self.addSetting = function() {
            $.ajax({
                url: "/ajax/settings/insert",
                type: "POST",
                contentType: "application/json",
                processData: false,
                data: JSON.stringify({
                    method: "update",
                    data: {
                        name: self.newSetting().name(),
                        type: self.newSetting().type(),
                        description: self.newSetting().description(),
                        value: self.newSetting().value()
                    }
                }),
                success: function (response) {
                    if(response && response.success) {
                        self.newSetting().dirtyFlag.reset();
                        self.settings.push(self.newSetting());
                        self.newSetting(new Setting());
                    }
                }
            });
        };
        
        self.refreshCache = function(setting) {
            $.get("/ajax/settings/flush", function (response) {
                    if(response) {
                        alert(JSON.stringify(response));
                    }
                }
            );
        };

        //#endregion

        return self;
    };
    

    window.viewModel = new SettingsViewModel();
    window.viewModel.load();
    ko.applyBindings(window.viewModel);
    
})(ko, jQuery, window);

