/// <reference path="../scripts/jquery.tablesorter.widgets.js" />
window.formProxy = {};
require.config({
    /*enforceDefine: true,*/
    paths: {
        jquery: '../scripts/jquery-3.7.0',
        'jquery.bootstrap': '../scripts/bootstrap.min',
        'jquery.upload': '../scripts/jquery.upload-1.0.2.min',
        'jquery.bootpag': '../scripts/jquery.bootpag',
        'jquery.tablesorter': '../scripts/jquery.tablesorter.min',
        'jquery.tablesorter.pager': '../scripts/jquery.tablesorter.pager',
        'jquery.tablesorter.widgets': '../scripts/jquery.tablesorter.widgets.min',
        'backbone-validation-amd-min': '../scripts/backbone-validation-amd-min',
        underscore: '../scripts/underscore',       
        dust: '../scripts/dust-full-0.6.0',
        backbone: '../Scripts/backbone',
        text: '../scripts/text',
        homeCollection:'Collections/HomeCollection',
        templates: '../templates'
    },
    waitSeconds: 200,
    shim: {
        'jquery.upload': {
            deps: ['jquery']
        },
        'jquery.bootpag':{
            deps: ['jquery']
        },
        'jquery.tablesorter': {
            deps: ['jquery']
        },
        'jquery.tablesorter.pager': {
            deps: ['jquery', 'jquery.tablesorter']
        },
        'jquery.tablesorter.widgets': {
            deps: ['jquery', 'jquery.tablesorter']
        },
        'jquery.bootstrap': {
            deps: ['jquery'],
            exports: '$'
        },
        underscore: {
            exports: '_'
        },
        'backbone-validation-amd-min': {
            deps: ['backbone']
        },
        backbone: {
            deps: ["underscore", "jquery"],
            exports: "Backbone"
        }
    }

});

require([

  // Load our app module and pass it to our definition function
  'app',
   'backbone',
  'jquery.bootstrap',
  'jquery.upload',
  'jquery.bootpag',
  'jquery.tablesorter',
  'jquery.tablesorter.pager',
  'jquery.tablesorter.widgets',
  'backbone-validation-amd-min'

  // Some plugins have to be loaded in order due to their non AMD compliance
  // Because these scripts are not "modules" they do not pass any values to the definition function below
], function (App, Backbone) {
    // The "app" dependency is passed in as "App"
    // Again, the other dependencies passed in are not "AMD" therefore don't pass a parameter to this function
    App.initialize();
    
})
