define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Models/user/PermissionModel',
  'views/Home/AlertView',
  'text!../../Templates/User/UserRole.html',
  'text!../../Templates/User/CreateUserRole.html',
  'text!../../Templates/User/EditUserRole.html'

], function ($, _, dust, Backbone, permissionModel, alertView, userRoleTemplate, createUserRoleTemplate, editUserRoleTemplate) {
    var permissionView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            this.model = new permissionModel();
        },
        events:
        {
            'click #btnCreateRole': 'createrole',
            'click .deleteRole': 'deleteRole',
            'click .EditRole': 'showEditRoleModal',
            'click #btnUpdateRole': 'updateRole',
            'click .addRole': 'showCreateRoleModal',
        },
        render: function () {
            var self = this;
            $.get('/api/permission', null, function (resData) {          
                var userRole = {};
                userRole.rolepermissions = resData;
                var dust_tag = userRoleTemplate;
                var compiled = dust.compileFn(dust_tag, "tmp_userrole");
                dust.loadSource(compiled);
                dust.render("tmp_userrole", userRole, function (err, html_out) {
                    $("#fileContainer").html(html_out);
                    self.bindSortableGrid();
                });

                $.get('/api/permission/GetUserPermission?origin=createrole', null, function (res) {
                    if (res.length > 0) {
                        if (!res[0].AllowWrite) {
                            $(".addRole").prop("disabled", "true");
                            $(".EditRole").hide();
                        }
                        if (!res[0].AllowDelete) {
                            $(".deleteRole").hide();
                        }
                    }
                }, 'json');

            }, 'json');
        },
        createrole: function (e) {
            e.preventDefault();
            var self = this;
            var role = $("#inputRoleName").val();          
            if (role) {
                var permissions = self.selectAllPermission($("#tblRole tr"));
                this.model.set({ UserRole: role, Userpermissions: permissions }).save(null,
                {
                    wait: true,
                    type: 'POST',
                    success: function (model, response, event) {
                        alertView.render('Well done!', 'Role created successfully.', 'alert-info');
                        $('#roleCreateModal').modal('hide');
                        self.render();
                    },
                    error: function (model, response, event) {
                        alertView.render('Oh snap!', 'Role not create.', 'alert-error');
                    }
                });
            } else {
                $("#lblRoleNameError").html('Please add a role name');
            }
        },
        showCreateRoleModal: function (e) {
            e.preventDefault();
            $.get('/api/Permission/GetAllPermission?permission=0', null, function (resData) {
                var userRole = {};
                userRole.permissions = resData;
                var dust_tag = createUserRoleTemplate;
                var compiled = dust.compileFn(dust_tag, "tmp_createuserrole");
                dust.loadSource(compiled);
                dust.render("tmp_createuserrole", userRole, function (err, html_out) {
                    $("#roleAdd").html(html_out);
                    $('#tblRole .Disabled').prop('disabled', true);
                    $('#roleCreateModal').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');
                });

            }, 'json');
        },
        deleteRole: function (e) {
            e.preventDefault();
            var self = this;
            var roleName = $(e.target).attr('data-Id');
            $.ajax({
                url: '/api/Permission?roleName=' + roleName,
                type: 'DELETE',
                success: function (data) {
                    alertView.render('Well done!', 'Role deleted successfully.', 'alert-info');
                    self.render();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alertView.render('Oh snap!', 'Role not delete.', 'alert-error');
                }
            });
        },
        showEditRoleModal: function (e) {
            e.preventDefault();
            var roleName = $(e.target).attr('data-Id');
            $.get('/api/Permission/GetPermissionByRoleName', { roleName: roleName }, function (res) {

                var userRole = {};
                userRole.RoleName = res.RoleName;
                userRole.permissions = res.PermissionViewModelList;

                var dust_tag = editUserRoleTemplate;
                var compiled = dust.compileFn(dust_tag, "tmp_edituserrole");
                dust.loadSource(compiled);
                dust.render("tmp_edituserrole", userRole, function (err, html_out) {
                    $("#roleEdit").html(html_out);
                    $('#tblEditRole .Checked').prop('checked', true);
                    $('#tblEditRole .Disabled').prop('disabled', true);
                    $('#roleEditModal').modal({
                        keyboard: true,
                        backdrop: true
                    }).modal('show');
                });

            }, 'json');
        },
        updateRole: function (e) {
            e.preventDefault();
            var self = this;
            var role = $("#editRoleName").val();
            if (role) {
                var permissions = self.selectAllPermission($("#tblEditRole tr"));
                this.model.set({ UserRole: role, Userpermissions: permissions }).save(null,
                {
                    wait: true,
                    success: function (model, response, event) {
                        alertView.render('Well done!', 'Role updated successfully.', 'alert-info');
                        $('#roleEditModal').modal('hide');
                        self.render();
                    },

                    error: function (model, response, event) {
                        alertView.render('Oh snap!', 'Role update failed.', 'alert-error');
                    }
                });
            }
            else {
                $('#lblRoleNameEditErr').html('Role name should not be empty');
            }
        },
        bindSortableGrid: function () {
            $("#tableRoles").tablesorter({ debug: false, sortList: [[0, 0]] })
            .tablesorterPager({ container: $("#rolepagerOne"), positionFixed: false })
            .tablesorterFilter({
                filterContainer: $("#rolefilterBoxOne"),
                filterClearContainer: $("#rolefilterClearOne"),
                filterCaseSensitive: false
            });
            $("#tableRoles .header").click(function () {
                $("#tableRoles tfoot .first").click();
            });
        },
        selectAllPermission: function (context) {
            var permissions = [];

            var allowRead = false;
            var allowWrite = false;
            var allowDelete = false;
            context.each(function (index, value) {
                allowRead = false;
                allowWrite = false;
                allowDelete = false;
                if (index > 0) {
                    if ($(this.childNodes[1].childNodes[0]).is(":checked") || $(this.childNodes[2].childNodes[0]).is(":checked") || $(this.childNodes[3].childNodes[0]).is(":checked")) {
                        var permissionText = this.childNodes[0].innerHTML;
                        //this.childNodes[1].childNodes[0].getAttribute('id')

                        if ($(this.childNodes[1].childNodes[0]).is(":checked")) {
                            allowRead = true;
                        }
                        if ($(this.childNodes[2].childNodes[0]).is(":checked")) {
                            allowWrite = true;
                        }
                        if ($(this.childNodes[3].childNodes[0]).is(":checked")) {
                            allowDelete = true;
                        }

                        var permission = {};
                        permission.PermissionName = permissionText;
                        permission.AllowRead = allowRead;
                        permission.AllowWrite = allowWrite;
                        permission.AllowDelete = allowDelete;
                        permissions.push(permission);
                    }
                }
            });

            return permissions;
        }
    });
    return new permissionView;
});