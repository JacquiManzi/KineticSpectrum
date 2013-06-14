

define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct,three) {
        "use strict";
        return declare("kui.FileMenu.FileMenu", null, {

            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function(obj, obj1, modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;


            },

createFileMenu: function (container)
{
    var contentPane = new ContentPane(
      {
          title: "File Menu",
          style: "background-color:transparent;"

      });

    container.addChild(contentPane);

}




        });

    });
