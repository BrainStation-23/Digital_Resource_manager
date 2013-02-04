define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var forgotPasswordModel = Backbone.Model.extend({
        urlRoot: '/api/user',
        url: function () {
            var url = this.urlRoot;
            if (this.get("email")) {
                url = url + "?email=" + this.get("email");
            }
            return url;
        },
        defaults: {
            email: ''
        },
        initialize: function () {
        }

    });
    return new forgotPasswordModel;

});