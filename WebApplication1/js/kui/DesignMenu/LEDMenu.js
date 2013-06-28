﻿

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/util/CommonFormItems"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm) {
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


            },

            createLEDMenu: function (container) {
                var contentPane = new ContentPane(
                  {
                      title: "LED Menu",
                      style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px,"+
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px,"+
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px,"+
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px,"+
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px),"+
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);"+
                "background-color: #131313;"+
                "background-size: 20px 20px;width:100%;"

                  });

                container.addChild(contentPane);

                /*Node Creation Section*/
                this.createNodeSection(contentPane);
                this.createSelectAllSection(contentPane.domNode);

                /*Node Removal Section*/
                this.createRemoveNodeSection(contentPane);

                /*Group Vertex Section*/
                this.createGroupVertexSection(contentPane.domNode);

                
    

            },

            createNodeSection: function (contentPane) {

                var nodeDiv = html.createDiv("text-align:center; color:#3d8dd5;");
                var nodeCreationDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "border-radisu:7px;"+
                    "background-color:#141414;" +
                    "border-radisu:7px;" +
                    "border: 2px solid #282828;");

                nodeCreationDiv.innerHTML = "LED Node Creation";
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


                domConstruct.place(nodeCreationDiv, nodeDiv);
                this.createNodeAmountBox(table);

                var ledModeDiv = html.createDiv("text-align:center;padding-top:20px;");
                domConstruct.place(ledModeDiv, nodeDiv);
                var obj = this;
                var ledModeButton = CommonForm.createButton("Add Single LED OFF", function () {

                    obj.modelView.sceneInteraction.setAddMode(this);

                }, null, "color:#3d8dd5;");

                this.setButtonStyle(ledModeButton);
                domConstruct.place(ledModeButton.domNode, ledModeDiv);

                domConstruct.place(table, tableDiv);
                domConstruct.place(tableDiv, nodeDiv); 
                domConstruct.place(nodeDiv, contentPane.domNode);


            },

            createRemoveNodeSection: function (contentPane) {
                var innerDiv = html.createDiv("text-align:center;" +
                   "color:#3d8dd5;"+
                   "padding-top: 10px;"+
                   "padding-bottom: 10px;");

                this.createRemoveButton(innerDiv);

                domConstruct.place(innerDiv, contentPane.domNode);

            },

            createNodeAmountBox: function (table) {
              
               
                var titleCell = html.createCell("border-bottom-left-radius: 7px;" +
                    "border-top-left-radius: 7px;" +
                    "border-left: 2px solid" + this.tableCellBorderColor+";"+
                    "border-top: 2px solid" + this.tableCellBorderColor + ";" +
                    "border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    "color:#3d8dd5;" +
                    "text-align: right;"+
                    "width:30%;");

                var valueRow = html.createRow("background-color:#141414;");

                var valueCell = html.createCell("text-align:center;" +
                    "border-top: 2px solid #2d2d2d;" +
                    "border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    "width:40%;");

                var checkButtonCell = html.createCell("border-top-right-radius: 7px;" +
                    "border-bottom-right-radius: 7px;"+
                    "border-top-right-radius: 7px;" +
                    "border-right: 2px solid" + this.tableCellBorderColor + ";" +
                    "border-top: 2px solid" + this.tableCellBorderColor + ";" +
                    "border-bottom: 2px solid" + this.tableCellBorderColor + ";" +
                    "text-align:left;"+
                    "width:30%;");

                var nodeNumberTextBox = CommonForm.createTableNumberTextBox("width:90%;");

                var obj = this;
                var checkButton = CommonForm.createButton('Add', function () {

                   var lineSegments = obj.modelView.sceneInteraction.findConnectingLines(nodeNumberTextBox.get('value'));
                   obj.modelView.sceneInteraction.drawNodes(lineSegments);

                });

                titleCell.innerHTML = "# Nodes";

                domConstruct.place(titleCell, valueRow);
                domConstruct.place(valueRow, table);
                domConstruct.place(valueCell, valueRow);
                domConstruct.place(checkButton.domNode, checkButtonCell);
                domConstruct.place(checkButtonCell, valueRow);
                domConstruct.place(nodeNumberTextBox.domNode, valueCell);

            },

            createRemoveButton: function (div) {

                var obj = this;
                var removeButton = CommonForm.createButton('Remove Selected Nodes', function () {

                    obj.modelView.sceneInteraction.removeNodes();
                });

                //dojo.addClass(removeButton, 'vertexImage');

                
                this.setButtonStyle(removeButton);

                domConstruct.place(removeButton.domNode, div);

            },

            createSelectAllSection: function(div)
            {
                /*Select All Vertices*/
                var selectAllDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var obj = this;
                var selectAllNodeButton = CommonForm.createButton('Select All Vertices', function () {

                    obj.modelView.sceneInteraction.selectAllVertexs();
                });

                this.setButtonStyle(selectAllNodeButton);
                
                domConstruct.place(selectAllNodeButton.domNode, selectAllDiv);
                domConstruct.place(selectAllDiv, div);

                /*Deselect All Vertices*/
                var deselectAllDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var deselectAllNodeButton = CommonForm.createButton('Deselect All Vertices', function () {

                    obj.modelView.sceneInteraction.deselectAllVertexs();

                });

                this.setButtonStyle(deselectAllNodeButton);

                domConstruct.place(deselectAllNodeButton.domNode, deselectAllDiv);
                domConstruct.place(deselectAllDiv, div);


                /*Select All LEDs*/
                var selectLEDDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var selectLEDButton = CommonForm.createButton('Select All LEDs', function () {

                    obj.modelView.sceneInteraction.selectAllLEDs();
                });

                this.setButtonStyle(selectLEDButton);

                domConstruct.place(selectLEDButton.domNode, selectLEDDiv);
                domConstruct.place(selectLEDDiv, div);

                /* Deselect All LEDs*/
                var deselectLEDDiv = html.createDiv("text-align:center;color:#3d8dd5;width:100%;");
                var deselectLEDButton = CommonForm.createButton('Deselect All LEDs', function () {

                    obj.modelView.sceneInteraction.deselectAllLEDs();

                });
                this.setButtonStyle(deselectLEDButton);

                domConstruct.place(deselectLEDButton.domNode, deselectLEDDiv);
                domConstruct.place(deselectLEDDiv, div);



            },

            setButtonStyle: function(button)
            {
                /*domStyle.set(button.domNode.firstChild, "border", "1px solid #4c4c4c");
                domStyle.set(button.domNode.firstChild, "background-image", "-ms-linear-gradient(bottom, rgb(33,33,33) 21%, rgb(46,45,46) 57%)");
                domStyle.set(button.domNode.firstChild, "background-image", "linear-gradient(bottom, rgb(33,33,33) 21%, rgb(46,45,46) 57%)");
                domStyle.set(button.domNode.firstChild, "background-image", "-o-linear-gradient(bottom, rgb(33,33,33) 21%, rgb(46,45,46) 57%)");
                domStyle.set(button.domNode.firstChild, "background-image", "-moz-linear-gradient(bottom, rgb(33,33,33) 21%, rgb(46,45,46) 57%)");
                domStyle.set(button.domNode.firstChild, "background-image", "-webkit-linear-gradient(bottom, rgb(33,33,33) 21%, rgb(46,45,46) 57%)");*/
                domStyle.set(button.domNode, "width", "90%");
                domStyle.set(button.domNode.firstChild, "width", "100%");

            },

            createGroupVertexSection: function (div) {

                var groupTitleDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#141414;" +
                    "border-radisu:7px;"+
                    "border: 2px solid #282828;");

                groupTitleDiv.innerHTML = "Node Grouping";
                domConstruct.place(groupTitleDiv, div); 

                var groupListBox = CommonForm.createListBox("width:89%;");

                var obj = this;
                var addVertexButton = CommonForm.createButton('Add Group', function () {

                    obj.modelView.sceneInteraction.addSelectedGroup(groupListBox);

                    
                }, null, "color:#3d8dd5;");
                domStyle.set(addVertexButton.domNode.firstChild, "width", "100px");

                //this.setButtonStyle(addVertexButton);
             
                var removeVertexButton = CommonForm.createButton('Remove Group', function () {

                    obj.modelView.sceneInteraction.removeSelectedGroup(groupListBox);

                }, null, "color:#3d8dd5;");

                domStyle.set(removeVertexButton.domNode.firstChild, "width", "100px");

                //this.setButtonStyle(removeVertexButton);

                var listDiv = html.createDiv("text-align:center;padding-top:10px;" +
                    "margin-right:auto;" +
                    "margin-left:auto;" +
                    "background-color:#141414;"+
                    "border: 2px solid #282828;"+
                    "border-radius:7px;"+
                    "margin-top:10px;");

                domConstruct.place(groupListBox.domNode, listDiv);
                domConstruct.place(listDiv, div);

                var buttonDiv = html.createDiv("text-align:center;");
                domConstruct.place(addVertexButton.domNode, listDiv);
                domConstruct.place(removeVertexButton.domNode, listDiv);                
                domConstruct.place(buttonDiv, div);

              
            },


        });

    });