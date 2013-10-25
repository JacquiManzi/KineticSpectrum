


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
     "dojo/dom-class",
     "dijit/registry",
     "dojo/dom-geometry"
], 
    function (declare, AccordionContainer, ModelMenu, FileMenu, LEDMenu, PatternMenu,
        PatterComposerMenu, PatternModel, PatternComposerModel, html, CommonForm, domConstruct,
        domStyle, domClass, registry, domGeom) {

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
         
            this.arrowBarClickEvent(rightDiv, leftDiv);
                                     
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
        },

        arrowBarClickEvent: function (rightDiv, leftDiv) {

            var thisObj = this;
            dojo.connect(rightDiv, "onclick", function (evt) {

                var leftContainer = registry.byId("leftContainer");
                var kuiLayout = registry.byId("kuiLayout");

                var leftWidth = 0;
                var rightWidth = 0;

                if (!leftContainer.isHidden) {
                   
                    leftWidth = thisObj.findPaneWidth(kuiLayout, 2);

                    if(leftWidth < 30){
                        leftWidth = 30;
                    }
                    else if (leftWidth > 40) {
                        leftWidth = 40;
                    }

                    leftContainer.resize({ w: leftWidth});
                    domStyle.set(leftDiv, 'width', '0px');
                    rightWidth = domGeom.getContentBox(leftContainer.domNode).w;

                    /*Subtract 2 pixels for the border on design menu accordian container*/
                    domStyle.set(rightDiv, 'width', rightWidth - 2 + 'px');

                    kuiLayout.resize();

                    leftContainer.isHidden = true;
                }
                else {

                    leftWidth = thisObj.findPaneWidth(kuiLayout, 26);
                    leftContainer.resize({ w: leftWidth });
                    
                    leftContainer.isHidden = false;
                    kuiLayout.resize();

                    leftDivWidth = thisObj.findPaneWidth(leftContainer, 95);
                    rightDivWidth = thisObj.findPaneWidth(leftContainer, 5);

                    domStyle.set(leftDiv, 'width', leftDivWidth + 'px');
                    domStyle.set(rightDiv, 'width', rightDivWidth + 'px');

                    kuiLayout.resize();
                }
            });

        },

        findPaneWidth: function (pane, percentage) {

            var width = domGeom.getContentBox(pane.domNode).w - domGeom.getContentBox(pane.domNode).l;
            var calculatedWidth = (percentage / 100) * width;

            return calculatedWidth;

        }

    });
});

