
define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../Collections/Category/CategoryCollection',
  'views/Home/AlertView',
  'text!../Templates/CreateResource.html'
], function ($, _, dust, Backbone, categoryCollection,alertView, createResourceTemplate) {
    var createResourceView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            this.collection = categoryCollection;
            this.collection.bind("reset", this.render, this);
            //this.render();
        },
        events:
        {
            'click #btnSaveforPost': 'postWithFile',
            'change #uploadingfile': 'clearResourceFileError',
            'change #thumbfile': 'clearThumbError',
            'change #Title': 'clearTitleError'
        },
        clearResourceFileError: function (e)
        {
            e.preventDefault();
            $("#lbluploadingfile").html("");
        },
        clearThumbError: function (e) {
            e.preventDefault();
            $("#lblthumbfile").html("");
        },
        clearTitleError: function (e) {
            e.preventDefault();
            $("#lblTitleError").html("");
        },
        render: function () {
            var dust_tag = createResourceTemplate;
            var fileData = {};
            fileData.files = this.collection;
            var compiled = dust.compileFn(dust_tag, "tmp_home");
            dust.loadSource(compiled);
            dust.render("tmp_home", fileData, function (err, html_out) {
                $("#fileContainer").html(html_out);
            });

            
            this.getTagData();
        },
        postWithFile:function(e)
        {
            e.preventDefault();
            var self = this;
            var data = $('#formFileUploader').serializeArray();
            var formData = new FormData();
            _.each(data, function (item) {
                formData.append(item.name, item.value);
            });
            var uploadingFile = document.getElementById('uploadingfile').files[0];
            var uploadingthumb = document.getElementById('thumbfile').files[0];
            formData.append('resource', uploadingFile);
            formData.append('thumb', uploadingthumb);

            var title = $('#Title').val().trim();
            var rank = $('#Rank').val();
            if (title == "")
            {
                $("#lblTitleError").html("please insert title");
            }
            if (!uploadingFile) {
                $("#lbluploadingfile").html("please select resource file");
            }
            if (!uploadingthumb) {
                $("#lblthumbfile").html("please select thumb file");
            }
            var isValidRank = false;
            if (!this.isNumber(rank) || rank > 5 || rank < 0) {

                $("#lblRankError").html("Rank must between 0 to 5.");
                isValidRank = false;
            }
            else {
                isValidRank = true;
            }
            
            /*TODO fix file upload issue here.*/
            if (title != "" && uploadingFile && uploadingthumb && isValidRank) {
                $('#formFileUploader').append('<div class="modal-backdrop fade in loading"></div><img class="loading" src="/img/loading.gif" style="position: absolute;top: 60%;left: 50%;">');
                $.ajax({
                    url: '/api/files/CreateFile',
                    data: formData,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    success: function (data) {
                        alertView.render('Well done!', 'Resource uploaded successfully.', 'alert-info');
                        self.getResourceData();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alertView.render('Oh snap!', 'Resource upload failed.', 'alert-error');
                        $('#formFileUploader .loading').remove();
                    }
                });
            }

            //$.post('/api/files/CreateFileNew/?formData=' + formData, formData, function (resData) {
            //    if (resData) {
            //        alert('Resource uploaded successfully');
            //    }
            //}, 'json');
        },
        getResourceData: function () {
            var that = this;
            new categoryCollection().fetch({
                success: function (collection, response) {

                    //self.render();
                    var dust_tag = createResourceTemplate;
                    var fileData = {};
                    fileData.categories = collection.toJSON();
                    var compiled = dust.compileFn(dust_tag, "tmp_category");
                    dust.loadSource(compiled);
                    dust.render("tmp_category", fileData, function (err, html_out) {
                        $("#fileContainer").html(html_out);
                    });

                    that.getTagData();
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
                        return $.get('/api/files?chars='+innerQuery, null, function (data) {
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
        isNumber:function (n) {
          return !isNaN(parseFloat(n)) && isFinite(n);
        }

    });
    return new createResourceView;
});