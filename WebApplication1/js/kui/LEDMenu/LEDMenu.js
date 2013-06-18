

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
        return declare("kui.LEDMenu.LEDMenu", null, {

            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function (modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;


            },

            createLEDMenu: function (container) {
                var contentPane = new ContentPane(
                  {
                      title: "LED Menu",
                      style: "background-color:transparent;"

                  });

                container.addChild(contentPane);

                /*Node Creation Section*/
                this.createNodeSection(contentPane);

                /*Node Removal Section*/
                this.createRemoveNodeSection(contentPane);

                /*Group Vertex Section*/
                this.createGroupVertexSection(contentPane.domNode);
    

            },

            createNodeSection: function (contentPane) {

                var nodeDiv = html.createDiv("text-align:center; color:#3d8dd5;");
                var nodeCreationDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#2d2d2d;" +
                    "border-radisu:7px;");

                nodeCreationDiv.innerHTML = "LED Node Creation";
                var table = html.createTable("margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:10px;"+
                    "padding-bottom:10px;"+
                    "border-spacing: 0px;");

                var tableDiv = html.createDiv("width:100%;" +                    
                    "margin-left:auto;" +
                    "margin-right:auto;"+
                    "border-radius:10px;");


                domConstruct.place(nodeCreationDiv, nodeDiv);
                this.createNodeAmountBox(table);

                domConstruct.place(table, tableDiv);
                domConstruct.place(tableDiv, nodeDiv); 
                domConstruct.place(nodeDiv, contentPane.domNode);


            },

            createRemoveNodeSection: function (contentPane) {

                var removeNodeDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#2d2d2d;" +
                    "border-radisu:7px;");

                removeNodeDiv.innerHTML = "LED Node Removal";

                var innerDiv = html.createDiv("text-align:center;" +
                   "color:#3d8dd5;"+
                   "padding-top: 10px;");

                this.createRemoveButton(innerDiv);

                domConstruct.place(removeNodeDiv, contentPane.domNode);
                domConstruct.place(innerDiv, contentPane.domNode);
                

            },

            createNodeAmountBox: function (table) {
              
               
                var titleCell = html.createCell("border-bottom-left-radius: 7px;" +
                    "border-top-left-radius: 7px;" +
                    "border-left: 2px solid #424242;"+
                    "border-top: 2px solid #424242;"+
                    "border-bottom: 2px solid #424242;"+
                    "color:#efefef;");

                var valueRow = html.createRow("background-color:#333333;");

                var valueCell = html.createCell("text-align:left;" +
                    "border-top: 2px solid #424242;" +
                    "border-bottom: 2px solid #424242;");

                var checkButtonCell = html.createCell("border-top-right-radius: 7px;" +
                    "border-bottom-right-radius: 7px;"+
                    "border-top-right-radius: 7px;" +
                    "border-right: 2px solid #424242;" +
                    "border-top: 2px solid #424242;" +
                    "border-bottom: 2px solid #424242;");

                var nodeNumberTextBox = CommonForm.createTableNumberTextBox();

                var obj = this;
                var checkButton = CommonForm.createButton('Add', function () {

                   var lineSegments = obj.modelView.modelSkeleton.findConnectingLines(nodeNumberTextBox.get('value'));
                   obj.modelView.modelSkeleton.drawNodes(lineSegments);

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

                    obj.modelView.modelSkeleton.removeNodes();
                });

                domConstruct.place(removeButton.domNode, div);

            },

            createGroupVertexSection: function (div) {


                var groupListBox = CommonForm.createListBox("width:50%;");

                var obj = this;
                var addVertexButton = CommonForm.createButton('Group Selected Vertices', function () {

                    obj.modelView.modelSkeleton.addSelectedGroup(groupListBox);

                    
                });

                domConstruct.place(addVertexButton.domNode, div);

                var removeVertexButton = CommonForm.createButton('Remove Selected Vertices', function () {

                    obj.modelView.modelSkeleton.removeSelectedGroup(groupListBox);

                });

                domConstruct.place(removeVertexButton.domNode, div);

                var listDiv = html.createDiv("text-align:center;");
                domConstruct.place(groupListBox.domNode, listDiv);
                domConstruct.place(listDiv, div);

                

                

            }




        });

    });
