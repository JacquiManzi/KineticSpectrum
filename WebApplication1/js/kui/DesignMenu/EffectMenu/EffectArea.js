define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
    "kui/DesignMenu/EffectMenu/TimeItem",
    "kui/DesignMenu/EffectMenu/IntItem",
    "kui/DesignMenu/EffectMenu/OrderingItem"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm, array, TimeItem, IntItem, OrderingItem) {
    return declare("EffectArea", [_WidgetBase], {
       
        items: null,
        propertyMap: null,
        handles: [],
        
        constructor: function() {
            this.items = [];
            this.propertyMap = {};
        },

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();
            
        },

        buildFromDefaults: function(effectDefinitions) {
            this.clearProps();

            if (effectDefinitions.length > 0) {
                var table = html.createTable(html.tableStyle);
                domConstruct.place(table, this.domNode);

                array.forEach(effectDefinitions, dojo.hitch(this, function(effectDef) {
                    var item = this._buildItemContainer(effectDef);
                    if (!!item) {
                        item.placeAt(table);
                    }
                }));
            }
        },
        
        _buildItemContainer: function(effectDef) {
            var item = this.selectItem(effectDef);
            if (!!item) {
                this.items.push(item);
                this.propertyMap[item.key] = item.value;
                this.handles.push(dojo.connect(item, "onUpdate", dojo.partial(this._updateMapping, this.propertyMap)));
            }

            return item;
        },
        
        _updateMapping : function(propMap, key, value) {
            propMap[key] = value;
        },
        
        selectItem: function(effectDef) {
            var type = effectDef.propertyType;
            var obj = { key: effectDef.name, value: effectDef.defaultValue };
            if (type === "Time" || type=="Float") {
                return new TimeItem(obj);
            }
            if (type == "Int") {
                return new IntItem(obj);
            }
            if (type === "Ordering") {
                return new OrderingItem(obj);
            }
            return null;
        },
        
        clearProps: function() {
            domConstruct.empty(this.domNode);
            this.items = [];
            this.propertyMap = {};
            array.forEach(this.handles, function(handle) {
                dojo.disconnect(handle);
            });

        },
        

        

        postCreate: function () {
            // every time the user clicks the button, increment the counter
            //this.connect(this.domNode, "onclick", "increment");
        },

    });

});