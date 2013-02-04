define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  'text!../../Templates/Home/HomeTemplate.html',
  'text!../../Templates/User/LoginNew.html',
  '../../Views/Navigation/HeaderNavigationView',
  '../../Models/user/LogInInfo',
  '../../Views/User/LoginSuccessView',
  '../../Views/User/ForgotPasswordview',
  'views/Home/AlertView',

], function ($, _, dust, Backbone, homeTemplate, loginNewTemplate, headerNavigationView, logInInfoModel, loginSuccessView, forgotPasswordview, alertView) {
    var homeView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            this.model = new logInInfoModel();
        },
        events:
        {
            'click #btnLoginNew': 'login',
            'click #linkForgotPasswd': 'forgotPassword'
        },
        render: function () {
            var dust_tag = loginNewTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_login");
            dust.loadSource(compiled);

            $.get('/api/user/?id=1', null, function (isLoggedIn) {
                if (isLoggedIn) {
                    $.get('/api/user?emailaddress=admin', null, function (userId) {
                        if (userId) {
                            loginSuccessView.render(userId);
                        }
                    }, 'json');
                }
                else {
                    dust.render("tmp_login", null, function (err, html_out) {
                        $("#fileContainer").html(html_out);
                    });
                    $.get('/api/user?userid=1', null, function (initialusermenulist) {
                        if (initialusermenulist) {

                            //headerNavigationView.render(initialusermenulist);
                        }
                    }, 'json');
                }
            }, 'json');
        },
        login: function (e) {
            e.preventDefault();
            var btn = $(e.target);
            var mail = $("#txtEmail").val();
            var pass = $("#txtPassword").val();
            if (mail != "" && pass != "") {
                if (btn.hasClass("disabled")) {
                    alertView.render('Please wait...', '', 'alert-info');
                    return;
                }
                btn.addClass("disabled");
                this.model.set({ emailAddress: mail, password: pass });
                $.post('/api/user', this.model.toJSON(), function (resData) {
                    if (resData) {
                        loginSuccessView.render(mail);
                        $.get('/api/user/?useremail=' + mail, null, function (usermenulist) {
                            if (usermenulist) {
                                headerNavigationView.render(usermenulist);
                                $("#fileContainer").html("");
                                $(".alert").alert('close');
                            }
                        }, 'json');
                    }
                    else {
                        alertView.render('Please enter valid email and password.');
                        btn.removeClass("disabled");
                    }
                }, 'json');
            }
            else {
                alertView.render('Please enter valid email and password.');
                btn.removeClass("disabled");
            }
        },
        forgotPassword: function (e) {
            e.preventDefault();
            forgotPasswordview.render();
        }
    });
    return new homeView;
});