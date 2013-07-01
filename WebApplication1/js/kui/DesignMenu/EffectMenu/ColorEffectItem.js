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
            var colorTitle = html.createCell(html.titleCellStyle);

            colorTitle.innerHTML = this.key;

            var colorCell = html.createCell("width:60%");
            var colorTypeUL = html.createUL("list-style-type: none;" +
                "padding: 0px;");
            domConstruct.place(colorTypeUL, colorCell);

            var colorTypeLI = html.createLI();

            this.typeBox = CommonForm.createDropDown(this.value.name, "width:100%;");
            this.typeBox.placeAt(colorTypeLI);
            CommonForm.setButtonStyle(this.typeBox);

            var me = this;
            Effects.getColorEffects(function (types) {
                array.forEach(types, function (type) {
                    me.typeBox.dropDown.addChild(new MenuItem({
                        label: type,
                        onClick: dojo.hitch(me, me._typeUpdated, type)
                    }));
                });
            });

            var colorLI = html.createLI();
            var colorBox = CommonForm.createDropDown("Select Color");

            var colorOnClick = function () {

                dojo.hitch(me, me._colorUpdated, colorPalette.get('value'), colorBox)();
            };
            var colorPalette = CommonForm.createColorPalette(colorOnClick, "width:100%");

            var colorPaletteItem = new MenuItem({
                
            });

            domConstruct.place(colorPalette.domNode, colorPaletteItem.domNode);

            colorBox.dropDown.addChild(colorPaletteItem);

            CommonForm.setButtonStyle(colorBox);
            colorBox.placeAt(colorLI);

            domConstruct.place(colorTitle, this.domNode);
            domConstruct.place(colorTypeLI, colorTypeUL);
            domConstruct.place(colorLI, colorTypeUL);
            domConstruct.place(colorCell, this.domNode);
            this._typeUpdated(this.value.name);
        },

        _typeUpdated: function (name) {
            this.value.name = name;
            this.onUpdate(this.key, this.value);
            this.typeBox.set('label', name);
           
        },

        _colorUpdated: function (value, colorBox) {

           
            colorBox.set('label', value); 

        }

    });

});