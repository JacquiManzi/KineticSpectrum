

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/dom-geometry",
    "dojo/dnd/Moveable",
    "kui/ajax/SimState",
    "dojo/_base/array",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class"
 
],
    function (declare, html, CommonForm, domStyle, domConstruct, on, domGeom, Moveable,
        SimSate, array, AccordianItem, domClass) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu.PatternComposerMenu", AccordianItem, {

            /*   
             */
            constructor: function () {
            
                this.title = "Pattern Composer";
                
            },

            createComposerMenu: function (container) {
               
                domConstruct.place(this.domNode, container.domNode);
               
                this._createPatternSection();
                var timelineDiv = this._createTimeline(container);
                this._createPatternprops();
                
                this.onShow = function () {

                    this.patternComposerModel.timeline.createCanvas(timelineDiv);
                    this.patternComposerModel.updateComposerFromServer();
                    this.sceneModel.simulation.setSimulationMode();
                };                
            },

            _createTimeline: function(){

                var timelineDivs = this.createTitlePane("Pattern Timeline");
                this.patternComposerModel.timeline.createCanvas(timelineDivs.contentDiv);

                domConstruct.place(timelineDivs.paneDiv, this.domNode);

                return timelineDivs.contentDiv;
            },

          /*  createTimeline: function (container) {
               
                //this._createPatternSection(this.contentPane);
                              
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
            },*/

            _createPatternSection: function(){

                var patternButtonTableItems = []; 

                var patternDivs = this.createTitlePane("Add Pattern");
                this.addDivItem(this._createPatternBox(), patternDivs.contentDiv);
                patternButtonTableItems.push(this._createPatternButtons());
                this.addTableItem(patternButtonTableItems, patternDivs.contentDiv);

                domConstruct.place(patternDivs.paneDiv, this.domNode);
            },

            _createPatternBox: function(){

                var div = html.createDiv();
                var patternListBox = CommonForm.createListBox("width:90%;");
                this.patternComposerModel.patternListBox = patternListBox;

                domConstruct.place(patternListBox.domNode, div);

                return{
                    valueContent: div
                }             

            },

            _createPatternButtons: function(){
               
                var thisObj = this;
                var addButton = CommonForm.createButton('Add', function () {
                    thisObj.patternComposerModel.addPattern(thisObj.patternComposerModel.patternListBox.getSelected());
                });

                CommonForm.setButtonStyle(addButton, 90);
             
                var removeButton = CommonForm.createButton('Remove', function () {
                    thisObj.patternComposerModel.removePatternFromOption(thisObj.patternComposerModel.patternListBox.getSelected());
                });

                CommonForm.setButtonStyle(removeButton, 90);

                return {               
                    addButton: addButton.domNode,
                    removeButton: removeButton.domNode
                }

            },

            _createPatternprops: function(){

                var patternPropButtons = [];

                var patternPropDivs = this.createTitlePane("Timeline Properties");
                patternPropButtons.push(this._createPatternPropButtons());
                this.addTableItem(patternPropButtons, patternPropDivs.contentDiv);

                domConstruct.place(patternPropDivs.paneDiv, this.domNode);
            },

            _createPatternPropButtons: function(){

                var thisObj = this;
                var removeButton = CommonForm.createButton("Remove Pattern", function () {

                    thisObj.patternComposerModel.removeSelectedPattern();

                });
                CommonForm.setButtonStyle(removeButton, 90);

                var applyButton = CommonForm.createButton("Apply All", function () {

                    thisObj.patternComposerModel.applyAllPatterns();

                });
                CommonForm.setButtonStyle(applyButton, 90);

                return {

                    removeButton: removeButton.domNode,
                    applyButton: applyButton.domNode
                }
            }
                      
        });   
               
    });
    