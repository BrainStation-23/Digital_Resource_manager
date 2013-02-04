define([
  'jquery',
  'backbone',
  'models/ModelBase'
], function ($, Backbone, modelBase) {
    var historyCollection = Backbone.Collection.extend({
        model: modelBase,
        url: '/api/DownloadHistory',
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
    return historyCollection;
});