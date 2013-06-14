


define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dijit/layout/AccordionContainer",
    "threejs/three",
    "kui/ModelMenu/ModelMenu",
    "kui/FileMenu/FileMenu",
    "kui/LEDMenu/LEDMenu"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, AccordionContainer,three, ModelMenu, FileMenu, LEDMenu) {
    "use strict";
    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        /*
         *   Left menu for Kinect 3D model design 
         *
         */

        constructor: function(obj, obj1, modelView) {

            this.style = "background-color:transparent;";
            this.modelView = modelView;


        },
         
        createMenu: function () {

           /*Model Menu*/
            var modelmenu = new ModelMenu();
            modelmenu.create3DMenu(this);

            /*File Menu*/
            var fileMenu = new FileMenu();
            fileMenu.createFileMenu(this);

            /*LEDMenu*/
            var ledMenu = new LEDMenu();
            ledMenu.createLEDMenu(this);

            this.startup();

        }


    });

});

