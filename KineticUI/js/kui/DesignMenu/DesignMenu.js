


define([   
    "dojo/_base/declare",
    "dijit/layout/AccordionContainer",
    "kui/DesignMenu/ModelMenu/ModelMenu",
    "kui/DesignMenu/FileMenu/FileMenu",
    "kui/DesignMenu/LEDMenu/LEDMenu",
    "kui/DesignMenu/PatternMenu/PatternMenu",
    "kui/DesignMenu/PatternComposerMenu/PatternComposerMenu",
    "kui/DesignMenu/PatternMenu/PatternModel",
    "kui/DesignMenu/PatternComposerMenu/PatternComposerModel",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dojo/dom-construct",
    "dojo/dom-style",
     "dojo/dom-class"
], 
    function (declare, AccordionContainer, ModelMenu, FileMenu, LEDMenu, PatternMenu,
        PatterComposerMenu, PatternModel, PatternComposerModel, html, CommonForm, domConstruct, domStyle, domClass) {

    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        constructor: function() {

            this.style = "background-color:#1f1f1f;" +
                "height:100%;" +
                "border-right: solid 2px #1f1f1f;" +
                "width:100%;";
        },
         
        createMenu: function () {

            this.containerDiv = html.createDiv("width:100%;" +
                "height:90%;"+
                "display:block;"+
                "background-color:#000000;");

            var arrowDiv = html.createDiv("width:100%;"+
                "height:100%;"+
                "border-top-right-radius: 7px;"+
                "border-bottom-right-radius: 7px;");

            var rightDiv = html.createDiv("width:5%;" +
                "height:100%;" +
                "float:right;" +
                "display:block;");

            var leftDiv = html.createDiv("width:95%;" +
               "height:100%;" +
               "float:left;" +
               "display:block;"+
               "background-color:#000000;");

            domClass.add(arrowDiv, "arrowButton");
               
            domConstruct.place(arrowDiv, rightDiv);
            domConstruct.place(rightDiv, this.containerDiv);
            domConstruct.place(this.domNode, leftDiv);
            domConstruct.place(leftDiv, this.containerDiv);
                           
            /*LED Menu*/
            var ledMenu = new LEDMenu({
                sceneModel: this.sceneModel
            });

            ledMenu.createLEDMenu(this); 
            
            /*Pattern Menu*/
            var patternModel = new PatternModel(this.sceneModel);
            var patternMenu = new PatternMenu({
                sceneModel: this.sceneModel,
                patternModel: patternModel
            });

            patternMenu.createPatternMenu(this); 

            /*Pattern Composer Menu*/
            var patternComposerModel = new PatternComposerModel(patternModel);
            var patternComposer = new PatterComposerMenu({
                sceneModel: this.sceneModel,
                patternComposerModel: patternComposerModel,
                patternModel: patternModel
            });

            patternComposer.createComposerMenu(this);
                    
           /*Model Menu
            var modelmenu = new ModelMenu(this.modelView);
            modelmenu.create3DMenu(this);*/

            /*File Menu*/
            var fileMenu = new FileMenu({
                sceneModel: this.sceneModel
            });

            fileMenu.createFileMenu(this);

            
            this.startup();           
        }
    });
});

