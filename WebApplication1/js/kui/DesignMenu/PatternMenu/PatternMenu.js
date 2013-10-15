/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacquimanzi@gmail.com
*/
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/util/CommonFormItems",
    "dijit/TitlePane",
    "dijit/DropDownMenu",
    "kui/ajax/Effects",
    "dijit/MenuItem",
    "dojo/_base/array",
    "kui/DesignMenu/EffectMenu/EffectMenu",
    "dojox/collections/ArrayList",
    "dojo/on",
    "kui/DesignMenu/AccordianItem" 
],
    function (declare, html, ContentPane, domStyle, domConstruct, three, CommonForm, TitlePane,
    DropDownMenu, Effects, MenuItem, array, EffectMenu, ArrayList, on,  AccordianItem) {
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
               
                domConstruct.place(this.domNode, container.domNode);
                on(this, "show", dojo.hitch(container.simulation, container.simulation.setPatternMode));
                //this.patternModel.updateGroupDropDown(); JMM---> what to do about this? Make this updateable
                
                var nameTableItems = [];
                nameTableItems.push(this._createPatternNameSection());
                nameTableItems.push(this._createPrioritySection());

                var patternPropDivs = this.createTitlePane("Pattern Properties");
                this.addTableItem(nameTableItems, patternPropDivs.contentDiv);
               
                var groupButtons = [];
                groupButtons.push(this._createGroupButtons());

                var removeButton = [];
                removeButton.push(this._createRemoveGroupButton());
                 
                var groupDivs = this.createTitlePane("Pattern Groups");
                this.addTableItem(groupButtons, groupDivs.contentDiv);
                this.addDivItem(this._createGroupListBox(), groupDivs.contentDiv); 
                this.addTableItem(removeButton, groupDivs.contentDiv);

                var patternCreationDivs = this.createTitlePane("Pattern Creation");
                this.addDivItem(this._createEffectSection(), patternCreationDivs.contentDiv); 
                this.addDivItem(this._createPatternButton(), patternCreationDivs.contentDiv);

                var patternSelectionButtons = [];
                patternSelectionButtons.push(this._createSelectPatternSection());
                var patternSelectionDivs = this.createTitlePane("Pattern Selection");
                this.addTableItem(patternSelectionButtons, patternSelectionDivs.contentDiv);
                 
                domConstruct.place(patternPropDivs.paneDiv, this.domNode);
                domConstruct.place(groupDivs.paneDiv, this.domNode);
                domConstruct.place(patternCreationDivs.paneDiv, this.domNode);
                domConstruct.place(patternSelectionDivs.paneDiv, this.domNode);
                 
                this.patternModel.dispatchUpdatesToMenuElementListeners();
            }, 
                                   
            _effectUpdated: function (patternDef) {

                this.patternModel.updatePatternDefinition(patternDef);
            },

            _createEffectSection: function(){

                var div = html.createDiv();

                var effectMenu = new EffectMenu();
                effectMenu.placeAt(div); 

                return {
                    valueContent: div
                }
            },
            
            _createPrioritySection: function()
            {

                var priorityDropDown = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");
                priorityDropDown.set('value', "0");
                 
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
                
                domConstruct.place(createButton.domNode, div);

                return {
                    valueContent: div
                } 
            },

            _createGroupButtons: function(){
                
                var groupDropDown = CommonForm.createDropDown("Select Group", "");

                this.patternModel.setGroupDropDown(groupDropDown);
                var addAllGroupButton = CommonForm.createButton("Add All", dojo.hitch(this, function () {

                    var groupList = this.patternModel.getGroupsFromSceneModel();
                    var thisObj = this;
                    array.forEach(groupList, function (groupName) {
                    thisObj.patternModel.addGroup(groupName); 
                    });      
                }));

                var addButton = CommonForm.createButton("+", dojo.hitch(this, function () {
                    this.patternModel.addGroup(this.patternModel.getGroupDropDownLabel());
                }));

                return {
                    groupDropDown: groupDropDown.domNode,
                    addButton: addButton.domNode,
                    addAllButton: addAllGroupButton.domNode
                }
            },

            _createGroupListBox: function(){

                var div = html.createDiv();
                var groupListBox = CommonForm.createListBox("width:90%;");               
                domConstruct.place(groupListBox.domNode, div);

                this.patternModel.setGroupListBox(groupListBox);

                return {
                    valueContent: div
                }
            },

            _createRemoveGroupButton: function(){

                var removeGroupButton = CommonForm.createButton("Remove", dojo.hitch(this, function () {

                    var optionList = this.patternModel.getGroupListBoxSelection();
                    var groupList = new ArrayList();
                    var thisObj = this;
                     array.forEach(optionList, function (option) {
 
                         var group = thisObj.patternModel.sceneInteraction.groupSet.nameToGroup[option.innerHTML];
                         groupList.add(group);
                     });        
 
                     this.patternModel.removeGroups(groupList);
                }));

                var removeAllGroupButton = CommonForm.createButton("Remove All", dojo.hitch(this, function () {

                    var groupList = this.patternModel.getGroups();
                    var newGroupList = new ArrayList();
                    groupList.forEach(function (group) {
                        newGroupList.add(group);
                    });
                    this.patternModel.removeGroups(newGroupList);
                }));
                
                return {                  
                    removeButton: removeGroupButton.domNode,
                    removeAllButton:removeAllGroupButton.domNode
                }
            }, 

            _createSelectPatternSection: function () {

                var patternDropDown = CommonForm.createDropDown("Select Pattern", "");
                this.patternModel.setPatternDropDown(patternDropDown);                 
              
                var applyButton = CommonForm.createButton('Apply Pattern', function () {
                    thisObj.patternModel.applyPattern();
                });

                var testButton = CommonForm.createButton('Test Pattern', function () {
                   // thisObj.patternModel.applyPattern();
                });

                return {
                    patternDropDown: patternDropDown.domNode,
                    applyButton: applyButton.domNode,
                    testButton: testButton.domNode
                };

            },

            _createPatternNameSection: function()
            {
                var nameField = CommonForm.createTextBox("", "Pattern Name", "text-align:left;width:100%;");
                this.patternModel.setNameField(nameField); //refactor this out and use update handlers instead

                return {
                    title: "Name",
                    valueContent: nameField.domNode          
                };                        
            }
                
        });

    });
