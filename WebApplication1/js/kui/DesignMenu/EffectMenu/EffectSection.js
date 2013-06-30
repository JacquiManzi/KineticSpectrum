define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "kui/DesignMenu/EffectMenu/EffectArea",
    "dojo/_base/array",
    "dijit/MenuItem"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects,CommonForm, EffectArea, array, MenuItem) {
    return declare("EffectSection", [_WidgetBase], {

        effectArea: null,
      
        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();

            var table = html.createTable(html.tableStyle);
            domConstruct.place(table, this.domNode);

            var effectNameRow = this.buildEffectNameSection();
            domConstruct.place(effectNameRow, table);
            
            this.effectArea = new EffectArea();
            this.effectArea.placeAt(this.domNode);

        },

        buildEffectNameSection: function(){
            var row = html.createRow();

            var titleCell = html.createCell(this.titleCellStyle);
            titleCell.innerHTML = "Effect";
            domConstruct.place(titleCell, row);

            var valueCell = html.createCell("width:60%;");
            domConstruct.place(valueCell, row);

            var effectDropDown = CommonForm.createDropDown("Select Effect", "width:100%;");
            domConstruct.place(effectDropDown.domNode, valueCell);

            var me = this;

            Effects.getEffects(function (effects) {
                array.forEach(effects, function (effect) {
                    effectDropDown.dropDown.addChild(new MenuItem({
                        label: effect,
                        onClick: dojo.hitch(me, me.effectSelectionChanged, effect)
                    }));
                });
            });

            return row;
        },

        effectSelectionChanged : function(effectName)
        {
            Effects.getEffectDefinition(effectName, dojo.hitch(this,function (effectDefs) {
                this.effectArea.buildFromDefaults(effectDefs);
            }));
        },

        postCreate: function () {
            // every time the user clicks the button, increment the counter
            //this.connect(this.domNode, "onclick", "increment");
        },

    });

});