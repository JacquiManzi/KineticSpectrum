

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/on",
    "dojo/dom-geometry",
    "dojo/dnd/Moveable",
    "kui/ajax/SimState",
    "dojo/_base/array",
    "dijit/TitlePane",
    "kui/DesignMenu/ComposerModel"
 
],
    function (declare, html, CommonForm, dom, ContentPane, domStyle, domConstruct, three, on,
        domGeom, Moveable, SimSate, array, TitlePane, ComposerModel) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu", null, {

            /*   
             */
            constructor: function (modelView) {

                this.style = "background-color:transparent;";    
                this.patternModel = modelView.sceneInteraction.patternModel;
                this.composerModel = modelView.sceneInteraction.composerModel;
                

                this.mainBackgroundColor = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                         "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                         "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                         "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                         "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                         "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
               "background-color: #131313;" +
               "background-size: 20px 20px;";
            },

            createComposerMenu: function (container) {
                this.contentPane = new ContentPane(
                {
                    title: "Pattern Composer",
                    style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;width:100%;height:100%;text-align:center;"

                });    

                container.addChild(this.contentPane);

                this.createTimeline(); 
            },    

            createTimeline: function () {
               
                this.createPatternSection(this.contentPane);
                              
                var timelineDiv = html.createDiv("width:100%;height:500px;background-color:#141414;");

                var timelineTitlePane = new TitlePane({    
                    title: "Pattern Timeline",
                    content: timelineDiv,
                    onShow: dojo.hitch(this.composerModel.timeline, this.composerModel.timeline.createCanvas, timelineDiv)
                });
                timelineDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                this.composerModel.timeline.createCanvas(timelineDiv); 
                   
                domConstruct.place(timelineTitlePane.domNode, this.contentPane.domNode);                                  
            },

            createPatternSection: function(container){

                var div = html.createDiv("background-color:#141414;"); 
                domConstruct.place(this.createPatternBox(), div);

                var patternTitlePane = new TitlePane({
                    title: "Pattern Selection", 
                    content: div 
                });
                div.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(patternTitlePane.domNode, container.domNode);

                var thisObj = this;
                var addButton = CommonForm.createButton('Add', function () {

                    thisObj.composerModel.addPatternFromOption(thisObj.composerModel.patternListBox.getSelected());
                });
                domConstruct.place(addButton.domNode, div);
                var removeButton = CommonForm.createButton('Remove'); 
                domConstruct.place(removeButton.domNode, div); 
            },

            createPatternBox: function () {    

                var patternListBox = CommonForm.createListBox("width:90%;");
                this.composerModel.patternListBox = patternListBox;

                this.composerModel.updatePatternListBox();
               
                return patternListBox.domNode;
            }
                    
        });   
               
    });
