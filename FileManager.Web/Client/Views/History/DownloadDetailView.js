define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'views/Home/AlertView',
  'text!../../Templates/History/HistoryDetail.html'

], function ($, _, dust, Backbone, alertView, historyDetailTemplate) {
    var downloadDetailView = Backbone.View.extend({
        el: $("#historyDetail"),
        initialize: function () {
        },
        events:
        {
            
        },
        
        render: function (resData) {
            var self = this;
            var data = {};
            data.resourceDetailhistories = resData;
            var dust_tag = historyDetailTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_historydetail");
            dust.loadSource(compiled);
            dust.render("tmp_historydetail", data, function (err, html_out) {
                $("#historyDetail").html(html_out);
                $('#historyDetailModal').modal({
                    keyboard: true,
                    backdrop: true
                }).modal('show');
                self.bindSortableGrid();
            });
        },
        bindSortableGrid: function () {
            $("#tableHistoryDetail").tablesorter({
                debug: false,
                sortList: [[[0, 0]]]
            }).tablesorterPager({
                container: $("#pagerOneHistoryDetail"),
                positionFixed: false
            });
        }
        
    });

    return new downloadDetailView;
});