define([
    "dojo/_base/declare",
    "dojo/_base/array",
    "dojo/dom-construct",
    "dojo/dom-attr",
    "kui/util/CommonHTML",
    "dijit/_WidgetBase",
    "dijit/form/MultiSelect"
], function(declare, array, domConstruct, domAttr, html, _WidgetBase, BaseMultiSelect) {
    return declare("kui.Widgets.MultiSelect", [_WidgetBase], {
        _baseMultiSelect: null,
        items: null,
        selectedItems: null,

        constructor: function(params, srcNodeRef) {
            this.items = this._asArray(params.items);
            this.selectedItems = this._asArray(params.selectedItems);
        },

        _asArray: function(toArray) {
            var arr = toArray || [];
            if (!dojo.isArray(arr)) {
                arr = [arr];
            }
            return arr;
        },

        _doSetItems: function(items, selectedItems) {
            var domNode = this.domNode;
            domConstruct.empty(domNode);

            array.forEach(items, function (item) {
                var option = html.createOption(item);
                if (array.indexOf(selectedItems, item) < 0) {
                    domAttr.set(option);
                }

                domConstruct.place(option, domNode);
                domNode.append(option);
            });
        },

        _doSetSelectedItems: function(selectedItems) {
            this.domNode.children().forEach(function(option) {
                var item = option.innerHTML;
                if (array.indexOf(selectedItems, item) < 0) {
                    domAttr.set(option, "selected", "selected");
                } else {
                    domAttr.remove(option, "selected");
                }
            });
        },

        _onSelectionChanged: function() {
            this.onSelectionChanged(this.get('selectedItems'));
        },

        onSelectionChanged: function() {
            
        },


        buildRendering: function() {
            this.inherited(arguments);
            this._baseMultiSelect = new BaseMultiSelect();
            this.domNode = this._baseMultiSelect.domNode;
        },

        _setItemsAttr: function(/*Array<String>*/ items) {
            items = this._asArray(items);
            this._set("items", items);
        },

        _setSelectedItemsAttr: function(/*Array<String>*/ selectedItems) {
            selectedItems = this._asArray(selectedItems);
            this._set("selectedItems", selectedItems);
        },

        postCreate: function () {
            // every time the user clicks the button, increment the counter
            //dojo.connect(this._baseMultiSelect, "onClick", dojo.hitch(this, this._onSelectionChanged));
        },
        
        setup: function() {
            this.inherited(arguments);
            this._baseMultiSelect.setup();
        }
    });
})