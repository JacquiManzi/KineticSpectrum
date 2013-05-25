define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/BorderContainer",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct"
],
    function (declare, html, BorderContainer, dom, ContentPane, domStyle, domConstruct) {
        "use strict";
        return declare("kui.ModelMenu.ModelMenu", null, {

            /*
             *   Bottom menu for Kinect 3D model files
             *
             */

            constructor: function () {


            },

            createMenu: function () {

             
                /**
                    Container for the file image icons
                */

                var modelBorderContainer = new BorderContainer({


                    style: "width:100%;" +
                           "height:100%;" +
                           "background-color:transparent;",
                    gutters:false 

                });

                var modelContainer = new ContentPane(
                {
                    region:"center", 
                    style: "width:100%;" +
                           "height:100%;" +
                           "background-color:transparent;"

                });

                modelBorderContainer.addChild(modelContainer);

                domConstruct.place(modelBorderContainer.domNode, dom.byId("bottomContainer"));

            }


        });

    });

