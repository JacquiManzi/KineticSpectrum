
/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacqui@revkitt.com
*
*   PatternComposerMenu.js - The sub accordian item for the pattern composer. Located in Design Menu.
*/

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/dom-geometry",
    "dojo/dnd/Moveable",
    "kui/ajax/SimState",
    "dojo/_base/array",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class",
    "dojo/dom-style"
],
    function (declare, html, CommonForm, domConstruct, on, domGeom, Moveable,
        SimSate, array, AccordianItem, domClass, domStyle) {

        return declare("kui.DesignMenu.PatternComposerMenu.PatternComposerMenu", AccordianItem, {

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
                    this.patternComposerModel.updatePatternListBox();
                    this.sceneModel.simulation.setSimulationMode();
                };

                domClass.add(this.domNode, "designMenu"); 
            }, 

            _createTimeline: function(){

                var timelineDivs = this.createTitlePane("Pattern Timeline");
                domStyle.set(timelineDivs.paneDiv, 'max-width', '100%');
                domStyle.set(timelineDivs.contentDiv, 'max-width', '100%');
                this.patternComposerModel.timeline.createCanvas(timelineDivs.contentDiv);

                domConstruct.place(timelineDivs.paneDiv, this.domNode);

                return timelineDivs.contentDiv;
            },

            _createPatternSection: function(){

                var patternButtonTableItems = []; 

                var patternDivs = this.createTitlePane("System Patterns");
                this.addDivItem(this._createPatternBox(), patternDivs.contentDiv);
                patternButtonTableItems.push(this._createPatternButtons());
                this.addTableItem(patternButtonTableItems, patternDivs.contentDiv);

                domConstruct.place(patternDivs.paneDiv, this.domNode);
            },

            _createPatternBox: function(){

                var div = html.createDiv();
                var patternListBox = CommonForm.createListBox("width:90%;" +
                    "background-color:black;" +
                    "color:rgb(61, 141, 213);");

                domClass.add(patternListBox.domNode, "designMenu");               
                domConstruct.place(patternListBox.domNode, div);

                this.patternComposerModel.patternListBox = patternListBox;

                return{
                    valueContent: div
                }             
            },
           
            _createPatternButtons: function(){
               
                var thisObj = this;
                var addButton = CommonForm.createButton('Add to Timeline', function () {
                    thisObj.patternComposerModel.addPattern(thisObj.patternComposerModel.patternListBox.getSelected());
                });

                CommonForm.setButtonStyle(addButton, 90);
             
                var removeButton = CommonForm.createButton('Remove from System', function () {
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
                var removeButton = CommonForm.createButton("Remove from Timeline", function () {

                    thisObj.patternComposerModel.removeSelectedPattern();
                });
                CommonForm.setButtonStyle(removeButton, 90);

                var applyButton = CommonForm.createButton("Apply All", function () {

                    thisObj.patternComposerModel.applyAllPatterns();
                });
                CommonForm.setButtonStyle(applyButton, 80);

                return {

                    removeButton: removeButton.domNode,
                    applyButton: applyButton.domNode
                }
            }
                      
        });   
               
    });
    