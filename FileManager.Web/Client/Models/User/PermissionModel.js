define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var permissionModel = Backbone.Model.extend({
        idAttribute: "UserRole",
        url: function () {
            var url = this.urlRoot;
            if (this.get("UserRole")) {
                url = url + "?roleName=" + this.get("UserRole");
            }
            return url;
        },
        urlRoot: '/api/permission',
        defaults: {
        },
        initialize: function () {
        }

    });
    return permissionModel;

});