/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacquimanzi@gmail.com
*
*   GroupBox.js - Widget for re-usable group xlist boxes between menus.
*/
define([
    "dojo/_base/declare",
    "dojo/dom-construct",
    "dojo/dom-class",
    "dojo/dom-style",
    "kui/util/CommonHTML",
    "dijit/_WidgetBase",
    "kui/Widgets/MultiSelect"
], function (declare, domConstruct, domClass, domStyle, html, _WidgetBase, MultiSelect) {
    return declare("kui.DesignMenu.GroupBox", [_WidgetBase], {

        _sceneModel: null,
        _multiSelect: null,

        constructor: function(params) {
            this._sceneModel = params.sceneModel;
            this._sceneModel.addGroupsUpdatedListener(dojo.hitch(this, this._updateGroups));
            this._multiSelect = new MultiSelect({
                    items: this._sceneModel.getGroups(),
                    selectedItems: this._sceneModel.getSelectedGroupNames(),
                    style: "width: 95%;"+
                            "background-color: #000000;"+
                            "margin-left: auto;"+
                            "margin-top: 10px;"+
                            "margin-bottom: 10px;"+
                            "color: #3d8dd5;"
                });
        },

        buildRendering: function () {
            this.inherited(arguments);
            this.domNode = this._multiSelect.domNode;

            domClass.add(this.domNode, "designMenu");
        },

        _updateGroups: function(groups, selectedGroups) {
            this._multiSelect.set('items', groups);
            this._multiSelect.set('selectedItems', selectedGroups);
        },

        postCreate: function() {
            dojo.connect(this._multiSelect, "onSelectionChanged", dojo.hitch(this, function(selection) {
                this._sceneModel.setSelectedGroups(selection);
            }));
        },

        setup: function() {
            this.inherited(arguments);
            this._multiSelect.setup();
        }
    });

});