define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'text!../../Templates/Home/Alert.html'
], function ($, _, dust, Backbone, alertTemplate) {
    var alertView = Backbone.View.extend({
        el: $(".alert"),
        initialize: function () {
        },
        render: function (header, msg, alertClass) {
            var dust_tag = alertTemplate;
            var data = { msgHeader: header, msgbody: msg, altclass:alertClass };
            var compiled = dust.compileFn(dust_tag, "tmp_alert");
            dust.loadSource(compiled);
            dust.render("tmp_alert", data, function (err, html_out) {
                $("#alertMsg").html(html_out);
            });
        }
    });
    return new alertView;
});