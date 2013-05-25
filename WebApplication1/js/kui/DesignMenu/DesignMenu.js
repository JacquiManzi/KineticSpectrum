


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
    return declare("kui.DesignMenu.DesignMenu", null, {

        /*
         *   Left menu for Kinect 3D model design
         *
         */

        constructor: function() {


        },

        createMenu: function () {

            var designMenuContainer = new BorderContainer(
            {
                design: "headline",
                style: "background-color:transparent",
                splitters: false,
                gutters: false 

            });

            domConstruct.place(designMenuContainer.domNode, dom.byId("leftContainer"));


            /**
                Top container for the design menu title
            */
            var titleContainer = new ContentPane(
            {
                region: "top", 
                style: "width:100%;" +
                       "height:100%;" +
                       "background-color:transparent;"

            });    

            designMenuContainer.addChild(titleContainer);

            this.createTitle(titleContainer);


           

        }, 

        createTitle: function (titleContainer)
        {

            //Set the title
            var title = "3D Design Menu";

            titleContainer.domNode.innerHTML = title;

            //Set the font and color
            domStyle.set(titleContainer.domNode, "font-size", "2em");
            domStyle.set(titleContainer.domNode, "color", "#6b32ff");
            domStyle.set(titleContainer.domNode, "text-align", "center");  

            

        }


         
    });

});

