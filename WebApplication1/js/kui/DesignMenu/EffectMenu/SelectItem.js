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

            var itemCell = html.createCell("text-align:left;width:100%;");
            this.itemBox = CommonForm.createDropDown(this.value, "width:100%;");
            this.itemBox.placeAt(itemCell);

            domConstruct.place(orderingTitle, this.domNode);
            domConstruct.place(itemCell, this.domNode);
        },
        
        setItems: function(items) {
            var me = this;
            array.forEach(items, function (item) {
                me.itemBox.dropDown.addChild(new MenuItem({
                    label: item,
                    onClick: dojo.hitch(me, me.typeUpdated, item)
                }));
            });
        },

        valueUpdated: function (value) {
            this.value = value;
            this.onUpdate(this.key, this.value);
            this.typeBox.set('label', type);
        },


    });

});