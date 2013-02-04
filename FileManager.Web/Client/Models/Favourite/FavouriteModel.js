define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var favouriteModel = Backbone.Model.extend({
        url: '/api/UserFavouriteResource',
        defaults: {
           
        },
        initialize: function () {
        }

    });
    return favouriteModel;

});