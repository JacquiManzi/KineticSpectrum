

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
    

            },

            createNodeSection: function (contentPane) {

                var nodeDiv = html.createDiv("text-align:center; color:#3d8dd5;");
                var nodeCreationDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#383838;" +
                    "border-radisu:7px;");

                nodeCreationDiv.innerHTML = "LED Node Creation";
                var table = html.createTable("margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:10px;"+
                    "padding-bottom:10px;");


                domConstruct.place(nodeCreationDiv, nodeDiv);
                this.createNodeAmountBox(table);

                domConstruct.place(table, nodeDiv);
                domConstruct.place(nodeDiv, contentPane.domNode);


            },

            createRemoveNodeSection: function (contentPane) {

                var removeNodeDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#383838;" +
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
              
               
                var titleCell = html.createCell();
                var valueRow = html.createRow();
                var valueCell = html.createCell("text-align:left;");
                var checkButtonCell = html.createCell();               

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

            }




        });

    });
