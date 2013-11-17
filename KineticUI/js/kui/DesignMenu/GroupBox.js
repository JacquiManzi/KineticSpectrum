/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacquimanzi@gmail.com
*
*     
*   
*/
define([
    "dojo/_base/declare",
    "dojo/dom-construct",
    "dojo/dom-class",
    "dojo/dom-style",
    "dijit/_WidgetBase",
    "kui/Widgets/MultiSelect"
], function (declare, domConstruct, domClass, domStyle, _WidgetBase, MultiSelect) {
    return declare("kui.DesignMenu.GroupBox", [_WidgetBase], {

        _sceneModel: null,
        _multiSelect: null,

        constructor: function(params) {
            this._sceneModel = params.sceneModel;
            this._sceneModel.addGroupsUpdatedListener(dojo.hitch(this, this._updateGroups));
            this._multiSelect = new MultiSelect({
                    items: this._sceneModel.getGroups(),
                    selectedItems: this._sceneModel.getSelectedGroupNames()
                });
        },

        buildRendering: function () {
            this.inherited(arguments);
            this.domNode = this._multiSelect.domNode;
            domStyle.set(this.domNode, "width", "95%");
            domStyle.set(this.domNode, "background-color", "black");

            //domClass.add(this.domNode, "designMenu");
        },

        _updateGroups: function(groups, selectedGroups) {
            this._multiSelect.set('items', groups);
            this._multiSelect.set('selectedItems', selectedGroups);
        },

        postCreate: function() {
            dojo.connect(this._multiSelect, "onSelectionChanged", function(selection) {
                this._sceneModel.setSelectedGroups(selection);
            });
        },

        setup: function() {
            this.inherited(arguments);
            this._multiSelect.setup();
        }
    });

});