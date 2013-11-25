/*
*@Author: Jacqui Manzi
*Novemeber 22nd, 2013
*jacqui@revkitt.com
*
* DesignMenu.js - This is the main AccordionContainer layout menu for Kinetic Spectrum UI (kui). 
*                 All subset Accordian Iten menus are placed here.
*/

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
     "dojo/aspect",
     "kui/Simulation/Simulation"
], 
    function (declare, AccordionContainer, ModelMenu, FileMenu, LEDMenu, PatternMenu,
        PatterComposerMenu, PatternModel, PatternComposerModel, html, CommonForm, domConstruct,
        domStyle, domClass, registry, domGeom, gfx, aspect, Simulation) {

    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        constructor: function() {
            
            /*Default main css styling*/
            this.style = "background-color:#1f1f1f;" +
                "height:100%;" +
                "border-right: solid 2px #1f1f1f;" +
                "width:100%;";          
        },
        
        /*Constructs the subset AccordianItem menus and creates the hide/show arrow bar for hiding the Design Menu*/
        createMenu: function () {

            this.containerDiv = html.createDiv("width:100%;" +
                "height:90%;"+
                "display:block;"+
                "background-color:#000000;");

            var arrowDiv = html.createDiv("width:100%;"+
                "height:97%;"); 

            var fullScreenDiv = html.createDiv();

            var rightDiv = html.createDiv("width:4%;" + 
                "height:100%;" +
                "float:right;" +
                "display:block;");

            var leftDiv = html.createDiv("width:95%;" +
               "height:100%;" + 
               "float:left;" +
               "display:block;"+
               "background-color:#000000;");

            domClass.add(arrowDiv, "arrowButton");
            domClass.add(fullScreenDiv, "fullScreenButton"); 
               
            domConstruct.place(fullScreenDiv, rightDiv);
            domConstruct.place(arrowDiv, rightDiv);
            domConstruct.place(rightDiv, this.containerDiv);
            domConstruct.place(this.domNode, leftDiv);
            domConstruct.place(leftDiv, this.containerDiv);

            var arrowMap = this._drawArrowCanvas(arrowDiv); 
         
            var thisObj = this;
            aspect.after(this , "resize", function () {
                thisObj._setArrowSize(arrowMap, arrowDiv);
            });
                                      
            /*LED Menu*/
            var ledMenu = new LEDMenu({
                sceneModel: this.sceneModel
            });

            ledMenu.createLEDMenu(this); 
             
            /*Pattern Menu*/
            var patternModel = new PatternModel(this.sceneModel, this.simulation);
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

            this._fullScreenClickEvent(fullScreenDiv, leftDiv, rightDiv, patternComposerModel); 
            this._arrowBarClickEvent(arrowDiv, rightDiv, leftDiv, arrowMap, patternComposerModel);
                    
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

        /*Draws the arrow image - orientation depends on hide/show state*/
        _drawArrowCanvas: function(div){

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
            };
        },

        _setArrowSize: function(arrowMap, div){

            var x = 0;
            var y = (domGeom.getContentBox(div).h) / 2;

            arrowMap.leftArrow.setShape({ x: 0, y: 0, width: 0, height: 0 });
            arrowMap.rightArrow.setShape({ x: 0, y: 0, width: 0, height: 0 });
            arrowMap.currentArrow.setShape({x: x, y: y, width: 15, height: 20});           
            arrowMap.canvas.setDimensions((domGeom.getContentBox(div).w),(domGeom.getContentBox(div).h));
            
        },

        _arrowBarClickEvent: function (arrowDiv, rightDiv, leftDiv, arrowMap, patternComposerModel) {

            var thisObj = this;
            dojo.connect(arrowDiv, "onclick", function () {

                var leftContainer = registry.byId("leftContainer");
                var kuiLayout = registry.byId("kuiLayout");

                if (!leftContainer.isHidden) {
                   
                    thisObj._hideMenu(leftDiv, rightDiv, kuiLayout, leftContainer, patternComposerModel);
                    leftContainer.isHidden = true;

                    arrowMap.currentArrow = arrowMap.rightArrow; 
                    thisObj._setArrowSize(arrowMap, rightDiv);
                }
                else {

                    thisObj._showMenu(leftDiv, rightDiv, kuiLayout, leftContainer, 25, patternComposerModel);
                    leftContainer.isHidden = false;

                    arrowMap.currentArrow = arrowMap.leftArrow;
                    thisObj._setArrowSize(arrowMap, rightDiv);
                }
            }); 
        },

        _fullScreenClickEvent: function(fullScreenDiv, leftDiv, rightDiv, patternComposerModel){

            var thisObj = this;
            dojo.connect(fullScreenDiv, "onclick", function () {

                var leftContainer = registry.byId("leftContainer");
                var centerContainer = registry.byId("centerContainer");
                var kuiLayout = registry.byId("kuiLayout");

                if (!leftContainer.isFullScreen) { 

                    domStyle.set(leftContainer.domNode, 'max-width', '100%');  

                    thisObj._showMenu(leftDiv, rightDiv, kuiLayout, leftContainer, 100, patternComposerModel);

                    leftContainer.isFullScreen = true;  
                    kuiLayout.resize();
                }
                else {

                    domStyle.set(leftContainer.domNode, 'max-width', '400px');
                    leftContainer.isFullScreen = false;

                    thisObj._showMenu(leftDiv, rightDiv, kuiLayout, leftContainer, 25, patternComposerModel); 
                }  
            });         
        },

        _hideMenu: function(leftDiv, rightDiv, kuiLayout, leftContainer){

            var leftWidth = this._findPaneWidth(kuiLayout, 1);

            if (leftWidth < 25) {
                leftWidth = 18;
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

        _showMenu: function(leftDiv, rightDiv, kuiLayout, leftContainer, percentage, patternComposerModel){

            var leftWidth = this._findPaneWidth(kuiLayout, percentage);
            leftContainer.resize({ w: leftWidth });
         
            kuiLayout.resize();

            var rightDivWidth = 20;
            var leftDivWidth = (this._findPaneWidth(leftContainer, 100)) - rightDivWidth;
            
            domStyle.set(leftDiv, 'width', leftDivWidth + 'px');
            domStyle.set(rightDiv, 'width', rightDivWidth - 3 + 'px');

            patternComposerModel.timeline.svg.attr("width", leftDivWidth); 

            kuiLayout.resize();
        },

        _findPaneWidth: function (pane, percentage) {

            var width = domGeom.getContentBox(pane.domNode).w - domGeom.getContentBox(pane.domNode).l;
            var calculatedWidth = (percentage / 100) * width;

            return calculatedWidth;

        }
    });
});

