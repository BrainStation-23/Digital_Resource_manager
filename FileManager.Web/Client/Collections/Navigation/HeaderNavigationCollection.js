define([
  'jquery',
  'backbone',
  '../../Models/Navigation/HeaderNavigationModel'
], function ($, Backbone, headerNavigationModel) {
    var HeaderNavigationCollection = Backbone.Collection.extend({
        model: headerNavigationModel,
        /*url: '/api/files',*/
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

    return new HeaderNavigationCollection;
});