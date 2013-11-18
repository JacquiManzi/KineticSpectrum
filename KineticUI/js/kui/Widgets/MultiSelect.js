define([
    "dojo/_base/declare",
    "dojo/_base/array",
    "dojo/dom-construct",
    "dojo/dom-attr",
    "kui/util/CommonHTML",
    "dijit/_WidgetBase",
    "dijit/form/MultiSelect",
    "dijit/_CssStateMixin"
], function(declare, array, domConstruct, domAttr, html, _WidgetBase, BaseMultiSelect, _CssStateMixin) {
    return declare("kui.Widgets.MultiSelect", [_WidgetBase, _CssStateMixin], {
        _baseMultiSelect: null,
        items: null,
        selectedItems: null,

        constructor: function(params) {
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

                domConstruct.place(option, domNode);
                domNode.appendChild(option);
            });
            this._doSetSelectedItems(selectedItems);
        },

        _doSetSelectedItems: function(selectedItems) {
            array.forEach(this.domNode.children, function(option) {
                var item = option.innerHTML;
                if (array.indexOf(selectedItems, item) > 0) {
                    domAttr.set(option, "selected", "selected");
                } else {
                    domAttr.remove(option, "selected");
                }
            });
        },

        _doGetSelectedItems: function() {
            var selectedItems = [];
            array.forEach(this.domNode.children, function(option) {
                if (domAttr.has(option, "selected")) {
                    selectedItems.push(option.innerHTML);
                }
            });

            return selectedItems;
        },

        _arraysEqual: function(array1, array2) {
            if (array1.length === array2.length) {
                return array.every(array1, function(item) {
                    return array.indexOf(array2, item) >= 0;
                });
            }
            return false;
        },

        _onSelectionChanged: function(selectedItems) {
            if (!this._arraysEqual(selectedItems, this.selectedItems)) {
                selectedItems = selectedItems.splice(0);
                this._set('selectedItems', selectedItems);
                this.onSelectionChanged(selectedItems);
            }
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
            this._doSetItems(items, this.selectedItems);
        },

        _setSelectedItemsAttr: function(/*Array<String>*/ selectedItems) {
            selectedItems = this._asArray(selectedItems);
            this._set("selectedItems", selectedItems);
            this._doSetSelectedItems(selectedItems);
        },

        postCreate: function () {
            // every time the user clicks the button, increment the counter
            dojo.connect(this._baseMultiSelect, "onChange", dojo.hitch(this, this._onSelectionChanged));
        },
        
        setup: function() {
            this.inherited(arguments);
            this._baseMultiSelect.setup();
        }
    });
})