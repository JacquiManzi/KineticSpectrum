


define([   
    "dojo/_base/declare",
    "dijit/layout/AccordionContainer",
    "kui/DesignMenu/ModelMenu/ModelMenu",
    "kui/DesignMenu/FileMenu/FileMenu",
    "kui/DesignMenu/LEDMenu/LEDMenu",
    "kui/DesignMenu/PatternMenu/PatternMenu",
    "kui/DesignMenu/PatternComposerMenu/PatternComposerMenu",
    "kui/DesignMenu/PatternMenu/PatternModel",
     "kui/DesignMenu/PatternComposerMenu/PatternComposerModel"
], 
    function (declare, AccordionContainer, ModelMenu, FileMenu, LEDMenu, PatternMenu,
        PatterComposerMenu, PatternModel, PatternComposerModel) {
    "use strict";
    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        constructor: function() {

            this.style = "background-color:#1f1f1f;" +
                "height:90%;" +
                "border-right: solid 3  px #cccccc;"; 
        },
         
        createMenu: function () {
               
            /*LED Menu*/
            var ledMenu = new LEDMenu(this.sceneModel);
            ledMenu.createLEDMenu(this);
            
            /*Pattern Menu*/
            var patternModel = new PatternModel(this.sceneModel);
            var patternMenu = new PatternMenu(this.sceneModel, patternModel);
            patternMenu.createPatternMenu(this); 

            /*Pattern Composer Menu*/
            var patternComposerModel = new PatternComposerModel(patternModel);
            var patternComposer = new PatterComposerMenu(this.sceneModel, patternComposerModel, patternModel);
            patternComposer.createComposerMenu(this);
                    
           /*Model Menu
            var modelmenu = new ModelMenu(this.modelView);
            modelmenu.create3DMenu(this);*/

            /*File Menu*/
            var fileMenu = new FileMenu({ sceneModel: this.sceneModel });
            fileMenu.createFileMenu(this);

            this.startup();
        }
    });
});

