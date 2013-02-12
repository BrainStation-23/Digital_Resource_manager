define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var genericSearchModel = Backbone.Model.extend({
        url: '/api/files/GetTagCloud',
       
        initialize: function () {
        }
    });
    return genericSearchModel;

});