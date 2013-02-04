define([
    'jquery',
  'underscore',
  'backbone'
], function ($,_,Backbone) {
    var HomeModel = Backbone.Model.extend({
        defaults: {
            Name:null,
            type:null
        },
        initialize: function () {

        }
    });
    return HomeModel;

});