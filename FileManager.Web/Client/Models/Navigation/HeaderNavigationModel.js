define([
    'jquery',
  'underscore',
  'backbone'
], function ($, _, Backbone) {
    var HeaderNavigationModel = Backbone.Model.extend({
        defaults: {
            Name: null,
            Link: null
        },
        initialize: function () {

        }
    });
    return HeaderNavigationModel;

});