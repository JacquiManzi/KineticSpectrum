define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
    "kui/DesignMenu/EffectMenu/TimeItem",
    "kui/DesignMenu/EffectMenu/IntItem",
    "kui/DesignMenu/EffectMenu/OrderingItem",
    "kui/DesignMenu/EffectMenu/SelectItem",
    "kui/DesignMenu/EffectMenu/ColorEffectItem"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm, array, TimeItem, IntItem,
    OrderingItem, SelectItem, ColorEffectItem) {
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
                this.handles.push(dojo.connect(item, "onUpdate", dojo.hitch(this, this._updateMapping)));
            }

            return item;
        },
        
        onUpdate: function(effectProperties) {
            
        },

        _updateMapping : function(propMap, key, value) {
            this.propertyMap[key] = value;
            this.onUpdate(this.propertyMap);
        },
        
        selectItem: function(effectDef) {
            var type = effectDef.propertyType;
            var obj = { key: effectDef.name, value: effectDef.defaultValue };
            if (type === "Time" || type ==="Float") {
                return new TimeItem(obj);
            }
            else if (type === "Int") {
                return new IntItem(obj);
            }
            else if (type === "Ordering") {
                return new OrderingItem(obj);
            }
            else if (type === "RepeatMethod") {
                var rItem = new SelectItem(obj);
                Effects.getRepeatMethods(dojo.hitch(rItem, rItem.setItems));
                return rItem;
            }
            else if (type === "Easing") {
                var eItem = new SelectItem(obj);
                Effects.getEasings(dojo.hitch(eItem, eItem.setItems));
                return eItem;
            }
            else if (type === "ColorEffect") {
                return new ColorEffectItem(obj);
            }
            else {
                this.propertyMap[obj.key] = obj.value;
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