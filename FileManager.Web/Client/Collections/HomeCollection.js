define([
  'jquery', 
  'backbone',
  'models/HomeModel'
], function ($, Backbone, homeModel) {
    var homeCollection = Backbone.Collection.extend({
        model: homeModel,
        url: '/api/files',
        initialize: function () {
           // this.fetchData();
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

    return new homeCollection;
});