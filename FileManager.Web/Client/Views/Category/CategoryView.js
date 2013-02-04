define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/Category/CategoryCollection',
  '../../Models/Category/CategoryModel',
  'views/Home/AlertView',
  'text!../../Templates/Category/Category.html',
  'text!../../Templates/Category/CategoryEdit.html'
], function ($, _, dust, Backbone, categoryCollection, categoryModel, alertView, categoryTemplate, categoryEditTemplate) {
    var categoryView = Backbone.View.extend({
        el: '#fileContainer',
        initialize: function () {
            this.collection = new categoryCollection();
            //this.model = new categoryModel();
            //this.collection.fetch();
        },
        events: {
            'click #btnCatSave': 'CreateCategory',
            'click .EditCategory': 'showEditCategoryModal',
            'click .deleteCategory': 'deleteCategory',
            'click #btnCategoryUpdate': 'updateCategory',
            'click .addCategory': 'addCategoryModal',
            'change #inputTitle': 'clearCategoryTitleError'
        },
        clearCategoryTitleError: function (e) {
            e.preventDefault();
            $("#lblCategoryTitleError").html("");
        },
        render: function () {
            var self = this
            this.collection.fetch({
                success: function (collection, response) {
                    var dust_tag = categoryTemplate;
                    var fileData = {};
                    fileData.categories = self.collection.toJSON();
                    var compiled = dust.compileFn(dust_tag, "tmp_category");
                    dust.loadSource(compiled);
                    dust.render("tmp_category", fileData, function (err, html_out) {
                        $("#fileContainer").empty().html(html_out);
                        self.bindSortableGrid();
                    });

                    $.get('/api/permission/GetUserPermission?origin=category', null, function (res) {
                        if (res.length > 0)
                        {
                            if (! res[0].AllowWrite) {
                                $(".addCategory").prop("disabled", "true");
                                $(".EditCategory").hide();
                            }
                            if (!res[0].AllowDelete) {
                                $(".deleteCategory").hide();
                                //$(".deleteCategory").attr("disabled", "disabled").addClass("disabled");
                            }
                        }
                    }, 'json');
                }
            });

        },
        getCategoryData: function () {
            this.render();
        },
        CreateCategory: function (formElement) {
            var self = this
            var data = $('#frmCategoryCreate').serializeArray();
            var selectdCategoryText = $("#CreateCategoryId option:selected").text();
            var parentCategory = selectdCategoryText == 'Select Category' ? '' : selectdCategoryText;
            var dataObj = this.serializeObj(data);
            dataObj.ParentCategory = parentCategory;

            if (dataObj.Title == "") {
                $("#lblCategoryTitleError").html("please insert title");
            }

            if (dataObj.Title != "")
            {
                var newCategory = new categoryModel(dataObj);
                self.collection.create(newCategory,
                    {
                        wait: true,
                        success: function (model, response, event) {
                            alertView.render('Well done!', 'Category created successfully.', 'alert-info');
                            $('#categoryCreateModal').modal('hide');
                            self.render();
                        },

                        error: function (model, response, event) {
                            alertView.render('Oh snap!', 'Category not added.', 'alert-error');
                        }
                    }); 
            }
        },
        bindSortableGrid: function () {
            $("#tableCategory").tablesorter({ debug: false, sortList: [[0, 0]] })
            .tablesorterPager({ container: $("#pagerOne"), positionFixed: false })
            .tablesorterFilter({
                filterContainer: $("#filterBoxOne"),
                filterClearContainer: $("#filterClearOne"),
                filterCaseSensitive: false
            });
            $("#tableCategory .header").click(function () {
                $("#tableCategory tfoot .first").click();
            });
        },
        showEditCategoryModal: function (e) {
            e.preventDefault();
            var self = this;
            var fileData = {};
            var dust_tag = categoryEditTemplate;
            fileData.categories = self.collection.toJSON();
            var fileId = parseInt($(e.target).attr('data-Id'));

            var res = this.collection.get(fileId).toJSON();
                    _.extend(fileData, res);
                    var compiled = dust.compileFn(dust_tag, "tmp_category");
                    dust.loadSource(compiled);
                    dust.render("tmp_category", fileData, function (err, html_out) {
                        $("#categoryEdit").html(html_out);
                    });
                    $("#categoryEditModal #allCategories").val(res.CategoryId);
                    $('#categoryEditModal').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');
        },
        updateCategory: function (e) {
            var self = this
            var fileId = parseInt($(e.target).attr('data-Id'));
            var data = $('#categoryEditForm').serializeArray();
            var selectdCategoryText = $("#allCategories option:selected").text();
            var parentCategory = selectdCategoryText == 'Select Category' ? '' : selectdCategoryText;
                
            var dataObj = this.serializeObj(data);
            dataObj.ParentCategory = parentCategory;

                this.collection.get(fileId).set(dataObj).save(
                    null,{
                        wait: true,
                        success: function (model, response, event) {
                            alertView.render('Well done!', 'Category updated successfully.', 'alert-info');
                            $('#categoryEditModal').modal('hide');
                            self.render();
                        },

                        error: function (model, response, event) {
                            alertView.render('Oh snap!', 'Category not update.', 'alert-error');
                        }
                    });
        },
        deleteCategory: function (e) {
            var self = this;
                var fileId = parseInt($(e.target).attr('data-Id'));
                this.collection.get(fileId).destroy(
                     {
                         wait: true,
                         success: function (model, response, event) {
                             alertView.render('Well done!', 'Category deleted successfully.', 'alert-info');
                             self.render();
                         },

                         error: function (model, response, event) {
                             alertView.render('Oh snap!', 'Category not delete.', 'alert-error');
                         }
                     });
        },
        addCategoryModal: function (e) {
                $('#categoryCreateModal').modal({
                    keyboard: true,
                    backdrop: true
                }).modal('show');                
        },
        serializeObj: function (serializeAr) {
            var obj = {};
            $.each(serializeAr, function (i, o) {
                var n = o.name,
                  v = o.value;

                obj[n] = obj[n] === undefined ? v
                  : $.isArray(obj[n]) ? obj[n].concat(v)
                  : [obj[n], v];
            });

            return obj;
        }
    });
    return new categoryView;
});