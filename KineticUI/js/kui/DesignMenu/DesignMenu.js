﻿


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
     "dojo/dom-geometry",
     "dojox/gfx",
     "dojo/aspect"
], 
    function (declare, AccordionContainer, ModelMenu, FileMenu, LEDMenu, PatternMenu,
        PatterComposerMenu, PatternModel, PatternComposerModel, html, CommonForm, domConstruct,
        domStyle, domClass, registry, domGeom, gfx, aspect) {

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

            var arrowMap = this.drawArrowCanvas(arrowDiv);
         
            this.arrowBarClickEvent(rightDiv, leftDiv, arrowMap);

            var thisObj = this;
            aspect.after(this , "resize", function () {
                thisObj.setArrowSize(arrowMap, arrowDiv);
            });
                                      
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

        drawArrowCanvas: function(div){

            var size =  10;
            var canvas = gfx.createSurface(div, size, size);

            var leftArrow = canvas.createImage({
                x: 0, y: 0, width: 0, height: 0, src: "../js/kui/images/leftArrow.svg"
            });

            var rightArrow = canvas.createImage({
                x: 0, y: 0, width: 0, height: 0, src: "../js/kui/images/rightArrow.svg"
            });

            return {
                canvas: canvas,
                leftArrow: leftArrow,
                rightArrow: rightArrow,
                currentArrow: leftArrow
            }
        },

        setArrowSize: function(arrowMap, div){

            var x = 0;
            var y = (domGeom.getContentBox(div).h) / 2;

            arrowMap.leftArrow.setShape({ x: 0, y: 0, width: 0, height: 0 });
            arrowMap.rightArrow.setShape({ x: 0, y: 0, width: 0, height: 0 });
            arrowMap.currentArrow.setShape({x: x, y: y, width: 13, height: 20});           
            arrowMap.canvas.setDimensions((domGeom.getContentBox(div).w),(domGeom.getContentBox(div).h));
            
        },

        arrowBarClickEvent: function (rightDiv, leftDiv, arrowMap) {

            var thisObj = this;
            dojo.connect(rightDiv, "onclick", function (evt) {

                var leftContainer = registry.byId("leftContainer");
                var kuiLayout = registry.byId("kuiLayout");

                var leftWidth = 0;
                var rightWidth = 0;

                if (!leftContainer.isHidden) {
                   
                    thisObj._hideMenu(leftDiv, rightDiv, kuiLayout, leftContainer);
                    leftContainer.isHidden = true;

                    arrowMap.currentArrow = arrowMap.rightArrow;
                    thisObj.setArrowSize(arrowMap, rightDiv);
                }
                else {

                    thisObj._showMenu(leftDiv, rightDiv, kuiLayout, leftContainer);
                    leftContainer.isHidden = false;

                    arrowMap.currentArrow = arrowMap.leftArrow;
                    thisObj.setArrowSize(arrowMap, rightDiv);
                }
            });

        },

        _hideMenu: function(leftDiv, rightDiv, kuiLayout, leftContainer){

            leftWidth = this._findPaneWidth(kuiLayout, 1);

            if (leftWidth < 25) {
                leftWidth = 25;
            }
            else if (leftWidth > 40) {
                leftWidth = 40;
            }

            leftContainer.resize({ w: leftWidth });
            domStyle.set(leftDiv, 'width', '0px');
            rightWidth = domGeom.getContentBox(leftContainer.domNode).w;

            /*Subtract 2 pixels for the border on design menu accordian container*/
            domStyle.set(rightDiv, 'width', rightWidth - 2 + 'px');

            kuiLayout.resize();
        },

        _showMenu: function(leftDiv, rightDiv, kuiLayout, leftContainer){

            leftWidth = this._findPaneWidth(kuiLayout, 26);
            leftContainer.resize({ w: leftWidth });
         
            kuiLayout.resize();

            leftDivWidth = this._findPaneWidth(leftContainer, 95);
            rightDivWidth = this._findPaneWidth(leftContainer, 5);

            domStyle.set(leftDiv, 'width', leftDivWidth - 10 + 'px');
            domStyle.set(rightDiv, 'width', rightDivWidth + 6 +'px');

            kuiLayout.resize();
        },

        _findPaneWidth: function (pane, percentage) {

            var width = domGeom.getContentBox(pane.domNode).w - domGeom.getContentBox(pane.domNode).l;
            var calculatedWidth = (percentage / 100) * width;

            return calculatedWidth;

        }
    });
});

