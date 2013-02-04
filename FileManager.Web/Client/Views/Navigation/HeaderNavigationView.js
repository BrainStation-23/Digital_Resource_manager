define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/Navigation/HeaderNavigationCollection',
  'text!../../Templates/Navigation/HeaderNavigation.html'

  
], function ($, _, dust, Backbone, headerNavigationCollection, headerNavigationTemplate) {
    var headerNavigationView = Backbone.View.extend({
        el:"#mainNav",
        initialize: function () {
            
        },

        events: {
        },

        render: function (usermenulist) {
            var menu = {};
            menu.menulist = usermenulist;
            var dust_tag = headerNavigationTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_headerNav");
            dust.loadSource(compiled);
            dust.render("tmp_headerNav", menu, function (err, html_out) {
                $("#mainNav").html(html_out);
            });
        }
        
    });
    return new headerNavigationView;
});