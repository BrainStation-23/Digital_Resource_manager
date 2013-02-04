define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/Category/CategoryCollection',
  'views/Home/AlertView',
  'text!../../Templates/Search/SearchResultsItemEdit.html'
], function ($, _, dust, Backbone, categoryCollection, alertView, searchResultsItemEditTemplate) {
    var searchResultsItemEditView = Backbone.View.extend({
        el: '#searchResultsItemDetails',
        initialize: function () {
            this.collection = new categoryCollection();
            //this.collection.fetch();
        },
        events: {
        },
        render: function (res) {
            var self = this;
            this.collection.fetch({
                success: function (collection, response) {
                    var dust_tag = searchResultsItemEditTemplate;
                    var compiled = dust.compileFn(dust_tag, "tmp_srcResultsItemEdit");
                    dust.loadSource(compiled);
            
                    res.categories = self.collection.toJSON();
                    dust.render("tmp_srcResultsItemEdit", res, function (err, html_out) {
                        $('#myModal').modal('hide');
                        $("#searchResultsItemDetails").empty().html(html_out);
                        self.getTagData();
                    });
                    $("#formFileUploaderEdit #allCategories").val(res.CategoryId);
                    self.updateItemDetails();
                    $('#myModalEdit').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');
                }
            });
            
        },
        updateItemDetails: function () {
            var self = this;
            $('#myModalEdit #btnUpdate').click(function () {
                /*var data = $('#formFileUploaderEdit').serialize();
                var fileId = parseInt($(this).attr('data-Id'));
                $('#uploadingfile,#thumbfile').upload('/api/files/UpdateResource', data, function (res) {
                    alert('File uploaded');
                }, 'json');*/

                var data = $('#formFileUploaderEdit').serializeArray();
                var fileId = $('#formFileUploaderEdit').children('#Id').val();
                
                var rank = $('#Rank').val();
                var formData = new FormData();
                _.each(data, function (item) {
                    formData.append(item.name, item.value);
                });
                var uploadingFile = document.getElementById('uploadingfile').files[0];
                var uploadingthumb = document.getElementById('thumbfile').files[0];
                formData.append('resource', uploadingFile);
                formData.append('thumb', uploadingthumb);

                var isValidRank = false;
                if (!self.isNumber(rank) || rank < 0 || rank > 5) {
                    $('#lblEditRkErr').html('Rank must between 0 to 5.');
                    isValidRank = false;
                }
                else {
                    isValidRank = true;
                }
                if (isValidRank) {
                    $.ajax({
                        url: '/api/files/UpdateResource',
                        data: formData,
                        processData: false,
                        contentType: false,
                        type: 'POST',
                        success: function (data) {
                            alertView.render('', "Resource updated successfully.", "alert-info");
                            $('#myModalEdit').modal('hide');
                            formProxy.trigger('searchitemDetailsShow', fileId);
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alertView.render('Oh snap!', 'Resource update failed.', 'alert-error');
                        }
                    });
                }
            });
        },
        getTagData: function () {
            function extractor(query) {
                var result = /([^,]+)$/.exec(query);
                if (result && result[1])
                    return result[1].trim();
                return '';
            }

            $('.typeahead').typeahead({
                source: function (query, process) {
                    var innerQuery = extractor(query);
                    return $.get('/api/files?chars=' + innerQuery, null, function (data) {
                        var querydata = query.split(',');
                        var processableData = _.difference(data, querydata);
                        return process(processableData);
                    });
                },
                updater: function (item) {
                    return this.$element.val().replace(/[^,]*$/, '') + item + ',';
                },
                matcher: function (item) {
                    var tquery = extractor(this.query);
                    if (!tquery) return false;
                    return ~item.toLowerCase().indexOf(tquery)
                },
                highlighter: function (item) {
                    var query = extractor(this.query).replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, '\\$&')
                    return item.replace(new RegExp('(' + query + ')', 'ig'), function ($1, match) {
                        return '<strong>' + match + '</strong>'
                    })
                }
            });
        },
        isNumber: function (n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }/*,
        showItemDetails: function () {
            
                var fileId = parseInt($(this).attr('data-Id'));
                $.get('/api/files', { itemDetailsId: fileId }, function (res) {
                    $('#myModalEdit').modal('hide');
                    searchResultItemDetails.render(res);
                }, 'json');
        }*/
    });
    return new searchResultsItemEditView;
});