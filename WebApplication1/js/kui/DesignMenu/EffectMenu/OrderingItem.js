define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "kui/DesignMenu/EffectMenu/EffectItem",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
        "dojo/_base/array",
        "dijit/MenuItem"
], function (declare, domConstruct, parser, ready, EffectItem, html, Effects, CommonForm, array, MenuItem) {
    return declare("TimeItem", [EffectItem], {

        timeBox: null,

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createRow();
            var orderingTitle = html.createCell(html.titleCellStyle);

            orderingTitle.innerHTML = this.key;

            var orderingTypeCell = html.createCell("text-align:left;width:33%%;");
            var typeBox = this.typeBox = CommonForm.createDropDown(this.value.type, "width:100%;");
            typeBox.placeAt(orderingTypeCell);

            var me = this;
            Effects.getOrderingTypes(function(types) {
                array.forEach(types, function(type) {
                    typeBox.dropDown.addChild(new MenuItem({
                        label: type,
                        onClick: dojo.hitch(me, me.typeUpdated, type)
                    }));
                });
            });


            var orderingCell = html.createCell("text-align:right;width:33%");
            this.orderingBox = CommonForm.createDropDown(this.value.ordering, "width:100%");
            this.orderingBox.placeAt(orderingCell);

            domConstruct.place(orderingTitle, this.domNode);
            domConstruct.place(orderingTypeCell, this.domNode);
            domConstruct.place(orderingCell, this.domNode);
            this.typeUpdated(this.value.type);
        },
        
        typeUpdated: function(type) {
            this.value.type = type;
            this.onUpdate(this.key, this.value);
            this.typeBox.set('label', type); 
            var me = this;

            array.forEach(this.orderingBox.dropDown.getChildren(), function(child) {
                me.orderingBox.dropDown.removeChild(child);
            });
            Effects.getOrderingForType(type, function(orderings) {
                array.forEach(orderings, function(ordering) {
                    me.orderingBox.dropDown.addChild(new MenuItem({
                        label: ordering,
                        onClick: dojo.hitch(me, me.orderingUpdated, ordering)
                    }));
                    me.orderingUpdated(orderings[0]);
                });
            });
        },
        
        orderingUpdated: function(ordering) {
            this.value.ordering = ordering;
            this.onUpdate(this.key, this.value);
            this.orderingBox.set('label', ordering);
        }

       

    });

});