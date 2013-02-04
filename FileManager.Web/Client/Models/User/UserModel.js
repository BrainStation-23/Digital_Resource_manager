define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var userModel = Backbone.Model.extend({
        idAttribute: "Username",
        urlRoot: '/api/Membership',
        url: function () {
            var url = this.urlRoot;//+ this.id;
            if (this.get("Username")) {
                url = url + "?username=" + this.get("Username");
            }
            return url;
        },
        initialize: function () {
        }

    });
    return userModel;

});