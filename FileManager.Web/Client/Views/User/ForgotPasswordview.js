define([
  'jquery',
  'underscore',
  'dust',
  'backbone',
  '../../Models/user/ForgotPasswordModel',
  'views/Home/AlertView',
  'text!../../Templates/User/ForgotPassword.html'


], function ($, _, dust, Backbone,forgotPasswordModel, alertView, loginNewTemplate) {
    var forgotPasswordView = Backbone.View.extend({
        el: $("#fileContainer"),
        initialize: function () {
            this.model = forgotPasswordModel;
        },
        events:
        {
            'click #RecoverPassword': 'recoverPassword',
            'keypress #txtRecoverEmail': 'recoverOnEnterKey'
        },
        render: function () {
            var dust_tag = loginNewTemplate;
            var compiled = dust.compileFn(dust_tag, "tmp_forgotPassword");
            dust.loadSource(compiled);
            dust.render("tmp_forgotPassword", null, function (err, html_out) {
                $("#fileContainer").html(html_out);
            });

        },
        recoverPassword: function (e) {
            e.preventDefault();
            var mail = $("#txtRecoverEmail").val();
            if (mail) {
                $.ajax({
                    url: '/api/user?email=' + mail,
                    type: 'POST',
                    success: function (data) {
                        if (data) {
                            alert('Please check your mail.');
                            document.location.href = '/Client/Home.html';
                            $(".alert").alert('close');
                        }
                        else {
                            alertView.render('Sorry, you are not a valid member.');
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alertView.render('Sorry, you are not a valid member.');
                    }
                });
            }
            else
                alertView.render('Please enter a valid mail.');
        },
        isValidEmail: function (email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        },
        recoverOnEnterKey: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                this.recoverPassword(e);
            }
        }

    });
    return new forgotPasswordView;
});