


define([   
    "dojo/_base/declare",
    "dijit/layout/AccordionContainer",
    "threejs/three",
    "kui/DesignMenu/ModelMenu",
    "kui/DesignMenu/FileMenu",
    "kui/DesignMenu/LEDMenu",
    "kui/DesignMenu/PatternMenu"
],
    function (declare, AccordionContainer,three, ModelMenu, FileMenu, LEDMenu, PatternMenu) {
    "use strict";
    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        /*
         *   Left menu for Kinect 3D model design 
         *
         */

        constructor: function(obj, obj1, modelView) {

            this.style = "background-color:#1f1f1f;" +
                "height:90%;" +
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
            var fileMenu = new FileMenu(this.modelView);
            fileMenu.createFileMenu(this);

           
            this.startup();

        }


    });

});

