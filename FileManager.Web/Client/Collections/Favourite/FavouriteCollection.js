define([
  'jquery',
  'backbone',
  'models/Favourite/FavouriteModel'
], function ($, Backbone, favouriteModel) {
    var favouriteCollection = Backbone.Collection.extend({
        model: favouriteModel,
        url: '/api/UserFavouriteResource',
        initialize: function () {
            
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
    return favouriteCollection;
});