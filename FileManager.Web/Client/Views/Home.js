
define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'homeCollection',
  'text!../Templates/CreateResource.html'
], function ($, _, dust, Backbone,homeCollection, homeTemplate) {

    
    var homeView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            this.collection = homeCollection;
            this.collection.bind("reset", this.render, this);
            this.render();
        },
        render: function () {
            var dust_tag = homeTemplate;
            var fileData = {};
            fileData.files = this.collection.toJSON();
            var compiled = dust.compileFn(dust_tag, "tmp_home");
            dust.loadSource(compiled);
            dust.render("tmp_home", fileData, function (err, html_out) {
                $("#fileContainer").html(html_out);
            });
        }
    });
    return new homeView;
});