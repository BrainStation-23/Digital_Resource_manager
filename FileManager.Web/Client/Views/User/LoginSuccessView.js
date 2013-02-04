define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Models/user/LogInInfo',
  'text!../../Templates/User/LoginSuccess.html',
  'text!../../Templates/User/LoginNew.html',
  '../../Views/Navigation/HeaderNavigationView'

], function ($, _, dust, Backbone, logInInfoModel, loginSuccessTemplate, loginTemplate, headerNavigationView) {
    var loginSuccessView = Backbone.View.extend({
        el: $("#userlogin"),
        initialize: function () {
            
            this.model = new logInInfoModel();
        },
        events:
        {
            'click #btnLogOut': 'logout'
        },
        render: function (mail) {
            var dust_tag = loginSuccessTemplate;
            mail =  mail;
            this.model.set({ emailAddress: mail, password: '' });
            var compiled = dust.compileFn(dust_tag, "tmp_logout");
            dust.loadSource(compiled);
            dust.render("tmp_logout", this.model.toJSON(), function (err, html_out) {
                $("#userlogin").html(html_out);
            });
        },
        logout: function (e) {
            e.preventDefault();
            var email = $("#currentUser").html();
            $.get('/api/user/?email=' + email, email, function (resData) {

                headerNavigationView.render(resData);

                var dust_tag = loginTemplate;
                var compiled = dust.compileFn(dust_tag, "tmp_login");
                dust.loadSource(compiled);
                dust.render("tmp_login", null, function (err, html_out) {
                    $("#fileContainer").html(html_out);
                    $("#userlogin").html("");
                    $(".alert").alert('close');
                    document.location.hash = '';
                });

            }, 'json');

            if (timerId)
                clearInterval(timerId);
        }
    });
    return new loginSuccessView;
});