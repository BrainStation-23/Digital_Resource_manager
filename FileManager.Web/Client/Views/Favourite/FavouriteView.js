define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/Favourite/FavouriteCollection',
  /*'views/search/SearchResultsItemDetailsView',*/
  'views/Home/AlertView',
  'text!../../Templates/favourite/Favourite.html'
], function ($, _, dust, Backbone,favouriteCollection,alertView, favouriteTemplate) {
    var favouriteView = Backbone.View.extend({
        el: '#fileContainer',
        initialize: function () {
            this.collection = new favouriteCollection();
        },
        events: {
            'click .RemoveFavourite': 'removeFromFavourite'

        },
        render: function () {

            var self = this
            this.collection.fetch({
                success: function (collection, response) {

                    var dust_tag = favouriteTemplate;
                    var fileData = {};
                    var res = collection.toJSON();
                    var root = location.protocol + '//' + location.host;
                    _.each(res, function (item) { item.root = root; });
                    fileData.favourites = res;
                    var compiled = dust.compileFn(dust_tag, "tmp_favourite");
                    dust.loadSource(compiled);
                    dust.render("tmp_favourite", fileData, function (err, html_out) {
                        $("#fileContainer").html(html_out);
                        self.showDownloadButton();
                        self.dragoutToFileSystem();
                    });
                }
            });

           
        },
        showDownloadButton: function () {
            $('.imageItemdiv').hover(function () {
                $(this).children('.fileLoaderdiv,.addFavouriteCart').show();
            },
            function () {
                $(this).children('.fileLoaderdiv,.addFavouriteCart').hide();
            });
        },
        removeFromFavourite: function (e) {
            e.preventDefault();
            e.stopPropagation();
            var self = this;
            var fileId = parseInt($(e.target).attr('data-id'));
            $.ajax({
                url: '/api/UserFavouriteResource?id=' + fileId,
                type: 'DELETE',
                success: function (data) {
                    alertView.render('Well done!', 'Removed from favourite list.', 'alert-info');
                    self.render();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alertView.render('Oh snap!', 'Not remove from favourite list.', 'alert-error');
                }
            });
        },
        dragoutToFileSystem: function () {
            var files = $('.downdrag');
            if (files.length > 0) {
                var use_data = (typeof files[0].dataset === "undefined") ? false : true;
                $(files).each(function () {
                    var url = use_data ? this.dataset.downloadurl : this.getAttribute("data-downloadurl");
                    this.addEventListener("dragstart", function (e) {
                        e.dataTransfer.setData("DownloadURL", url);
                    }, false);
                });
            }
        }

    });
    return new favouriteView;
});