define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Collections/User/UserCollection',
  'views/Home/AlertView',
  'text!../../Templates/User/Users.html',
  'text!../../Templates/User/EditUser.html',
  'text!../../Templates/User/CreateNewUser.html'
], function ($, _, dust, Backbone, userCollection, alertView, usersTemplate, editUsersTemplate, createNewUserTemplate) {
    var usersView = Backbone.View.extend({
        el: '#fileContainer',
        initialize: function () {
            this.collection = userCollection;
            _.extend(formProxy, Backbone.Events);
            
        },
        events: {
            'click .EditUser': 'showEditUserModal',
            'click .deleteUser': 'deleteUser',
            'click #btnUserUpdate': 'updateUser',
            'click .addUser': 'addUserModal',
            'click #btnCreateUser': 'createuser'
        },
        render: function () {
            var self = this
            this.collection.fetch({
                success: function (collection, response) {
                    var dust_tag = usersTemplate;
                    var userData = {};
                    userData.users = self.collection.toJSON();
                    var compiled = dust.compileFn(dust_tag, "tmp_user");
                    dust.loadSource(compiled);
                    dust.render("tmp_user", userData, function (err, html_out) {
                        $("#fileContainer").empty().html(html_out);
                        self.bindSortableGrid();
                    });

                    $.get('/api/permission/GetUserPermission?origin=users', null, function (res) {
                        if (res.length > 0) {
                            if (!res[0].AllowWrite) {
                                $(".addUser").prop("disabled", "true");
                                $(".EditUser").hide();
                            }
                            if (!res[0].AllowDelete) {
                                $(".deleteUser").hide();
                            }
                        }
                    }, 'json');
                }
            });

        },
        bindSortableGrid: function () {
            var table = $('#tableUsers').tablesorter({
                theme: 'blue',
                debug: false,
                sotList: [[0, 0]],
                widgets: ["filter"],
                widgetOptions: {
                    // use the filter_external option OR use bindSearch function (below)
                    // to bind external filters.
                    // filter_external : '.search',

                    filter_columnFilters: false,
                    filter_saveFilters: false,
                    filter_reset: '.reset'
                }
            }).tablesorterPager({
                container: $("#userpagerOne")
            });

            $.tablesorter.filter.bindSearch(table, $('.search'), false);
        },
        showEditUserModal: function (e) {
            e.preventDefault();
            var self = this;
            
            var dust_tag = editUsersTemplate;
            var userName = $(e.target).attr('data-Id');
            var res = this.collection.get(userName).toJSON();
                var userData = {};
                userData.roles = null;
                _.extend(userData, res);
                $.get('/api/role', null, function (resData) {
                    userData.roles = resData;

                    var compiled = dust.compileFn(dust_tag, "tmp_editUser");
                    dust.loadSource(compiled);
                    dust.render("tmp_editUser", userData, function (err, html_out) {
                        $("#editUserContainer").html(html_out);
                    });
                    $("#userEditModal #userRole").val(res.role);
                    $('#userEditModal').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');

                }, 'json');

        },
        updateUser: function (e) {
            e.preventDefault();
            var self = this
            var userName = $(e.target).attr('data-Id');
            var data = $('#userEditForm').serializeArray();
            var dataObj = this.serializeObj(data);
            if (self.isValidEmail(dataObj.email)) {
                this.collection.get(userName).set(dataObj).save(
                    null, {
                        wait: true,
                        success: function (model, response, event) {
                            alertView.render('Well done!', "User updated successfully.", "alert-info");
                            $('#userEditModal').modal('hide');
                            self.render();
                        },

                        error: function (model, response, event) {
                            alertView.render('Oh snap!', 'User update failed.', 'alert-error');
                        }
                    });
            }
            else {                
                $("#lblEditEmailError").html("please insert valid email");
            }
        },
        deleteUser: function (e) {
            var self = this;
            var userName = $(e.target).attr('data-Id');
            this.collection.get(userName).destroy(
             {
                 wait: true,
                 success: function (model, response, event) {
                     alertView.render('Well done!', 'User deleted successfully.', 'alert-info');
                     self.render();
                 },

                 error: function (model, response, event) {
                     alertView.render('Oh snap!', 'User not delete.', 'alert-error');
                 }
             });

        },
        addUserModal: function (e) {
            $.get('/api/role', null, function (resData) {
                //createNewUserView.render(resData);
                var self = this;
                var userRole = {};
                userRole.roles = resData;
                var dust_tag = createNewUserTemplate;
                var compiled = dust.compileFn(dust_tag, "tmp_createnewuser");
                dust.loadSource(compiled);
                dust.render("tmp_createnewuser", userRole, function (err, html_out) {
                    $("#createUserContainer").html(html_out);
                    //self.createuser();
                    $('#userCreateModal').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');
                });

            }, 'json');
        },
        createuser: function (e) {
            e.preventDefault();
            var self = this;
          

                var userloginId = $("#loginId").val().trim();
                var userpassword = $("#txtUserPassword").val().trim();
                var userconfirmedpassword = $("#txtConfirmPassword").val().trim();
                var useremail = $("#txtemail").val().trim();
                var userrole = $("#userRole").val().trim();

                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                if (!userloginId) {
                    $("#lblloginIdError").html("please insert login id");
                }
                if (!userpassword) {
                    $("#lblPasswordError").html("please insert password");
                }
                if (!userconfirmedpassword) {
                    $("#lblConfirmPasswordError").html("please insert password");
                }
                if (!useremail) {
                    $("#lblEmailError").html("please insert email");
                }
                else if (!re.test(useremail)) {
                    $("#lblEmailError").html("please insert valid email");
                }
                if (!userrole) {
                    $("#lblRoleError").html("please select role");
                }

                if (userloginId != "" && userpassword != "" && userconfirmedpassword != "" && useremail != "" && userrole != "" && re.test(useremail)) {
                    if (userpassword != userconfirmedpassword) {
                        $("#lblConfirmPasswordError").html("password and confirmpassword not same");
                    }
                    else {

                        self.collection.create({ Username: userloginId, password: userpassword, email: useremail, role: userrole },
                                {
                                    wait: true,
                                    type: 'POST',
                                    success: function (model, response, event) {
                                        alertView.render('Well done!', "User created successfully.", "alert-info");
                                        $('#userCreateModal').modal('hide');
                                        self.render();
                                    },

                                    error: function (model, response, event) {
                                        alertView.render('Oh snap!', 'User not added.', 'alert-error');
                                    }
                                });                                
                    }
                }

            
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
        },
        isValidEmail: function (email) {
            var regEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return regEmail.test(email);
        }
    });
    return new usersView;
});