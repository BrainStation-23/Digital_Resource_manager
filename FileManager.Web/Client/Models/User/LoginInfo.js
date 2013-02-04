define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var logininfoModel = Backbone.Model.extend({
        url: '/api/user/',
        defaults: {
            emailAddress: "newemail@yahoo.com",
            password: "123456"
        },
        initialize: function () {
        }

    });
    return logininfoModel;

});