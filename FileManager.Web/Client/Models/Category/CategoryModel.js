define([
  'underscore',
  'backbone'
], function (_, Backbone) {
    var categoryModel = Backbone.Model.extend({
        idAttribute: "Id",
        urlRoot: '/api/categories',
        url : function(){
            var url = this.urlRoot;//+ this.id;
            if(this.get("Id")){
                url = url + "?id=" + this.get("Id");
            }
            return url;
        },
        /*defaults: {
            Id: 0,
            Title: '',
            CategoryId: 0,
            ParentCategory:''
        },*/

        initialize: function () {
        },
        validation: {
            Title: {
                required: true,
                msg: 'Please enter a Title'
            }
        }

    });
    return categoryModel;

});