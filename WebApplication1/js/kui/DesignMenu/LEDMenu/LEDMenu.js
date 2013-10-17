
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
    "kui/ModelView/Node/LightAddress",
    "kui/DesignMenu/AccordianItem"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm,
        TitlePane, on, array, Scenes, LightAddress, AccordianItem) {
        "use strict";
        return declare("kui.DesignMenu.LEDMenu.LEDMenu", AccordianItem, {
                   
            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function () {

               // this.style = "background-color:#141414;";           
                this.title = "LED Menu";

                /*CSS Stylings*/
                /*
                this.tableCellBorderColor = "#333333";

                this.mainBackgroundColor = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                         "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                         "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                         "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                         "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                         "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
               "background-color: #131313;" +
               "background-size: 20px 20px;";*/


            },

            createLEDMenu: function (container) {
              /*  var contentPane = new ContentPane(
                  {
                      title: "LED Menu",
                      style: this.mainBackgroundColor,
                      onShow: dojo.hitch(container.simulation, container.simulation.setSceneMode)

                  });*/

                //var spacerDivStyle = "width:100%;height:7px;";

                //container.addChild(contentPane);

               // var titlePaneDiv = html.createDiv("width:100%;height:100%;");
                //domConstruct.place(titlePaneDiv, contentPane.domNode);

                //var nodeDiv = this.createNodeSection();
                              
                /*Node Creation Section*/
               /* var nodePane = new TitlePane({

                    title: "Create / Select Nodes",
                    content: nodeDiv

                });

                nodeDiv.parentNode.setAttribute('style', 'background-color:#141414;');
                domConstruct.place(nodePane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);

                          
                /*Node Removal Section*/
                /*var removalDiv = this.createRemoveNodeSection();

                var removePane = new TitlePane({

                    title: "Remove Nodes",
                    content: removalDiv

                });

                removalDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(removePane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);

                /*Group Section*/            
               /* var groupDiv = this.createGroupSection();

                var groupPane = new TitlePane({

                    title: "Group Nodes",
                    content: groupDiv

                });

                groupDiv.parentNode.setAttribute('style', this.mainBackgroundColor);
                domConstruct.place(groupPane.domNode, titlePaneDiv);
                domConstruct.place(html.createDiv(spacerDivStyle), titlePaneDiv);*/

                //contentPane.startup();

                this.onShow = function () {

                    //this.patternModel.updateGroupDropDown();
                   // container.simulation.setPatternMode();
                };

                domConstruct.place(this.domNode, container.domNode);

                this._createNodes();
                this._createSelectNodes();

            },

            _createNodes: function(){

                var nodeDivs = this.createTitlePane("Create Nodes");
                this.addDivItem(this._createAddLEDModeDiv(), nodeDivs.contentDiv);

                var amountTableItems = [];
                amountTableItems.push(this._createNodeAmountSection());
                amountTableItems.push(this._createLEDAddressSection());
                amountTableItems.push(this._createPortAddressSection());
                amountTableItems.push(this._createFixureAddressSection());
                this.addTableItem(amountTableItems, nodeDivs.contentDiv);

                domConstruct.place(nodeDivs.paneDiv, this.domNode); 
            },

            _createSelectNodes: function(){

                var selectNodeDivs = this.createTitlePane("Select / Deselect Nodes");
                this.addDivItem(this._createSelectLEDsSection(), selectNodeDivs.contentDiv);
                this.addDivItem(this._createDeselectLEDsSection(), selectNodeDivs.contentDiv);
                this.addDivItem(this._createtSelectVertexSection(), selectNodeDivs.contentDiv);
                this.addDivItem(this._createDeselectVertexSection(), selectNodeDivs.contentDiv);
                this.addDivItem(this._createRemoveSection(), selectNodeDivs.contentDiv);

                domConstruct.place(selectNodeDivs.paneDiv, this.domNode);                
            },

            _createAddLEDModeDiv: function(){

                var nodeDiv = html.createDiv();

                var thisObj = this;
                var addNodeButton = CommonForm.createButton("Add Single LED OFF",function () {

                    thisObj.sceneModel.sceneInteraction.setAddMode(this);
                }, null);

                domConstruct.place(addNodeButton.domNode, nodeDiv);
                CommonForm.setButtonStyle(addNodeButton);

                return {
                    valueContent: nodeDiv
                }
            },

            _createNodeAmountSection: function(){

                var amountField = CommonForm.createTableNumberTextBox("", "0", "text-align:left;width:100%;");

                var thisObj = this;
                var checkButton = CommonForm.createButton('Add', function () {

                    var lineSegments = thisObj.sceneModel.sceneInteraction.findConnectingLines(nodeNumberTextBox.get('value'));

                    var lightAddress = new LightAddress();
                    lightAddress.lightNo = ledAddressText.get('value');
                    lightAddress.fixtureNo = fixtureText.get('value');
                    lightAddress.portNo = portText.get('value');

                    thisObj.sceneModel.sceneInteraction.drawNodes(lineSegments, lightAddress);

                });

                return {
                    title: "LED Amount",
                    valueContent: amountField.domNode,
                    checkButton: checkButton.domNode
                }
                 
            },

            _createLEDAddressSection: function(){

                var ledAddressText = CommonForm.createTableNumberTextBox("", "0", "text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                });

                return {
                    title: "Address",
                    valueContent: ledAddressText.domNode,
                    verifyButton: verifyButton.domNode
                }
            },

            _createFixureAddressSection: function(){

                var fixtureAddressText = CommonForm.createTableNumberTextBox("", "0", "text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                });

                return {
                    title: "Fixture",
                    valueContent: fixtureAddressText.domNode,
                    verifyButton: verifyButton.domNode
                }

            },

            _createPortAddressSection: function(){

                var portAddressText = CommonForm.createTableNumberTextBox("", "0", "text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                });

                return {
                    title: "Port",
                    valueContent: portAddressText.domNode,
                    verifyButton: verifyButton.domNode
                }

            },

            _createSelectLEDsSection: function(){

                var div = html.createDiv("padding-top:10px;");

                var thisObj = this;
                var selectLEDButton = CommonForm.createButton('Select All LEDs', function () {

                    thisObj.sceneModel.sceneInteraction.ledSet.selectAllLEDs();
                });

                domConstruct.place(selectLEDButton.domNode, div);
                CommonForm.setButtonStyle(selectLEDButton);

                return {
                    valueContent: div
                }
            },

            _createDeselectLEDsSection: function(){

                var div = html.createDiv();

                var thisObj = this;
                var deselectLEDButton = CommonForm.createButton('Deselect All LEDs', function () {

                    thisObj.sceneModel.sceneInteraction.ledSet.deselectAllLEDs();
                });

                domConstruct.place(deselectLEDButton.domNode, div);
                CommonForm.setButtonStyle(deselectLEDButton);

                return {
                    valueContent: div
                }
            },

            _createtSelectVertexSection: function(){

                var div = html.createDiv();

                var thisObj = this;
                var selectVertexButton = CommonForm.createButton('Select All Vertices', function () {

                  //  thisObj.sceneModel.sceneInteraction.ledSet.selectAllVertexs();
                });

                domConstruct.place(selectVertexButton.domNode, div);
                CommonForm.setButtonStyle(selectVertexButton);

                return {
                    valueContent: div
                }
            },

            _createDeselectVertexSection: function(){

                var div = html.createDiv();

                var thisObj = this;
                var deselectVertexButton = CommonForm.createButton('Deselect All Vertices', function () {

                    //  thisObj.sceneModel.sceneInteraction.ledSet.selectAllVertexs();
                });

                domConstruct.place(deselectVertexButton.domNode, div);
                CommonForm.setButtonStyle(deselectVertexButton);

                return {
                    valueContent: div
                }
            },

            _createRemoveSection: function(){

                var div = html.createDiv("padding-top:10px;");

                var thisObj = this;
                var removeButton = CommonForm.createButton('Remove Selected Nodes', function () {

                    thisObj.sceneModel.sceneInteraction.removeLEDNodes();
                });

                domConstruct.place(removeButton.domNode, div);

                return {
                    valueContent: div
                }

            },

            _createGroupSection: function(){



            },

            _createGroupListBoxSection: function(){

                var div = html.createDiv();
                var groupListBox = CommonForm.createListBox("width:89%;");
                //this.sceneModel.sceneInteraction.groupModel.ledGroupListBox = groupListBox;

                domConstruct.place(groupListBox.domNode, div);

                return {
                    valueContent: div
                }
            },


            createGroupSection: function () {

                var div = html.createDiv();
                var groupListBox = CommonForm.createListBox("width:89%;");
                this.sceneModel.sceneInteraction.groupModel.ledGroupListBox = groupListBox;

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
                    var group = obj.sceneModel.sceneInteraction.groupModel.createGroupFromSelected(groupNameTextBox.get('value'));
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
                    thisObj.sceneModel.sceneInteraction.groupModel.selectGroups(selected);
                });
            },
            
            removeSelected: function () {
                var thisObj = this;
                array.forEach(this.groupListBox.getSelected(), function (node) {
                    thisObj.sceneModel.sceneInteraction.groupModel.removeGroup(node.innerHTML);
                    thisObj.groupListBox.domNode.removeChild(node);
                });
                
            }
        });

    });
