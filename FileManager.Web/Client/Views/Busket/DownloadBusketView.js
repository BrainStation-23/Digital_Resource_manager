define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/Busket/DownloadBusketCollection',
  'views/Home/AlertView',
  'text!../../Templates/Busket/DownloadBusket.html'
], function ($, _, dust, Backbone, downloadBusketCollection, alertView, downloadBusketTemplate) {
    var downloadBusketView = Backbone.View.extend({
        el: '#fileContainer',
        initialize: function () {
            this.collection = new downloadBusketCollection();
        },
        events: {
            'click .RemoveFromBasket': 'removeFromBasket',
            'click #downloadAll': 'downloadAllResource'

        },
        render: function (result) {
            var self = this

            var dust_tag = downloadBusketTemplate;
            var fileData = {};
            fileData.downloadBusket = result.BasketViewModelList;
            fileData.Zippath = result.Zippath;
            var compiled = dust.compileFn(dust_tag, "tmp_downloadBusket");
            dust.loadSource(compiled);
            dust.render("tmp_downloadBusket", fileData, function (err, html_out) {
                $("#fileContainer").html(html_out);
                self.showRemoveButton();
            });

            if (result.BasketViewModelList.length == 0)
            {
                $("#downloadAll").hide();
            }
        },
        showRemoveButton: function () {
            $('.imageItemdownloaddiv').hover(function () {
                $(this).children('.removeDownloadBusket').show();
            },
            function () {
                $(this).children('.removeDownloadBusket').hide();
            });
        },
        removeFromBasket: function (e) {
            e.preventDefault();
            e.stopPropagation();
            var self = this;
            var fileId = parseInt($(e.target).attr('data-id'));
            $.ajax({
                url: '/api/Basket?id=' + fileId,
                type: 'DELETE',
                success: function (data) {
                    var preId = "img[data-id=" + fileId + "]";
                    $(preId).parents('.imageItemdownloaddiv').remove();
                    alertView.render('Well done!', 'Removed from download busket.', 'alert-info');
                    $.get('/api/basket', null, function (res) {
                        self.render(res);
                    }, 'json');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alertView.render('Oh snap!', 'Not remove from download busket.', 'alert-error');
                }
            });
        },
        downloadAllResource: function (e)
        {
            var self = this;
            $.post('/api/Basket/PostDownloadComplete/?username=aa', null, function (res) {
                if (res != "")
                {
                    window.location.href = res;
                    $.get('/api/basket', null, function (res) {
                        self.render(res);
                    }, 'json');
                }
            }, 'json');
            
        }

    });
    return new downloadBusketView;
});