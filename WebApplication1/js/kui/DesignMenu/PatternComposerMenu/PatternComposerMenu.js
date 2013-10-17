

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
    "dijit/TitlePane"
 
],
    function (declare, html, CommonForm, dom, ContentPane, domStyle, domConstruct, three, on,
        domGeom, Moveable, SimSate, array, TitlePane) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu.PatternComposerMenu", null, {

            /*   
             */
            constructor: function (sceneModel, patternComposerModel, patternModel) {

                this.style = "background-color:transparent;";    
                this.patternModel = patternModel;
                this.patternComposerModel = patternComposerModel;
                

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

                this.createTimeline(container);
                this.createPatternProps(this.contentPane);
                

                
            },    

            createTimeline: function (container) {
               
                this._createPatternSection(this.contentPane);
                              
                var timelineDiv = html.createDiv("width:100%;" +
                    "height:500px;" +
                    "background-color:#141414;" +
                    "overflow:auto;");

                var timelineTitlePane = new TitlePane({    
                    title: "Pattern Timeline",
                    content: timelineDiv,
                    onShow: dojo.hitch(this, function(){
                        this.patternComposerModel.timeline.createCanvas(timelineDiv);
                        this.patternComposerModel.updateComposerFromServer();
                        container.simulation.setSimulationMode();
                    
                    })
                });
                timelineDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                this.patternComposerModel.timeline.createCanvas(timelineDiv);
                   
                domConstruct.place(timelineTitlePane.domNode, this.contentPane.domNode);                                  
            },

            _createPatternSection: function(container){

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
                    thisObj.patternComposerModel.addPattern(thisObj.patternComposerModel.patternListBox.getSelected());
                });

                CommonForm.setButtonStyle(addButton);
                domConstruct.place(addButton.domNode, div);

                var removeButton = CommonForm.createButton('Remove', function () {
                    thisObj.patternComposerModel.removePatternFromOption(thisObj.patternComposerModel.patternListBox.getSelected());
                });

                CommonForm.setButtonStyle(removeButton);
                domConstruct.place(removeButton.domNode, div); 
            },

            createPatternBox: function () {    

                var patternListBox = CommonForm.createListBox("width:90%;");
                this.patternComposerModel.patternListBox = patternListBox;

                return patternListBox.domNode;
            },

            createPatternProps: function (container) {

                var div = html.createDiv("width:100%;");   

                var patternPropertyTitlePane = new TitlePane({

                    title: "Pattern Properties",
                    content: div
                });
                div.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(patternPropertyTitlePane.domNode, container.domNode);

                var thisObj = this;
                var removeButton = CommonForm.createButton("Remove Pattern", function () {

                    thisObj.patternComposerModel.removeSelectedPattern();

                });
                CommonForm.setButtonStyle(removeButton);

                var applyButton = CommonForm.createButton("Apply All", function () {

                    thisObj.patternComposerModel.applyAllPatterns(); 

                });
                CommonForm.setButtonStyle(applyButton);
                 
                domConstruct.place(removeButton.domNode, div);
                domConstruct.place(applyButton.domNode, div); 
                domConstruct.place(patternPropertyTitlePane.domNode, container.domNode);
 
            }
                      
        });   
               
    });
    