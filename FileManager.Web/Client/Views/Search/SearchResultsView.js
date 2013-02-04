define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'Views/Search/SearchResultsItemDetailsView',
  'text!../../Templates/Search/SearchResults.html',
  'Views/Home/AlertView'
  
], function ($, _, dust, Backbone, searchResultsItemDetailsView, searchResultsTemplate, alertView) {
    var searchResultsView = Backbone.View.extend({
        el: '#fileContainer',
        SearchData: {},
        initialize: function () {
            //this.collection = categoryCollection;
            //var self = this;
            //this.collection.bind("reset", this.render, this);
            // this.render();
            // _.extend(formProxy, Backbone.Events);
            //formProxy.bind('searchitemDetailsShow', this.searchitemDetailsShow, this);
        },
        events: {
            'click .downdrag': 'searchitemDetails',
            'click .addFavourite': 'addToFavourite',

            'click #fDownload': 'addDownloadHistorySearch',
            'click .addtodownload': 'addtodownload',
            'click #RankUp': 'sortByRankAsc',
            'click #RankDown': 'sortByRankDsc',
            'click #DownloadUp': 'sortByDownloadAsc',

            'click #DownloadDown': 'sortByDownloadDsc'

            /*'mouseenter .imageItemdiv': 'showDownloadButton',
            'mouseleave .imageItemdiv': 'hideDownloadButton'*/
        },
        render: function (res) {
            var dust_tag = searchResultsTemplate;
            var fileData = {};
            var searchText = JSON.stringify(res.SearchResult, null, 2);
            this.SearchData.SearchResult = JSON.parse(searchText);
            this.SearchData = res;
            var root = location.protocol + '//' + location.host;
            _.each(res.SearchResult, function (item) { item.root = root; });
            
            fileData.searchResults = res.SearchResult;
            var compiled = dust.compileFn(dust_tag, "tmp_srcResults");
            dust.loadSource(compiled);
            dust.render("tmp_srcResults", fileData, function (err, html_out) {
                $("#searchResults").html(html_out);
            });
            this.showDownloadButton();
            this.userRating();       
            this.dragoutToFileSystem();
            $('#sorter').show();
            if (fileData.searchResults.length < 2)
                this.hideSortingOptions();
            //this.dragndrop();
            this.applyRolePermission();
        },
        hideSortingOptions : function()
        {
            $('#RankUp').hide();
            $('#RankDown').hide();
            $('#DownloadUp').hide();
            $('#DownloadDown').hide();
        },

        addDownloadHistorySearch: function (e) {
            var downloadhistory = {};
            downloadhistory.ResourceId = $(e.currentTarget).attr('data-id');
            
            $.post('/api/DownloadHistory', downloadhistory, function (res) {
            }, 'json');
        },

        searchitemDetails:function(e)
        {
            e.preventDefault();
            e.stopPropagation();
            var fileId = parseInt($(e.target).attr('data-id'));            
            this.searchitemDetailsShow(fileId);
            
        },
        showDownloadButton: function ()
        {
            $('.imageItemdiv').hover(function () {
                $(this).children('.fileLoaderdiv,.addFavouriteCart,.addtodownload').show();
            },
            function () {
                $(this).children('.fileLoaderdiv,.addFavouriteCart,.addtodownload').hide();
            });
        },
        addToFavourite:function(e)
        {
            e.preventDefault();
            e.stopPropagation();
            var fileId = parseInt($(e.target).attr('data-id'));
            $.post('/api/UserFavouriteResource?id=' + fileId, null, function (res) {
                alertView.render('Well done!', 'Added in favourite list.', 'alert-info');
                $(e.target).removeClass("icon-star  icon-white").addClass("icon-yellow");
                $(e.currentTarget).attr("title", "In Favourite List");
            }, 'json');
        }, 
        addtodownload: function (e) {
            e.preventDefault();
            e.stopPropagation();
            var basket = {};
            basket.ResourceId = $(e.currentTarget).attr('data-id');
            $.post('/api/Basket', basket, function (res) {
                alertView.render('Well done!', 'Resource added to download basket.', 'alert-info');
            }, 'json');
        },
        searchitemDetailsShow: function (fileId)
        {
            $.get('/api/files', { itemDetailsId: fileId }, function (res) {
                searchResultsItemDetailsView.render(res);
            }, 'json');
        },
        sortByRankAsc: function (e)
        {
            var self = this;
            $('#RankDown').show();
            $('#RankUp').hide();
            this.SearchData.SearchResult.sort(function (objA, objB) {
                if (objA.UserRating == 0)
                    return 1;//objB.UserRating;
                else if (objB.UserRating == 0)
                    return -1;//objA.UserRating;
                else
                    return ((objB.UserRating / objB.RatingCount) - (objA.UserRating / objA.RatingCount));
            });
            this.render(this.SearchData);
        },        
        sortByRankDsc: function (e) {
            var self = this;
            $('#RankDown').hide();
            $('#RankUp').show();
            this.SearchData.SearchResult.sort(function (objA, objB) {
                if (objA.UserRating == 0)
                    return -1;//objA.UserRating;
                else if (objB.UserRating == 0)
                    return 1;//objB.UserRating;
                else
                    return ((objA.UserRating / objA.RatingCount) - (objB.UserRating / objB.RatingCount));
            });
            this.render(this.SearchData);
        },
        sortByDownloadAsc: function (e) {
            var self = this;
            $('#DownloadDown').show();
            $('#DownloadUp').hide();
            this.SearchData.SearchResult.sort(function (objA, objB) {
                 return (parseInt(objB.DownloadCount,10) - parseInt(objA.DownloadCount,10));
            });
            this.render(this.SearchData);
        },
        sortByDownloadDsc: function (e) {
            var self = this;
            $('#DownloadDown').hide();
            $('#DownloadUp').show();
            this.SearchData.SearchResult.sort(function (objA, objB) {
                return (parseInt(objA.DownloadCount,10) - parseInt(objB.DownloadCount,10));
            });
            this.render(this.SearchData);
        },
        addPagination: function (totalPage) {
            var self = this;
            totalPage = parseInt(totalPage, 10);
            $('#searchResultsPagination').empty().bootpag({
                total: totalPage,
                page: 1,
                maxVisible: 10
            }).on('page', function (event, num) {
                num -= 1;
                var data = $('#searchForm').serialize();
                var tempdata = data + ("&pageNumber=" + num);
                $.get('/api/files', tempdata, function (res) {
                    self.render(res);
                }, 'json');
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
        applyRolePermission: function () {
            $.get('/api/permission/GetUserPermission?origin=resourcedetail', null, function (res) {
                var dowloadIcon = $(".fDownload");
                dowloadIcon.hide();
                for (var i = 0; i < res.length; i++) {
                    if (res[i].PermissionName == "Download") {
                        if (res[i].AllowRead) {
                            dowloadIcon.show();
                        }
                    }
                }
            }, 'json');
        }

        //dragndrop: function () {
            
        //    $(".downdrag").draggable({
        //        revert: true
        //    });
        //    $("#drophere").droppable({
        //        tolerance: 'touch',
        //        over: function () {
        //            alert("over");
        //            $(this).removeClass('out').addClass('over');
        //        },
        //        out: function () {
        //            alert("out");
        //            $(this).removeClass('over').addClass('out');
        //        },
        //        drop: function () {
        //            alert("drop");
        //            var answer = confirm('Permantly delete this item?');
        //            $(this).removeClass('over').addClass('out');
        //        }
        //    });
        //}

    });
    return new searchResultsView;
});