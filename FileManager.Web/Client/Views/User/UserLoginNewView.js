define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Models/user/LogInInfo',
  '../../Views/user/LoginSuccessView',
  'text!../../Templates/User/LoginNew.html',
  '../../Views/Navigation/HeaderNavigationView',
  '../../Views/Home/HomeView',
  'views/Home/AlertView'

], function ($, _, dust, Backbone, logInInfoModel, loginSuccessView, loginTemplate, headerNavigationView, homeView) {
    var userLoginNewView = Backbone.View.extend({
        el: $("#userlogin"),
        initialize: function () {
            this.model = new logInInfoModel();
        },
        events:
        {
            'click #btnLoginNew': 'login'
            
        },
        render: function () {

            var dust_tag = loginTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_login");
            dust.loadSource(compiled);

            $.get('/api/user/?id=1', null, function (isLoggedIn) {
                if (isLoggedIn) {
                    $.get('/api/user/?emailaddress=admin', null, function (userId) {
                        if (userId) {
                            loginSuccessView.render(userId);
                        }
                    }, 'json');
                }
                else {
                    dust.render("tmp_login", null, function (err, html_out) {
                        $("#fileContainer").html(html_out);
                    });
                    $.get('/api/user/?userid=1', null, function (initialusermenulist) {
                        if (initialusermenulist) {
                            headerNavigationView.render(initialusermenulist);
                        }
                    }, 'json');

                    homeView.render();
                }
            }, 'json');


        },
        login: function (e) {
            e.preventDefault();

            var mail = $("#txtEmail").val();
            var pass = $("#txtPassword").val();
            if (mail != "" && pass != "") {
                this.model.set({ emailAddress: mail, password: pass });
                $.post('/api/user', this.model.toJSON(), function (resData) {
                    if (resData) {
                        loginSuccessView.render(mail);
                        $.get('/api/user/?useremail=' + mail, null, function (usermenulist) {
                            if (usermenulist) {
                                headerNavigationView.render(usermenulist);
                            }
                        }, 'json');
                    }
                }, 'json');
            }

        }
    });
    return new userLoginNewView;
});
