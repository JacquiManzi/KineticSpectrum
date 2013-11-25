/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacqui@revkitt.com
*
*   LEDMenu.js - Main menu for Node control. LED creation and selection items are placed here.
*/

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
    "kui/DesignMenu/GroupBox",
    "dojo/dom-class",
    "dojo/dom-style"  
],
    function (declare, html, domConstruct, CommonForm, on, array, Scenes, LightAddress, AccordianItem, GroupBox, domClass, domStyle) {

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

                var selectNodeDivs = this.createTitlePane("Select / Deselect All Nodes");
                var nodeTableItems = [];
                nodeTableItems.push(this._createSelectNodeSection());
                this.addTableItem(nodeTableItems, selectNodeDivs.contentDiv);

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
                };
            },

            _createNodeAmountSection: function(){
                var amountField = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");

                var thisObj = this;
                var checkButton = CommonForm.createButton('Add', function () {
                    var lineSegments = thisObj.sceneModel.sceneInteraction.findConnectingLines(amountField.get('value'));

                    var lightAddress = new LightAddress();
                    lightAddress.lightNo = thisObj.ledAddressText.get('value') || -1;
                    lightAddress.fixtureNo = thisObj.fixtureAddressText.get('value') || -1;
                    lightAddress.portNo = thisObj.portAddressText.get('value') || -1;

                    thisObj.sceneModel.createLEDForSegments(lineSegments, lightAddress);

                }, null, "width:90%;");

                CommonForm.setButtonStyle(checkButton, 70);

                return {
                    title: "Amount",
                    valueContent: amountField.domNode,
                    checkButton: checkButton.domNode
                };
            },

            _createLEDAddressSection: function() {

                this.ledAddressText = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                }, null, "width:90%;");

                CommonForm.setButtonStyle(verifyButton, 70);

                return {
                    title: "Address",
                    valueContent: this.ledAddressText.domNode,
                    verifyButton: verifyButton.domNode
                };
            },

            _createFixureAddressSection: function(){
                this.fixtureAddressText = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                }, null, "width:90%;");

                CommonForm.setButtonStyle(verifyButton, 70);

                return {
                    title: "Fixture",
                    valueContent: this.fixtureAddressText.domNode,
                    verifyButton: verifyButton.domNode
                };
            },

            _createPortAddressSection: function(){

                this.portAddressText = CommonForm.createTableNumberTextBox("text-align:left;width:100%;");
                var verifyButton = CommonForm.createButton('Verify', function () {

                }, null, "width:90%;");

                CommonForm.setButtonStyle(verifyButton, 70);

                return {
                    title: "Port",
                    valueContent: this.portAddressText.domNode,
                    verifyButton: verifyButton.domNode
                };

            },

            _createSelectNodeSection: function(){

                var div = html.createDiv("padding-top:10px;");

                var thisObj = this;
                var selectLEDButton = html.createDiv();
                domClass.add(selectLEDButton, "allLeds");

                on(selectLEDButton, "click", function(){

                    if (!thisObj.sceneModel.allLEDsSelected) {
                        thisObj.sceneModel.getNodeModel().selectAllLEDs();
                        domStyle.set(selectLEDButton, "background-position", "-50px 0px");

                        thisObj.sceneModel.allLEDsSelected = true;
                    }
                    else {
                        thisObj.sceneModel.getNodeModel().deselectAllLEDs();
                        domStyle.set(selectLEDButton, "background-position", "0px 0px");

                        thisObj.sceneModel.allLEDsSelected = false;
                    } 
                });

                domConstruct.place(selectLEDButton, div);

                var selectVertexButton = html.createDiv();
                domClass.add(selectVertexButton, "allVertices");

                on(selectVertexButton, "click", function () {

                    if (!thisObj.sceneModel.allVerticesSelected) {
                        thisObj.sceneModel.getNodeModel().selectAllVertexs();
                        domStyle.set(selectVertexButton, "background-position", "-50px 0px");

                        thisObj.sceneModel.allVerticesSelected = true;
                    }
                    else {
                        thisObj.sceneModel.getNodeModel().deselectAllVertexs();
                        domStyle.set(selectVertexButton, "background-position", "0px 0px");

                        thisObj.sceneModel.allVerticesSelected = false;
                    }
                });

                return {
                    selectLEDButton: selectLEDButton,
                    selectVertexButton: selectVertexButton
                };
            },

            _createSelectVertexSection: function(){

                var div = html.createDiv(); 

                var thisObj = this;
                var selectVertexButton = html.createDiv();
                domClass.add(selectVertexButton, "allVertices");

                on(selectVertexButton, "click", function () {

                    if (!thisObj.sceneModel.allVerticesSelected) {
                        thisObj.sceneModel.getNodeModel().selectAllVertexs();
                        domStyle.set(selectVertexButton, "background-position", "-50px 0px");

                        thisObj.sceneModel.allVerticesSelected = true;
                    } 
                    else {
                        thisObj.sceneModel.getNodeModel().deselectAllVertexs();
                        domStyle.set(selectVertexButton, "background-position", "0px 0px");

                        thisObj.sceneModel.allVerticesSelected = false; 
                    }
                });

                domConstruct.place(selectVertexButton, div); 

                return {
                    valueContent: div
                };
            },

            _createRemoveSection: function(){ 

                var div = html.createDiv("padding-top:10px;");

                var thisObj = this;
                var removeButton = CommonForm.createButton('Remove Selected Nodes', function () {

                    var selectedNodes = thisObj.sceneModel.getSelectedNodes();
                    thisObj.sceneModel.removeNodes(selectedNodes);
                });

                CommonForm.setButtonStyle(removeButton, 90); 

                domConstruct.place(removeButton.domNode, div);

                return {
                    valueContent: div
                };
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

                var groupListBox = new GroupBox({ sceneModel: this.sceneModel });

                return {
                    valueContent: groupListBox.domNode
                };
            },

            _createGroupNameBox: function () {

                this.groupNameTextBox = CommonForm.createTextBox("", "Group Name", "text-align:left;width:100%;");
                return {
                    title: "Group Name",
                    valueContent: this.groupNameTextBox.domNode
                };

            },

            _createAddRemoveGroupSection: function () {
                var thisObj = this;
                var addButton = CommonForm.createButton('Add Group', function () {
                    var groupName = thisObj.groupNameTextBox.get('value');
                    thisObj.sceneModel.createGroupFromSelected(groupName);
                }, null);

                CommonForm.setButtonStyle(addButton, 90);

                var removeButton = CommonForm.createButton('Remove Group', function () {
                    thisObj.sceneModel.deleteSelectedGroups();
                }, null);

                CommonForm.setButtonStyle(removeButton, 90);

                return {
                    addButton: addButton.domNode,
                    removeButton: removeButton.domNode
                };
            },
        });

    });
