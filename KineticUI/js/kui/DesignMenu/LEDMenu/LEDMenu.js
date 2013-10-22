
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom-construct",
    "kui/util/CommonFormItems",
    "dojo/on",
    "dojo/_base/array",
    "kui/ajax/Scenes",
    "kui/ModelView/Node/LightAddress",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class"],
    function (declare, html,  domConstruct, CommonForm,  on, array, Scenes, LightAddress, AccordianItem, domClass) {
        "use strict";
        return declare("kui.DesignMenu.LEDMenu.LEDMenu", AccordianItem, {
                   

            constructor: function () {

                this.title = "LED Menu";
            },

            createLEDMenu: function (container) {
            
                this._createNodes();
                this._createSelectNodes();
                this._createGroupBox();

                this.onShow = dojo.hitch(container.simulation, container.simulation.setSceneMode);

                domConstruct.place(this.domNode, container.domNode);
                domClass.add(this.domNode, "designMenu");
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
                CommonForm.setButtonStyle(addNodeButton, 90);

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
                CommonForm.setButtonStyle(selectLEDButton, 90);

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
                CommonForm.setButtonStyle(deselectLEDButton, 90);

                return {
                    valueContent: div
                }
            },

            _createtSelectVertexSection: function(){

                var div = html.createDiv();

                var thisObj = this;
                var selectVertexButton = CommonForm.createButton('Select All Vertices', function () {

                  thisObj.sceneModel.sceneInteraction.ledSet.selectAllVertexs();
                });

                domConstruct.place(selectVertexButton.domNode, div);
                CommonForm.setButtonStyle(selectVertexButton, 90);

                return {
                    valueContent: div
                }
            },

            _createDeselectVertexSection: function(){

                var div = html.createDiv();

                var thisObj = this;
                var deselectVertexButton = CommonForm.createButton('Deselect All Vertices', function () {

                    thisObj.sceneModel.sceneInteraction.ledSet.selectAllVertexs();
                });

                domConstruct.place(deselectVertexButton.domNode, div);
                CommonForm.setButtonStyle(deselectVertexButton, 90);

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

            _createGroupBox: function(){

                var groupDivs = this.createTitlePane("Create Groups");

                var groupNameTableItems = [];
                groupNameTableItems.push(this._createGroupNameBox());
                this.addTableItem(groupNameTableItems, groupDivs.contentDiv); 
                this.addDivItem(this._createGroupListBoxSection(), groupDivs.contentDiv);
                var addRemoveTableItems = [];
                addRemoveTableItems.push(this._createAddRemoveGroupSection());
                this.addTableItem(addRemoveTableItems, groupDivs.contentDiv);

                domConstruct.place(groupDivs.paneDiv, this.domNode);
            },

            _createGroupListBoxSection: function(){

                var div = html.createDiv();
                var groupListBox = CommonForm.createListBox("width:89%;");
                this.sceneModel.sceneInteraction.groupModel.ledGroupListBox = groupListBox;

                domConstruct.place(groupListBox.domNode, div);

                return {
                    valueContent: div
                }
            },

            _createGroupNameBox: function () {

                var groupNametextBox = CommonForm.createTextBox("", "Group Name", "text-align:left;width:100%;");
                //this.patternModel.setNameField(nameField); //refactor this out and use update handlers instea

                return {
                    title: "Group Name",
                    valueContent: groupNametextBox.domNode
                };

            },

            _createAddRemoveGroupSection: function(){


                var thisObj = this;
                var addButton = CommonForm.createButton('Add Group', function () {
                    var group = thisObj.sceneModel.sceneInteraction.groupModel.createGroupFromSelected(groupNameTextBox.get('value'));
                    thisObj._addGroup(group);
                }, null);

                CommonForm.setButtonStyle(addButton, 90);

                var removeButton = CommonForm.createButton('Remove Group', function () {

                    thisObj._removeSelected();

                }, null);

                CommonForm.setButtonStyle(removeButton, 90);

                return {
                    addButton: addButton.domNode,
                    removeButton: removeButton.domNode
                }
            },
            
            _setGroupNames: function () {

                var thisObj = this;

                dojo.empty(this.groupListBox.domNode.children);
                
                Scenes.getGroupNames(function (groupNames) {
                    array.forEach(groupNames, function (groupName) {
                        thisObj._addGroup(groupName);                       
                    });
                });
            },

            _addGroup: function(groupName) {
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
            
            _removeSelected: function () {
                var thisObj = this;
                array.forEach(this.groupListBox.getSelected(), function (node) {
                    thisObj.sceneModel.sceneInteraction.groupModel.removeGroup(node.innerHTML);
                    thisObj.groupListBox.domNode.removeChild(node);
                });
                
            }
        });

    });
