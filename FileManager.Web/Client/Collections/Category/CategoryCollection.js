define([
  'jquery',
  'backbone',
  'models/Category/CategoryModel'
], function ($, Backbone, categoryModel) {
    var categoryCollection = Backbone.Collection.extend({
        model:categoryModel,
        url: '/api/categories',
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
    return categoryCollection;
});