/// <reference path="LEDMenu.js" />
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
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
    "kui/DesignMenu/EffectMenu/EffectSection",
    "dojox/collections/ArrayList"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm, TitlePane,
    DropDownMenu, Effects, MenuItem, array, EffectSection, ArrayList) {
        "use strict";
        return declare("kui.DesignMenu.PatternMenu", null, {

            /*
             *   
             *
             */

            constructor: function (modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;
                this.patternModel = this.modelView.sceneInteraction.patternModel;
                
                this.backgroundColor = "#141414";
                this.textColor = "#3d8dd5";
                this.tableStyle = "margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:5px;" +
                    "padding-bottom:5px;" +
                    "background-color:" + this.backgroundColor + ";" +
                    "border-radius: 7px;" +
                    "border: 3px solid #333333;" +
                    "width:99%;" +
                    "color:" + this.textColor + ";";

                this.titleCellStyle = "background-color: #232323;" +
                    "border-radius:5px;"+
                    "width: 40%;";

                this.mainBackgroundColor = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;";

                this.patternModel.simulation = modelView.simulation;

            },

            createPatternMenu: function (container) {

                var contentPane = new ContentPane(
                  {
                      title: "Pattern Menu",
                      style: this.mainBackgroundColor,
                      onShow: dojo.hitch(this, function() {
                          this.patternModel.updateGroupDropDown();
                          container.simulation.setPatternMode();
                      })
                      
                  });

                container.addChild(contentPane);

            
                var patternSection = this._createPatternSection();
                domConstruct.place(patternSection, contentPane.domNode);

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
                       
                /*Effect Section*/
                var effectSection = new EffectSection();
                effectSection.placeAt(div);
               

                var effectPropPane = new TitlePane({

                    title: "Effect Properties",
                    content: effectSection.domNode

                });

                var createPatternButtonDiv = this._createPatternButtonSection();
                domConstruct.place(createPatternButtonDiv, effectPropPane.domNode);
                domConstruct.place(html.createDiv(spacerDivStyle), effectPropPane.domNode);

                effectSection.domNode.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(effectPropPane.domNode, div);
                domConstruct.place(html.createDiv(spacerDivStyle), div);

                dojo.connect(effectSection, "onUpdate", dojo.hitch(this, this._effectUpdated));

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
                var applyButton = CommonForm.createButton('Apply Pattern', function () {
                    thisObj.patternModel.applyPattern();
                });

                CommonForm.setButtonStyle(applyButton);

                domConstruct.place(applyButton.domNode, applyDiv);
                domConstruct.place(applyPropPane.domNode, div); 

                return div;    
            },
            
            _effectUpdated: function (patternDef) {

                this.patternModel.updatePatternDefinition(patternDef);
            },
            
            _createPrioritySection: function()
            {
                var row = html.createRow();

                var titleCell = html.createCell(this.titleCellStyle);
                titleCell.innerHTML = "Priority";
                domConstruct.place(titleCell, row);

                var valueCell = html.createCell();
                domConstruct.place(valueCell, row);

                var priorityDropDown = CommonForm.createDropDown("0");
                domConstruct.place(priorityDropDown.domNode, valueCell);

                this.patternModel.priorityDropDown = priorityDropDown;

                return row;
            },

            _createRunTimeSection: function()
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
            },

            _createPatternButtonSection: function()
            {
                var div = html.createDiv();
                var thisObj = this;
                var createButton = CommonForm.createButton('Create Pattern', function () {
 
                    thisObj.patternModel.createPattern();
                });
                
                domConstruct.place(createButton.domNode, div);

                return div;
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

                this.patternModel.groupDropDown = groupDropDown;
                this.patternModel.updateGroupDropDown();
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
                this.patternModel.patternDropDown = patternDropDown;
                this.patternModel.updatePatternDropDown();
                 
                CommonForm.setButtonStyle(patternDropDown); 

                var patternDropDownDiv = html.createDiv("padding-top:10px;");
            
                domConstruct.place(patternDropDown.domNode, patternDropDownDiv);
                domConstruct.place(patternDropDownDiv, innerDiv);

                var obj = this;
                
                domConstruct.place(innerDiv, div);

                return div;

            },

            _createPatternNameSection: function()
            {
                var tableRow = html.createRow();
                var titleCell = html.createCell(this.titleCellStyle);
                var valueCell = html.createCell("text-align: left;width:60%;");

                domConstruct.place(titleCell, tableRow);
                domConstruct.place(valueCell, tableRow)

                titleCell.innerHTML = "Name: ";

                var nameField = CommonForm.createTextBox("", "Pattern Name", "width: 100%;", dojo.hitch(this,function () {                   
                }));

                this.patternModel.nameField = nameField;

                domConstruct.place(nameField.domNode, valueCell);

                return tableRow;
            }

        });

    });
