define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Models/GenericSearch/GenericSearchModel',
  /*'../../Collections/GenericSearch/GenericSearchCollection',*/
  'Views/GenericSearch/GenericSearchResultsView',
  'text!../../Templates/GenericSearch/GenericSearch.html'

], function ($, _, dust, Backbone, /*genericSearchCollection*/ genericSearchModel, genericSearchResultsView, genericSearchTemplate) {
    var genericSearchView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            //this.collection = new genericSearchCollection();
            //this.collection.bind("reset", this.render, this);
            this.model = new genericSearchModel();
            //this.render();
        },
        events:
        {
            'click #btnSearch': 'searchResults',
            /*'keyup #searchText': 'searchOnEnterKey',*/
            'keypress #searchText': 'searchOnEnterKey',
            'click .tags':'searchByTag'
            /*'change #allSorter': 'sortBy'*/
        },
        render: function () {
            var self = this;

            this.model.fetch({
                success: function (collection, response) {
                    var data = {};
                    data.Tags = response;
                    self.addTagStyle(data);
                    var dust_tag = genericSearchTemplate;
                    var compiled = dust.compileFn(dust_tag, "tmp_genericSearch");
                    dust.loadSource(compiled);
                    dust.render("tmp_genericSearch", data, function (err, html_out) {
                        $("#fileContainer").html(html_out);
                        self.searchResults(null);
                    });

                }
            });
        },
        searchResults: function (e) {
            if(e)
                e.preventDefault();
            var data = $('#searchForm').serialize();
            var tempdata = data + "&pageNumber=0";
            $.get('/api/files', tempdata, function (res) {
                genericSearchResultsView.render(res);
                genericSearchResultsView.sortByDate();
                genericSearchResultsView.addPagination(res.TotalPage);
                $('#TagSearch').val('');
            }, 'json');
        },
        searchOnEnterKey: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                this.searchResults(e);
            }
        },
        addTagStyle: function (data) {
            var totalResource = 0;
            $.each(data.Tags,function (index,item) {
                totalResource += item.NoOfResource;
            });
            var minFontSize = 10;
            var maxFontSize = 34;
            $.each(data.Tags,function (index,item) {
                var ratio = parseInt(item.NoOfResource)/ totalResource;
                var fontSize = Math.ceil((maxFontSize * ratio) + minFontSize);
                item.FontSize = fontSize;
                item.FontColor = Math.ceil(9 * ratio) + "cf";
            });
            
        },
        searchByTag: function (e) {
            e.preventDefault();
            var dataId = e.target.dataset.id;
            var tempdata = "tagName="+dataId + "&pageNumber=0";
            if (dataId != "") {
                $.get('/api/files', tempdata, function (res) {
                    $('#allSorter').val(0);
                    genericSearchResultsView.render(res);
                    genericSearchResultsView.sortByDate();
                    genericSearchResultsView.addPagination(res.TotalPage);
                    $('#TagSearch').val(dataId);
                }, 'json');
            }
        }
    });
    return new genericSearchView;
});