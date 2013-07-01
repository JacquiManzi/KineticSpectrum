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

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createRow();
            var orderingTitle = html.createCell(html.titleCellStyle);

            orderingTitle.innerHTML = this.key;

            var orderingCell = html.createCell("width:60%");
            var orderingTypeUL = html.createUL("list-style-type: none;"+
                "padding: 0px;");
            domConstruct.place(orderingTypeUL, orderingCell);
         
            var orderingTypeLI = html.createLI();
           
            var typeBox = this.typeBox = CommonForm.createDropDown(this.value.type, "width:100%;");
            typeBox.placeAt(orderingTypeLI);
            CommonForm.setButtonStyle(typeBox);

            typeBox.set('label', this.value.type);
            var me = this;
            Effects.getOrderingTypes(function(types) {
                array.forEach(types, function(type) {
                    typeBox.dropDown.addChild(new MenuItem({
                        label: type,
                        onClick: dojo.hitch(me, me.typeUpdated, type)
                    }));
                });
            });

            var orderingLI = html.createLI();
            this.orderingBox = CommonForm.createDropDown(this.value.ordering, "width:100%;");
            CommonForm.setButtonStyle(this.orderingBox);
            this.orderingBox.placeAt(orderingLI);
            this.orderingBox.set('label', this.value.ordering);

            domConstruct.place(orderingTitle, this.domNode);
            domConstruct.place(orderingTypeLI, orderingTypeUL);
            domConstruct.place(orderingLI, orderingTypeUL);
            domConstruct.place(orderingCell, this.domNode);
            this.setOrdering(this.value.type, false);
        },
        
        typeUpdated: function(type) {
            this.value.type = type;
            this.onUpdate(this.key, this.value);
            this.typBox.set('label', type);
            this.setOrdering(type, true);
        },

        setOrdering: function (type, update) {
            
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
                    if (update) {
                        me.orderingUpdated(orderings[0]);
                    }
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