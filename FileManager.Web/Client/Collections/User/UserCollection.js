define([
  'jquery',
  'backbone',
  'models/User/UserModel'
], function ($, Backbone, userModel) {
    var userCollection = Backbone.Collection.extend({
        model: userModel,
        url: '/api/Membership',
        initialize: function () {
             //this.fetchData();
        },
        fetchData: function () {
            this.fetch({
                success: function (collection, response) {

                },
                error: function (collection, response) {

                }
            });
        },
    });
    return new userCollection;
});