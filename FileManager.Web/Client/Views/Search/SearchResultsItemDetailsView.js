define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'views/search/SearchResultsItemEditView',
  'views/Home/AlertView',
  'text!../../Templates/Search/SearchResultsItemDetails.html'
], function ($, _, dust, Backbone,searchResultsItemEditView,alertView, searchResultsItemDetailsTemplate) {
    var searchResultsItemDetailsView = Backbone.View.extend({
        el: '#myModal',
        initialize: function () {
        },
        events: {
        },
        render: function (res) {
            var dust_tag = searchResultsItemDetailsTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_srcResultsItem");
            dust.loadSource(compiled);
            dust.render("tmp_srcResultsItem", res, function (err, html_out) {
                $("#searchResultsItemDetails").html(html_out);
            });

            this.userRating();
            this.searchResultItemEdit();
            this.deleteResource();
            this.addDownloadHistory();
            $('#myModal').modal({
                keyboard: true,
                backdrop: true
            }).modal('show');

            //  $(".searchResultItemEdit").hide();
            this.applyRolePermission();
        },
        addDownloadHistory: function()
        {
            $('#fDownloadDetails').click(function () {
                //this.preventDefault();
                var downloadhistory = {};
                downloadhistory.ResourceId =$(this).attr('data-id');
                
                var totaldownload =parseInt(  $('#totalDownload').text());
                totaldownload = totaldownload + 1;
                $('#totalDownload').text(totaldownload);

                $.post('/api/DownloadHistory', downloadhistory, function (res) {
                }, 'json');
            });
        },
        applyRolePermission:function()
        {
            $.get('/api/permission/GetUserPermission?origin=resourcedetail', null, function (res) {
                $("#fDownloadDetails").hide();
                $(".searchResultItemEdit").hide();
                $("#A3").hide();
                for (var i = 0; i < res.length; i++)
                {
                    if (res[i].PermissionName == "Download")
                    {
                        if (res[i].AllowRead) {
                            $("#fDownloadDetails").show();
                        }
                    }
                    else if (res[i].PermissionName == "Resource")
                    {
                        if (res[i].AllowRead) {
                            
                        }
                        if (res[i].AllowWrite) {
                            $(".searchResultItemEdit").show();
                        }
                        if (res[i].AllowDelete) {
                            $("#A3").show();
                        }
                    }
                }

            }, 'json');
        }
        ,
        searchResultItemEdit:function()
        {         
            $('#myModal .searchResultItemEdit').click(function () {
            var fileId = parseInt($(this).attr('data-Id'));   
            $.get('/api/files', { itemDetailsId: fileId }, function (res) {
                searchResultsItemEditView.render(res);
            }, 'json');
            });
        },
        userRating: function () {
            _.each($('.ratingArea'), function (item) {

                previousRating(item);
            });
            $(".rating").mouseover(function () {
                makeAllEmpty(this);
                giveRating($(this), "FilledStar.png");
                $(this).css("cursor", "pointer");
            });

            $(".rating").mouseout(function () {
                giveRating($(this), "EmptyStar.png");
            });
            $(".rating").parent().mouseout(function () {
                previousRating(this);
            });
            $(".rating").click(function () {
                $(this).parent().unbind("mouseout").children().unbind("mouseout mouseover click");
                // call ajax methods to update database
                var url = "/api/files/?rating=" + parseInt($(this).attr("id")) + "&fileId=" + parseInt($(this).siblings('#fileIdforRating').val());
                $.post(url, null, function (data) {

                });
            });

            function giveRating(img, image) {
                img.attr("src", "/img/" + image)
                   .prevAll("img").attr("src", "/img/" + image);
            }
            function previousRating(item) {
                var previousRating = $("#previousRating", item).val();
                var preId = "img[id=" + previousRating + "]";
                var previousRatingImage = $(preId, item);
                giveRating(previousRatingImage, "FilledStar.png");
            }
            function makeAllEmpty(item) {
                $(item).parent().children().attr("src", "/img/EmptyStar.png");
            }

        },
        deleteResource: function () {
            $('.resourceItemDelete').click(function () {
                var fileId = parseInt($(this).attr('data-id'));
                var preId = "img[data-id=" + fileId + "]";
                   $.ajax({
                        url: '/api/files?id=' + fileId,
                        type: 'DELETE',
                        success: function (data) {
                            alertView.render('Well done!', 'Resource deleted successfully.', 'alert-info');
                            $('#myModal').modal('hide');                            
                            $(preId).parents('.imageItemdiv').remove();
                           
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alertView.render('Oh snap!', 'Resource delete operation failed.', 'alert-error');
                        }
                    });
            });
        }

    });
    return new searchResultsItemDetailsView;
});