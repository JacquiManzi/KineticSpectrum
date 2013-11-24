/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacqui@revkitt.com
*/
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom-style",
    "dojo/dom-construct",
    "kui/util/CommonFormItems",
    "dijit/DropDownMenu",
    "kui/ajax/Effects",
    "dojo/_base/array",
    "kui/DesignMenu/EffectMenu/EffectMenu",
    "dojox/collections/ArrayList",
    "dojo/on",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class",
    "kui/DesignMenu/GroupBox"
],
    function (declare, html, domStyle, domConstruct, CommonForm, DropDownMenu, Effects, array,
        EffectMenu, ArrayList, on, AccordianItem, domClass, GroupBox) {
        "use strict";
        return declare("kui.DesignMenu.PatternMenu.PatternMenu", AccordianItem, {

            constructor: function () {
                
                this.title = "Pattern Menu";
            },

            createPatternMenu: function (container) {

               /* var contentPane = new ContentPane(
                  {
                      title: "Pattern Menu",
                      style: this.mainBackgroundColor,
                      onShow: dojo.hitch(this, function() {
                          this.patternModel.updateGroupDropDown();
                          //TODO: JCU, JMM: This is why sample patterns don't work anymore!
                         // container.simulation.setPatternMode();
                      })
                       
                  });

                container.addChild(contentPane);

                
            
                var patternSection = this._createPatternSection();
               */

                this.onShow = function () {

                    //this.patternModel.updateGroupDropDown();
                    container.simulation.setPatternMode();
                };
               
                domConstruct.place(this.domNode, container.domNode);
                on(this, "show", dojo.hitch(container.simulation, container.simulation.setPatternMode));
                //this.patternModel.updateGroupDropDown(); JMM---> what to do about this? Make this updateable
                
                              
                this._createPatternProps();
                this._createPatternGroups();
                this._createPatternCreation();
                this._createPatternSelection();
                 
                this.patternModel.dispatchUpdatesToMenuElementListeners();

                domClass.add(this.domNode, "designMenu");
            },

            _createPatternProps: function () {

                var nameTableItems = [];
                nameTableItems.push(this._createPatternNameSection());
                nameTableItems.push(this._createPrioritySection());

                var patternPropDivs = this.createTitlePane("Pattern Properties");
                this.addTableItem(nameTableItems, patternPropDivs.contentDiv);
                domConstruct.place(patternPropDivs.paneDiv, this.domNode);
            },

            _createPatternGroups: function(){

                var groupDivs = this.createTitlePane("Pattern Groups");
                this.groupBox = new GroupBox({ sceneModel: this.sceneModel });
                this.groupBox.placeAt(groupDivs.contentDiv);

                domConstruct.place(groupDivs.paneDiv, this.domNode);
            },

            _createPatternCreation: function(){

                var patternCreationDivs = this.createTitlePane("Pattern Creation");
                this.addDivItem(this._createEffectSection(), patternCreationDivs.contentDiv);
                this.addDivItem(this._createPatternButton(), patternCreationDivs.contentDiv);

                domConstruct.place(patternCreationDivs.paneDiv, this.domNode);
            },

            _createPatternSelection: function(){

                var patternSelectionDivs = this.createTitlePane("Pattern Selection");

                this.addDivItem(this._createSelectPatternDropDown(), patternSelectionDivs.contentDiv); 
                var patternSelectionButtons = [];
                patternSelectionButtons.push(this._createApplyPatternSection());               
                this.addTableItem(patternSelectionButtons, patternSelectionDivs.contentDiv);

                domConstruct.place(patternSelectionDivs.paneDiv, this.domNode);
            },
                                   
            _effectUpdated: function (patternDef) {

                this.patternModel.updatePatternDefinition(patternDef);
            },

            _createEffectSection: function(){

                var div = html.createDiv();

                var effectMenu = new EffectMenu();
                effectMenu.placeAt(div);
                on(effectMenu, "update", dojo.partial(function(model, value) {
                    model.patternDef = value;
                }, this.patternModel));

                return {
                    valueContent: div
                };
            },
            
            _createPrioritySection: function()
            {

                var priorityDropDown = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");
                priorityDropDown.set('value', "0");
                on(priorityDropDown, "change", dojo.partial(function(model, value) {
                    model.patternPriority = value;
                }, this.patternModel));
                 
                this.patternModel.setPriorityDropDown(priorityDropDown);

                return {
                    title: "Priority",
                    valueContent: priorityDropDown.domNode 
                }
            },

            _createPatternButton: function()
            {
                var div = html.createDiv();
                var thisObj = this;
                var createButton = CommonForm.createButton('Create Pattern', function () { 
                    thisObj.patternModel.createPattern();
                });

                CommonForm.setButtonStyle(createButton, 50);               
                domConstruct.place(createButton.domNode, div);

                return {
                    valueContent: div
                };
            },

            _createGroupButtons: function(){
                
                var groupDropDown = CommonForm.createDropDown("Select Group", "");
                CommonForm.setButtonStyle(groupDropDown, 90); 

                this.patternModel.setGroupDropDown(groupDropDown);
                var addAllGroupButton = CommonForm.createButton("Add All", dojo.hitch(this, function () {

                    var groupList = this.patternModel.getGroupsFromSceneModel();
                    var thisObj = this;
                    array.forEach(groupList, function (groupName) {
                    thisObj.patternModel.addGroup(groupName); 
                    });      
                }));

               CommonForm.setButtonStyle(addAllGroupButton, 80);
                 
                var addButton = CommonForm.createButton("+", dojo.hitch(this, function () {
                    this.patternModel.addGroup(this.patternModel.getGroupDropDownLabel());
                }));

                CommonForm.setButtonStyle(addButton, 90);

                return {
                    groupDropDown: groupDropDown.domNode,
                    addButton: addButton.domNode,
                    addAllButton: addAllGroupButton.domNode
                };
            },

            _createSelectPatternDropDown: function(){

                var patternDropDown = CommonForm.createDropDown("Select Pattern", "margin-top:10px;");
                CommonForm.setButtonStyle(patternDropDown, 90);
                this.patternModel.setPatternDropDown(patternDropDown);

                return {
                    valueContent: patternDropDown.domNode
                };
            },

            _createApplyPatternSection: function () {

                var thisObj = this;
                var applyButton = CommonForm.createButton('Apply Pattern', function () {
                    thisObj.patternModel.applyPattern(thisObj.groupBox.getS);
                });

                CommonForm.setButtonStyle(applyButton, 90);

                var testButton = CommonForm.createButton('Test Pattern', function () {
                   thisObj.patternModel.applyPattern();
                });
                CommonForm.setButtonStyle(testButton, 90);

                return {                 
                    applyButton: applyButton.domNode,
                    testButton: testButton.domNode
                };

            },

            _createPatternNameSection: function() {
                var onChange = function(patternModel, value) {
                    patternModel.patternName = value;
                };
                var nameField = CommonForm.createTextBox("", "Pattern Name",
                    "text-align:left;width:100%;");
//                this.patternModel.setNameField(nameField); //refactor this out and use update handlers instead
                on(nameField, "change", dojo.partial(onChange, this.patternModel));

                return {
                    title: "Name",
                    valueContent: nameField.domNode          
                };                        
            }
                
        });

    });
