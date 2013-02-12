define([
  'jquery',
  'backbone'
], function ($, Backbone) {
    var downloadBusketCollection = Backbone.Collection.extend({
        url: '/api/files/GetTagCloud',
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