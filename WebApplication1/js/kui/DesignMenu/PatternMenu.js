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
    "kui/DesignMenu/EffectMenu/EffectSection"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm, TitlePane,
    DropDownMenu, Effects, MenuItem, array, EffectSection) {
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
                               

            },

            createPatternMenu: function (container) {
                var contentPane = new ContentPane(
                  {
                      title: "Pattern Menu",
                      style: this.mainBackgroundColor

                  });

                container.addChild(contentPane);

            
                var patternSection = this._createPatternSection();
                domConstruct.place(patternSection, contentPane.domNode);

                var applySection = this._createApplyPattern();
                domConstruct.place(applySection, contentPane.domNode);

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
                       
                /*Effect Section*/
                var effectSection = new EffectSection();
                effectSection.placeAt(div);
               

                var effectPropPane = new TitlePane({

                    title: "Effect Properties",
                    content: effectSection.domNode

                });

                effectSection.domNode.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(effectPropPane.domNode, div);
                domConstruct.place(html.createDiv(spacerDivStyle), div);

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

                /*Create pattern button*/
                var createPatternButtonDiv = this._createPatternButtonSection();
                domConstruct.place(createPatternButtonDiv, div);
                domConstruct.place(html.createDiv(spacerDivStyle), div);

             
                return div;
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
                var obj = this;
                var createButton = CommonForm.createButton('Create Pattern', function () {


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

                var groupDropDown = CommonForm.createDropDown("Select Group");
                domConstruct.place(groupDropDown.domNode, dropDownCell);

                this.patternModel.groupDropDown = groupDropDown;
                this.patternModel.groupDropDownMenu = new DropDownMenu();
                this.patternModel.groupDropDown.set('dropDown', this.patternModel.groupDropDownMenu); 

                var addButton = CommonForm.createButton("+");
                domConstruct.place(addButton.domNode, addCell);

                var groupDiv = html.createDiv("width:100%;");
                domConstruct.place(groupDiv, div);

                var groupListBox = CommonForm.createListBox("width:90%;");
                domConstruct.place(groupListBox.domNode, groupDiv);

                this.patternModel.groupListBox = groupListBox;

                var groupButtonDiv = html.createDiv("width:100%;");
                domConstruct.place(groupButtonDiv, groupDiv);

                var addAllGroupButton = CommonForm.createButton("Add All");
                domConstruct.place(addAllGroupButton.domNode, groupButtonDiv);

                var removeGroupButton = CommonForm.createButton("Remove");
                domConstruct.place(removeGroupButton.domNode, groupButtonDiv);


                return div;
            },

            _createApplyPattern: function () {

                var div = html.createDiv();

                var patternApplyDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#383838;" +
                    "border-radisu:7px;");
                patternApplyDiv.innerHTML = "Apply Pattern";

                var innerDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;");

                var patternDropDown = CommonForm.createDropDown("Select Pattern", "");

                var patternDropDownDiv = html.createDiv("padding-top:10px;");
            
                domConstruct.place(patternDropDown.domNode, patternDropDownDiv);
                domConstruct.place(patternDropDownDiv, innerDiv);

                var obj = this;
                var applyButton = CommonForm.createButton('Apply Pattern', function () {


                });

                domConstruct.place(applyButton.domNode, innerDiv);

                domConstruct.place(patternApplyDiv, div);
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

                var nameField = CommonForm.createTextBox("", "Pattern Name", "width: 100%;");

                domConstruct.place(nameField.domNode, valueCell);

                return tableRow;
            },

            populatePatternDropDown: function () {

                var patterns = this.patterns.getPatterns();
                for (var i = 0; i < patterns.count; i++) {



                }


            }




        });

    });
