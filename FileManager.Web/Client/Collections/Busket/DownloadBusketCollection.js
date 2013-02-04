define([
  'jquery',
  'backbone',
  'models/Favourite/FavouriteModel'
], function ($, Backbone, favouriteModel) {
    var downloadBusketCollection = Backbone.Collection.extend({
        model: favouriteModel,
        url: '/api/Basket',
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
    return downloadBusketCollection;
});