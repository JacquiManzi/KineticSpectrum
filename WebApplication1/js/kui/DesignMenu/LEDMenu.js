﻿/// <reference path="../ModelView/SceneInteraction.js" />


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
    "dojo/on",
    "dojo/_base/array",
    "kui/ajax/Scenes",
    "kui/LED/LightAddress"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm, TitlePane, on, array, Scenes, LightAddress) {
        "use strict";
        return declare("kui.DesignMenu.LEDMenu", null, {
                   
            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function (modelView) {

                this.style = "background-color:#141414;";
                this.modelView = modelView;

                /*CSS Stylings*/
                this.tableCellBorderColor = "#333333";

                this.mainBackgroundColor = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                         "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                         "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                         "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                         "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                         "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
               "background-color: #131313;" +
               "background-size: 20px 20px;";


            },

            createLEDMenu: function (container) {
                var contentPane = new ContentPane(
                  {
                      title: "LED Menu",
                      style: this.mainBackgroundColor,
                      onShow: dojo.hitch(container.simulation, container.simulation.setSceneMode)

                  });

                var spacerDivStyle = "width:100%;height:7px;";

                container.addChild(contentPane);

                var titlePaneDiv = html.createDiv("width:100%;height:100%;");
                domConstruct.place(titlePaneDiv, contentPane.domNode);

                var nodeDiv = this.createNodeSection();
                              
                /*Node Creation Section*/
                var nodePane = new TitlePane({

                    title: "Create / Select Nodes",
                    content: nodeDiv

                });

                nodeDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(nodePane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);

                          
                /*Node Removal Section*/
                var removalDiv = this.createRemoveNodeSection();

                var removePane = new TitlePane({

                    title: "Remove Nodes",
                    content: removalDiv

                });

                removalDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(removePane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);

                /*Group Section*/            
                var groupDiv = this.createGroupSection();

                var groupPane = new TitlePane({

                    title: "Group Nodes",
                    content: groupDiv

                });

                groupDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(groupPane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);
            },

            createNodeSection: function () {

                var nodeDiv = html.createDiv("text-align:center;" +
                   // "color:#3d8dd5;" +
                   "color: #f1f1f1;"+
                    "width:100%;" +
                    "height:100%;");
                
                var table = html.createTable("margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:10px;"+
                    "padding-bottom:10px;"+
                    "border-spacing: 0px;"+
                    "width:95%;");

                var tableDiv = html.createDiv("width:100%;" +                    
                    "margin-left:auto;" +
                    "margin-right:auto;"+
                    "border-radius:10px;");

                this.createNodeAmountBox(table);

                var ledModeDiv = html.createDiv("text-align:center;padding-top:20px;");
                domConstruct.place(ledModeDiv, nodeDiv);
                var obj = this;
                var ledModeButton = CommonForm.createButton("Add Single LED OFF", function () {

                    obj.modelView.sceneInteraction.setAddMode(this, obj.getAddress);

                }, null, "color:#3d8dd5;");

                this.setButtonStyle(ledModeButton);
                domConstruct.place(ledModeButton.domNode, ledModeDiv);

                domConstruct.place(table, tableDiv);
                domConstruct.place(tableDiv, nodeDiv);

                var selectionDiv = this.createSelectAllSection();
                domConstruct.place(selectionDiv, nodeDiv);

                return nodeDiv;
            },

            createRemoveNodeSection: function () {
                var innerDiv = html.createDiv("text-align:center;" +
                   "color:#3d8dd5;"+
                   "padding-top: 10px;"+
                   "padding-bottom: 10px;");

                this.createRemoveButton(innerDiv);


                return innerDiv;
            },

            createNodeAmountBox: function (table) {
              
               
                var titleCell = html.createCell(
                    //"border-bottom-left-radius: 7px;" +
                    //"border-top-left-radius: 7px;" +
                    //"border-left: 2px solid" + this.tableCellBorderColor+";"+
                    //"border-top: 2px solid" + this.tableCellBorderColor + ";" +
                    //"border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    //"color:#3d8dd5;" +
                    //"text-align: right;"+
                    //"width:30%;"
                    );

                var valueRow = html.createRow("background-color:#141414;");

                var valueCell = html.createCell(
                    //"text-align:center;" +
                    //"border-top: 2px solid #2d2d2d;" +
                    //"border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    //"width:40%;"
                    );

                var checkButtonCell = html.createCell(
                    //"border-top-right-radius: 7px;" +
                    //"border-bottom-right-radius: 7px;"+
                    //"border-top-right-radius: 7px;" +
                    //"border-right: 2px solid" + this.tableCellBorderColor + ";" +
                    //"border-top: 2px solid" + this.tableCellBorderColor + ";" +
                    //"border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    //"text-align:left;"+
                    //"width:30%;"
                    );
    
                var nodeNumberTextBox = CommonForm.createTableNumberTextBox("width:90%;");
                var ledAddressText = CommonForm.createTableNumberTextBox("width:90%");
                var portText = CommonForm.createTableNumberTextBox("width:90%");
                var fixtureText = CommonForm.createTableNumberTextBox("width:90%");

                this.getAddress = dojo.hitch(this, function() {
                    return new LightAddress({
                        lightNo: ledAddressText.get('value'),
                        fixtureNo: fixtureText.get('value'),
                        portNo: portText.get('value')
                    });
                });

                var obj = this;
                var checkButton = CommonForm.createButton('Add', function () {

                    var lineSegments = obj.modelView.sceneInteraction.findConnectingLines(nodeNumberTextBox.get('value'));

                   obj.modelView.sceneInteraction.drawNodes(lineSegments, obj.getAddress());
                          
                });

                titleCell.innerHTML = "#Nodes";

                var addressRow = html.createRow();
                var addressTitleCell = html.createCell();
                var addressCell = html.createCell();

                addressTitleCell.innerHTML = "#Address";
                

                var portRow = html.createRow();
                var portTitleCell = html.createCell();
                var portCell = html.createCell();
                

                portTitleCell.innerHTML = "#Port";

                var fixtureRow = html.createRow();
                var fixtureTitleCell = html.createCell();
                var fixtureCell = html.createCell();
                
                fixtureTitleCell.innerHTML = "#Fixture";


                domConstruct.place(titleCell, valueRow);
                domConstruct.place(valueRow, table);
                domConstruct.place(valueCell, valueRow);
                domConstruct.place(checkButton.domNode, checkButtonCell);
                domConstruct.place(checkButtonCell, valueRow);
                domConstruct.place(nodeNumberTextBox.domNode, valueCell);

                domConstruct.place(addressTitleCell, addressRow);
                domConstruct.place(ledAddressText.domNode, addressCell); 
                domConstruct.place(addressCell, addressRow);
                domConstruct.place(addressRow, table);

                domConstruct.place(portTitleCell, portRow);
                domConstruct.place(portCell, portRow);
                domConstruct.place(portText.domNode, portCell);
                domConstruct.place(portRow, table);

                domConstruct.place(fixtureTitleCell, fixtureRow);
                domConstruct.place(fixtureCell, fixtureRow);
                domConstruct.place(fixtureText.domNode, fixtureCell);
                domConstruct.place(fixtureRow, table);    
            },

            createRemoveButton: function (div) {

                var obj = this;
                var removeButton = CommonForm.createButton('Remove Selected Nodes', function () {

                    obj.modelView.sceneInteraction.removeLEDNodes();
                });

                this.setButtonStyle(removeButton);

                domConstruct.place(removeButton.domNode, div);
            },

            createSelectAllSection: function()
            {
                var div = html.createDiv();

                /*Select All Vertices*/
                var selectAllDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var obj = this;
                var selectAllNodeButton = CommonForm.createButton('Select All Vertices', function () {

                    obj.modelView.sceneInteraction.ledSet.selectAllVertexs();
                });

                this.setButtonStyle(selectAllNodeButton);
                
                domConstruct.place(selectAllNodeButton.domNode, selectAllDiv);
                domConstruct.place(selectAllDiv, div);

                /*Deselect All Vertices*/
                var deselectAllDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var deselectAllNodeButton = CommonForm.createButton('Deselect All Vertices', function () {

                    obj.modelView.sceneInteraction.ledSet.deselectAllVertexs();

                });

                this.setButtonStyle(deselectAllNodeButton);

                domConstruct.place(deselectAllNodeButton.domNode, deselectAllDiv);
                domConstruct.place(deselectAllDiv, div);


                /*Select All LEDs*/
                var selectLEDDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var selectLEDButton = CommonForm.createButton('Select All LEDs', function () {

                    obj.modelView.sceneInteraction.ledSet.selectAllLEDs();
                });

                this.setButtonStyle(selectLEDButton);

                domConstruct.place(selectLEDButton.domNode, selectLEDDiv);
                domConstruct.place(selectLEDDiv, div);

                /* Deselect All LEDs*/
                var deselectLEDDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var deselectLEDButton = CommonForm.createButton('Deselect All LEDs', function () {

                    obj.modelView.sceneInteraction.ledSet.deselectAllLEDs();

                });
                this.setButtonStyle(deselectLEDButton);

                domConstruct.place(deselectLEDButton.domNode, deselectLEDDiv);
                domConstruct.place(deselectLEDDiv, div);

                return div;
            },

            setButtonStyle: function(button)
            {
                domStyle.set(button.domNode, "width", "90%");
                domStyle.set(button.domNode.firstChild, "width", "100%");
            },

            createGroupSection: function () {

                var div = html.createDiv();
                var groupListBox = CommonForm.createListBox("width:89%;");
                this.modelView.sceneInteraction.groupSet.ledGroupListBox = groupListBox;

                var groupNameTable = html.createTable("margin-left:auto;" +
                    "margin-right:auto;" +
                    "border-spacing: 0px;"+
                    "width:95%;");
                domConstruct.place(groupNameTable, div);

                var groupNameRow = html.createRow("background-color:#141414;");
                domConstruct.place(groupNameRow, groupNameTable);

                var groupNameTitleCell = html.createCell("border-bottom-left-radius: 7px;" +
                    "border-top-left-radius: 7px;" +
                    "border-left: 2px solid" + this.tableCellBorderColor+";"+
                    "border-top: 2px solid" + this.tableCellBorderColor + ";" +
                    "border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    "color:#3d8dd5;" +
                    "width:40%;"+
                    "text-align: center");

                domConstruct.place(groupNameTitleCell, groupNameRow);
                groupNameTitleCell.innerHTML = "Group Name";

                var groupNameValueCell = html.createCell("text-align:center;" +
                    "border-top: 2px solid #2d2d2d;" +
                    "border-right: 2px solid #2d2d2d;" +                  
                    "border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    "border-left-radius: 7px;" +
                    "border-right-radius: 7px;" +
                    "width: 60%");

                domConstruct.place(groupNameValueCell, groupNameRow);

                var groupNameTextBox = CommonForm.createTextBox("", "Group Name", "width:100%;");
                domConstruct.place(groupNameTextBox.domNode, groupNameValueCell);

                this.groupListBox = groupListBox;
                
                var obj = this;

                var addButton = CommonForm.createButton('Add Group', function () {
                    var group = obj.modelView.sceneInteraction.groupSet.createGroupFromSelected(groupNameTextBox.get('value'));
                    obj.addGroup(group);
                }, null, "color:#3d8dd5;");
                domStyle.set(addButton.domNode.firstChild, "width", "100px");
             
                var removeButton = CommonForm.createButton('Remove Group', function () {

                    obj.removeSelected();

                }, null, "color:#3d8dd5;");

                domStyle.set(removeButton.domNode.firstChild, "width", "100px");

                var listDiv = html.createDiv("text-align:center;" +
                    "margin-right:auto;" +
                    "margin-left:auto;" +
                    "background-color:#141414;"+
                    "border: 2px solid #282828;"+
                    "border-radius:7px;");

                domConstruct.place(groupListBox.domNode, listDiv);
                domConstruct.place(listDiv, div);

                var buttonDiv = html.createDiv("text-align:center;");
                domConstruct.place(addButton.domNode, listDiv);
                domConstruct.place(removeButton.domNode, listDiv);                
                domConstruct.place(buttonDiv, div);

                this.setGroupNames();

                return div;
            },
            
            setGroupNames: function () {
                var obj = this;

                dojo.empty(this.groupListBox.domNode.children);
                
                Scenes.getGroupNames(function (groupNames) {
                    array.forEach(groupNames, function (groupName) {
                        obj.addGroup(groupName);
                       
                    });
                });
            },

            addGroup: function(groupName) {
                var option = html.createOption(groupName);
                var thisObj = this;
                this.groupListBox.domNode.appendChild(option);

                on(option, "click", function () {
                    var selected = [];
                    array.forEach(thisObj.groupListBox.getSelected(), function (node) {
                        selected.push(node.innerHTML);
                    });
                    thisObj.modelView.sceneInteraction.groupSet.selectGroups(selected);
                });
            },
            
            removeSelected: function () {
                var thisObj = this;
                array.forEach(this.groupListBox.getSelected(), function (node) {
                    thisObj.modelView.sceneInteraction.groupSet.removeGroup(node.innerHTML);
                    thisObj.groupListBox.domNode.removeChild(node);
                });
                
            }
        });

    });
