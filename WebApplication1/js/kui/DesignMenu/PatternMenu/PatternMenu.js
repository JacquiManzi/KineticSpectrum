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
                this.addDivItem(this._createSelectPatternSection(), patternPropDivs.contentDiv);

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
                 
                domConstruct.place(patternPropDivs.paneDiv, this.domNode);
                domConstruct.place(groupDivs.paneDiv, this.domNode);
                domConstruct.place(patternCreationDivs.paneDiv, this.domNode); 
            }, 
             
            _createPatternSection: function () {

                var spacerDivStyle = "width:100%;height:7px;";
                var div = html.createDiv("text-align:center;");
                var table = html.createTable(this.tableStyle);
             
                var patternPropPane = new TitlePane({

                    title: "Pattern Properties",
                    content: table                  
                });

                table.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(patternPropPane.domNode, div);

                /*Pattern Name text box*/
                var nameRow = this._createPatternNameSection();
                domConstruct.place(nameRow, table);
                domConstruct.place(html.createDiv(spacerDivStyle), div);

                /*Run time text box*/
                var runTimeRow = this._createRunTimeSection();
                domConstruct.place(runTimeRow, table);

                /*Priority Section*/
                var priorityRow = this._createPrioritySection();
                domConstruct.place(priorityRow, table);
                
                /*Group Section*/
                var groupSectionDiv = this._createGroupSection();
                domConstruct.place(groupSectionDiv, div);

                var groupPropPane = new TitlePane({

                    title: "Pattern Groups",
                    content: groupSectionDiv
                });

                groupSectionDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(groupPropPane.domNode, div);
                domConstruct.place(html.createDiv(spacerDivStyle), div);
                       
                /*Effect Menu*/
                var effectMenu = new EffectMenu();
                effectMenu.placeAt(div);
               
                var effectPropPane = new TitlePane({

                    title: "Effect Properties",
                    content: effectMenu.domNode
                });

                var createPatternButtonDiv = this._createPatternButtonSection();
                domConstruct.place(createPatternButtonDiv, effectPropPane.domNode);
                domConstruct.place(html.createDiv(spacerDivStyle), effectPropPane.domNode);

                effectMenu.domNode.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(effectPropPane.domNode, div);
                domConstruct.place(html.createDiv(spacerDivStyle), div);

                dojo.connect(effectMenu, "onUpdate", dojo.hitch(this, this._effectUpdated));

                /*Apply pattern button*/
                var applyDiv = html.createDiv(this.tableStyle);
                var applyPropPane = new TitlePane({

                    title: "Apply Pattern",
                    content: applyDiv
                });

                applyDiv.parentNode.setAttribute('style', this.mainBackgroundColor);

                var selectPatternSection = this._createSelectPatternSection();
                domConstruct.place(selectPatternSection, applyDiv);

                var thisObj = this;
                var testButton = CommonForm.createButton('Test Pattern', function () {
                    //thisObj.patternModel.applyPattern();
                });

                var removeButton = CommonForm.createButton('Remove Pattern', function () {
                    thisObj.patternModel.removePattern();
                });    

                var applyButton = CommonForm.createButton('Apply Pattern', function () {
                    thisObj.patternModel.applyPattern();
                });

                var topSpacerDiv = html.createDiv("height:20px");
                var bottomSpacerDiv = html.createDiv("height:20px");

                CommonForm.setButtonStyle(applyButton);   
               // CommonForm.setButtonStyle(testButton);
                CommonForm.setButtonStyle(removeButton); 

               // domConstruct.place(testButton.domNode, applyDiv);
                domConstruct.place(applyButton.domNode, applyDiv);
                domConstruct.place(topSpacerDiv, applyDiv);    
                domConstruct.place(removeButton.domNode, applyDiv); 
                domConstruct.place(bottomSpacerDiv, applyDiv); 
                domConstruct.place(applyPropPane.domNode, div); 

                return div;    
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
                 

               // this.patternModel.priorityDropDown = priorityDropDown;


                return {
                    title: "Priority",
                    valueContent: priorityDropDown.domNode 
                }
            },


            //@TODO: Jacqui -> remove
           /* _createRunTimeSection: function()
            {
              
                var timeRow = html.createRow();
                var timeTitle = html.createCell(this.titleCellStyle);

                timeTitle.innerHTML = "Run Time (ms)";

                var timeValue = html.createCell("text-align:left;" +
                    "width:100%;");
                var timeBox = CommonForm.createTableNumberTextBox("width:100%;");

                domConstruct.place(timeTitle, timeRow);
                domConstruct.place(timeValue, timeRow);
                domConstruct.place(timeBox.domNode, timeValue);

                return timeRow;
            },*/

            _createPatternButton: function()
            {
                var div = html.createDiv();
                var thisObj = this;
                var createButton = CommonForm.createButton('Create Pattern', function () {
 
                    //thisObj.patternModel.createPattern();
                });
                
                domConstruct.place(createButton.domNode, div);

                return {
                    valueContent: div
                } 
            },

            _createGroupButtons: function(){
                
                var groupDropDown = CommonForm.createDropDown("Select Group", "");

                //this.patternModel.groupDropDown = groupDropDown;
                //this.patternModel.updateGroupDropDown();
                var addAllGroupButton = CommonForm.createButton("Add All", dojo.hitch(this, function () {

                    //var groupList = this.patternModel.sceneInteraction.groupSet.getGroups();
                    //var thisObj = this;
                    //array.forEach(groupList, function (groupName) {
                    //     thisObj.patternModel.addGroup(groupName); 
                    //});      
                }));

                var addButton = CommonForm.createButton("+", dojo.hitch(this, function () {
                    //this.patternModel.addGroup(this.patternModel.groupDropDown.get('label'));
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

                return {
                    valueContent: div
                }
            },

            _createRemoveGroupButton: function(){

                var removeGroupButton = CommonForm.createButton("Remove", dojo.hitch(this, function () {

                    //var optionList = this.patternModel.groupListBox.getSelected();
                    // var groupList = new ArrayList();
                    /* var thisObj = this;
                     array.forEach(optionList, function (option) {
 
                         var group = thisObj.patternModel.sceneInteraction.groupSet.nameToGroup[option.innerHTML];
                         groupList.add(group);
                     });        
 
                     this.patternModel.removeGroups(groupList);*/
                }));

                var removeAllGroupButton = CommonForm.createButton("Remove All", dojo.hitch(this, function () {

                   /* var groupList = this.patternModel.getGroups();
                    var newGroupList = new ArrayList();
                    groupList.forEach(function (group) {
                        newGroupList.add(group);
                    });
                    this.patternModel.removeGroups(newGroupList);*/
                }));
                
                return {                  
                    removeButton: removeGroupButton.domNode,
                    removeAllButton:removeAllGroupButton.domNode
                }
            }, 
        
            _createGroupSection: function()
            {
                var div = html.createDiv(this.tableStyle);

                var table = html.createTable("margin-right:auto;margin-left:auto;");
                domConstruct.place(table, div);

                var row = html.createRow();
                domConstruct.place(row, table);

                var dropDownCell = html.createCell();
                domConstruct.place(dropDownCell, row);

                var addCell = html.createCell();
                domConstruct.place(addCell, row);

                var groupDropDown = CommonForm.createDropDown("Select Group", "");
                domConstruct.place(groupDropDown.domNode, dropDownCell);
               
                var addButton = CommonForm.createButton("+", dojo.hitch(this, function () {
                    this.patternModel.addGroup(this.patternModel.groupDropDown.get('label'));
                }));
                domConstruct.place(addButton.domNode, addCell);
                
                var groupDiv = html.createDiv("width:100%;");
                domConstruct.place(groupDiv, div);

                var groupListBox = CommonForm.createListBox("width:90%;");
                domConstruct.place(groupListBox.domNode, groupDiv);
               
                this.patternModel.groupListBox = groupListBox;

                var groupButtonDiv = html.createDiv("width:100%;");
                domConstruct.place(groupButtonDiv, groupDiv);

                var addAllGroupButton = CommonForm.createButton("Add All", dojo.hitch(this, function () {

                    var groupList = this.patternModel.sceneInteraction.groupSet.getGroups();
                    var thisObj = this;
                    array.forEach(groupList, function (groupName) {
                        thisObj.patternModel.addGroup(groupName); 
                    });      
                }));
                domConstruct.place(addAllGroupButton.domNode, groupButtonDiv);

                var removeGroupButton = CommonForm.createButton("Remove", dojo.hitch(this, function () {

                    var optionList = this.patternModel.groupListBox.getSelected();
                    var groupList = new ArrayList();
                    var thisObj = this;
                    array.forEach(optionList, function (option) {

                        var group = thisObj.patternModel.sceneInteraction.groupSet.nameToGroup[option.innerHTML];
                        groupList.add(group);
                    });        

                    this.patternModel.removeGroups(groupList);
                }));
                domConstruct.place(removeGroupButton.domNode, groupButtonDiv);

                var removeAllGroupButton = CommonForm.createButton("Remove All", dojo.hitch(this, function () {

                    var groupList = this.patternModel.getGroups();
                    var newGroupList = new ArrayList();
                    groupList.forEach(function (group) {
                        newGroupList.add(group);
                    });
                    this.patternModel.removeGroups(newGroupList);    
                }));
                domConstruct.place(removeAllGroupButton.domNode, groupButtonDiv);

                return div;
            },
            

            _createSelectPatternSection: function () {

                var div = html.createDiv();

                var innerDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;");

                var patternDropDown = CommonForm.createDropDown("Select Pattern", "");
                //this.patternModel.patternDropDown = patternDropDown;
               // this.patternModel.updatePatternDropDown();
                 
                //CommonForm.setButtonStyle(patternDropDown); 

                var patternDropDownDiv = html.createDiv("padding-top:10px;");
            
                domConstruct.place(patternDropDown.domNode, patternDropDownDiv);
                domConstruct.place(patternDropDownDiv, innerDiv);

               // var obj = this;
                
                domConstruct.place(innerDiv, div);

               // return div;

                return {
                    valueContent: div 
                };

            },

            _createPatternNameSection: function()
            {
                //this.patternModel.nameField = nameField; //refactor this out and use update handlers instead

                return {
                    title: "Name",
                    valueContent: CommonForm.createTextBox("", "Pattern Name", "text-align:left;width:100%;").domNode                 
                };                        
            }
                
        });

    });
