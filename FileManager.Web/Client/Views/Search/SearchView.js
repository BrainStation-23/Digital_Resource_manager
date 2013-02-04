define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'Views/Search/SearchResultsView',
  'text!../../Templates/Search/Search.html'

], function ($, _, dust, Backbone, searchResultsView, searchTemplate) {
    var searchView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            //this.collection = categoryCollection;
            //this.collection.bind("reset", this.render, this);
            //this.render();
        },
        events:
        {
            'click #btnSearch': 'searchResults',
            /*'keyup #searchText': 'searchOnEnterKey',*/
            'keypress #searchText': 'searchOnEnterKey'
        },
        render: function () {
            var dust_tag = searchTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_search");
            dust.loadSource(compiled);
            dust.render("tmp_search", null, function (err, html_out) {
                $("#fileContainer").html(html_out);
            });
        },
        searchResults: function (e) {
            e.preventDefault();
            var data = $('#searchForm').serialize();
            var tempdata = data + "&pageNumber=0";
            $.get('/api/files', tempdata, function (res) {
                searchResultsView.render(res);
                searchResultsView.addPagination(res.TotalPage);
            }, 'json');        
        },
        searchOnEnterKey: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                this.searchResults(e);
            }
        }
    });
    return new searchView;
});