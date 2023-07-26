define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/History/HistoryCollection',
  'views/Home/AlertView',
  'text!../../Templates/History/DownloadHistory.html',
  'text!../../Templates/History/HistoryDetail.html',
  'views/History/DownloadDetailView'
], function ($, _, dust, Backbone, historyCollection, alertView, downloadHistoryTemplate, historyDetailTemplate, downloadDetailView) {
    var downloadHistoryView = Backbone.View.extend({
        el: '#fileContainer',
        initialize: function () {
            this.collection = new historyCollection();
        },
        events: {
            
            'click .DetailHistory': 'showDetailHistoryModal'
        },
        
        render: function () {
            var self = this
            this.collection.fetch({
                success: function (collection, response) {
                    var dust_tag = downloadHistoryTemplate;
                    var fileData = {};
                    fileData.resourcehistories = collection.toJSON();
                    var compiled = dust.compileFn(dust_tag, "tmp_category");
                    dust.loadSource(compiled);
                    dust.render("tmp_category", fileData, function (err, html_out) {
                        $("#fileContainer").empty().html(html_out);
                        self.bindSortableGrid();
                    });
                }
            });

        },
        getHistoryData: function () {
            this.render();
        },
        bindSortableGrid: function () {
            var table = $("#tableHistory").tablesorter({
                theme: 'blue',
                debug: false,
                sotList: [[0, 0]],
                widgets: ["filter"],
                widgetOptions: {
                    filter_columnFilters: false,
                    filter_saveFilters: false,
                    filter_reset: '.reset'
                }
            }).tablesorterPager({
                container: $("#pagerOneHistory")
            });

            $.tablesorter.filter.bindSearch(table, $('.search'), false);
        },
        showDetailHistoryModal: function (e) {
            var self = this;
            var fileData = {};

            var userId = $(e.target).attr('data-userId');
            var resourceId = $(e.target).attr('data-Id');
            var userIdAndRscId = userId + "~" + resourceId;
            $.get('/api/DownloadHistory/?userIdAndResourceId=' + userIdAndRscId, null, function (res) {
                downloadDetailView.render(res);
            }, 'json');
        }
       
    });
    return new downloadHistoryView;
});