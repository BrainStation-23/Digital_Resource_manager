// Filename: router.js
define([
  'jquery',
  'underscore',
  'backbone',
  'views/Home/HomeView',
  'views/Navigation/HeaderNavigationView',
  'views/Category/CategoryView',
  'views/CreateResourceView',
  'views/Search/SearchView',
  'views/User/UsersView',
  'views/User/PermissionView',
  'views/Favourite/FavouriteView',
  'views/History/DownloadHistoryView',
  'views/Busket/DownloadBusketView',
  'views/GenericSearch/GenericSearchView'
  
], function ($, _, Backbone, homeView, headerNavigationView, categoryListView, createResourceView, searchView, usersView, permissionView, favouriteView, downloadHistoryView, downloadBusketView, genericSearchView) {
    var AppRouter = Backbone.Router.extend({
    routes: {
      // Define some URL routes
        'Resource': 'showResourceURL',
        'resource': 'showResourceURL',
      'Users': 'showUsersUrl',
      'users': 'showUsersUrl',
      'Search': 'showSearchURL',
      'search': 'showSearchURL',
      'Favourites': 'showFavouriteURL',
      'favourites': 'showFavouriteURL',
      'Download Basket': 'showDownloadBasketURL',
      'download basket': 'showDownloadBasketURL',
      'Category': 'showCategoryURL',
      'category': 'showCategoryURL',
      'Create Role': 'showCreateRoleURL',
      'create role': 'showCreateRoleURL',
      'Download History': 'showHistoryURL',
      'download history': 'showHistoryURL',
      'List': 'showGenericSearch',
      'list': 'showGenericSearch',
      // Default
      '*actions': 'defaultAction'
    },
    initialize: function (options) {
        this.bind('all', this.defaultTask);
    },
    defaultAction: function (actions) {
        var self = this;
        //homeView.render();
        //self.headerNavigationRender();
    },

    headerNavigationRender: function (actions) {
        homeView.render();
        $.get('/api/user/?useremail=' + "", null, function (initialusermenulist) {
            if (initialusermenulist && initialusermenulist.length > 0) {
                headerNavigationView.render(initialusermenulist);
            }
        }, 'json');
    },

      /* Code  Migrated from HeaderNavigation */

    showCategoryURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                categoryListView.getCategoryData();

                var currentTab = $('a[href="#Category"]');
                if (currentTab.length==0) {
                    self.headerNavigationRender();
                }
               
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
    
    showResourceURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                createResourceView.getResourceData();
                var currentTab = $('a[href="#Resource"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
    
    showSearchURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                searchView.render();
                var currentTab = $('a[href="#Search"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
    showGenericSearch:function()
    {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                genericSearchView.render();
                var currentTab = $('a[href="#Generic Search"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
    showUsersUrl: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                usersView.render();
                var currentTab = $('a[href="#Users"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
   
    showCreateRoleURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                permissionView.render();
                var currentTab = $('a[href="#Create Role"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
   
    showFavouriteURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                favouriteView.render();
                var currentTab = $('a[href="#Favourites"]');
                self.addActivClass(currentTab);
            }
        }, 'json');
    },
   
    showHistoryURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                downloadHistoryView.render();
                var currentTab = $('a[href="#Download History"]');
                self.addActivClass(currentTab)
            }
        }, 'json');
    },
   
    showDownloadBasketURL: function () {
        var self = this;
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (isLoggedIn) {
                $.get('/api/basket', null, function (res) {
                    downloadBusketView.render(res);
                    var currentTab = $('a[href="#Download Basket"]');
                    self.addActivClass(currentTab)
                }, 'json');
            }
        }, 'json');
    },

    addActivClass: function (e) {
        e.parent().addClass('active').siblings().removeClass('active');
        $(".alert").alert('close');
    },
    defaultTask: function (route, router) {
        var self = this;
        var isEmptyHeader = $('.nav').children('li').length == 0;
        var isNotLoginWindow = $('#frmLogin').length == 0
        if (isEmptyHeader && isNotLoginWindow) {
            self.headerNavigationRender();
        }
    }
   
  });

  var initialize = function(){
    var app_router = new AppRouter;
    Backbone.history.start();
  };

  return {
    initialize: initialize
  };
    
});


var timerId;
function myTimeoutFunction() {
    function sessionOut() {
        $.get('/api/user/?id=1', null, function (isLoggedIn) {
            if (!isLoggedIn) {
                $("#btnLogOut").click();
            }
        }, 'json');
    }

    timerId = setInterval(sessionOut, 720000);
}
myTimeoutFunction();