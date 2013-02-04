// Filename: app.js
define([
  'jquery',  
  'underscore',
  'dust',
  'backbone',
  'router'
], function ($, _, dust, Backbone, Router) {
    var initialize = function () {
        _.extend(formProxy, Backbone.Events);
        // Pass in our Router module and call it's initialize function
        Router.initialize();
    }

    return {
        initialize: initialize
    };
});
