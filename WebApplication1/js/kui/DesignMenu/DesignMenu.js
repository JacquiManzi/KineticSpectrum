


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
    "kui/LEDMenu/LEDMenu",
    "kui/PatternMenu/PatternMenu"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, AccordionContainer,three, ModelMenu, FileMenu, LEDMenu, PatternMenu) {
    "use strict";
    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        /*
         *   Left menu for Kinect 3D model design 
         *
         */

        constructor: function(obj, obj1, modelView) {

            this.style = "background-color:#1f1f1f;" +
                "height:80%;" +
                "border-right: solid 3  px #cccccc;";
            this.modelView = modelView;


        },
         
        createMenu: function () {


            /*LED Menu*/
            var ledMenu = new LEDMenu(this.modelView);
            ledMenu.createLEDMenu(this);

            /*Pattern Menu*/
            var patternMenu = new PatternMenu(this.modelView);
            patternMenu.createPatternMenu(this);

           /*Model Menu*/
            var modelmenu = new ModelMenu(this.modelView);
            modelmenu.create3DMenu(this);

            /*File Menu*/
            var fileMenu = new FileMenu();
            fileMenu.createFileMenu(this);

           
            this.startup();

        }


    });

});

