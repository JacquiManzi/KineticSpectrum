define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "kui/DesignMenu/EffectMenu/EffectItem",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
     "dijit/MenuItem",
     "dojox/collections/ArrayList",
     "dojo/dom-style"
], function (declare, domConstruct, parser, ready, EffectItem, html, Effects, CommonForm, array, MenuItem, ArrayList, domStyle) {
    return declare("TimeItem", [EffectItem], {

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createRow();
            var colorTitle = html.createCell(html.titleCellStyle);

            this.colorDropDownList = new ArrayList();
            this.addButton = CommonForm.createButton();

            dojo.mixin(this.value, this.value.properties);

            colorTitle.innerHTML = this.key;

            var colorCell = html.createCell("width:60%");
            this.colorTypeUL = html.createUL("list-style-type: none;" +
                "padding: 0px;");
            domConstruct.place(this.colorTypeUL, colorCell);

            this.colorTypeLI = html.createLI();

            this._createTypeDropDown();


            domConstruct.place(colorTitle, this.domNode);
            domConstruct.place(this.colorTypeLI, this.colorTypeUL);
            domConstruct.place(colorCell, this.domNode);
            this._typeUpdated(this.value.name);
        },

        _createColorPalette: function(multipleColors)
        {
            
            this.addButton.destroy();

            var colorRow = html.createRow("width:90%;");
            var dropDownCell = html.createCell();
            domConstruct.place(dropDownCell, colorRow);

            var colorLI = html.createLI();
            var colorBox = CommonForm.createDropDown("Select Color");

            var me = this;
            var colorOnClick = function () {

                dojo.hitch(me, me._colorUpdated, colorPalette.get('value'), colorBox)();
            };
            var colorPalette = CommonForm.createColorPalette(colorOnClick, "width:100%");

            var colorPaletteItem = new MenuItem({

            });

            domConstruct.place(colorPalette.domNode, colorPaletteItem.domNode);

            colorBox.dropDown.addChild(colorPaletteItem); 

            CommonForm.setButtonStyle(colorBox);
            colorBox.placeAt(dropDownCell);
            domConstruct.place(colorRow, colorLI);
            domConstruct.place(colorLI, this.colorTypeUL);

            this.colorDropDownList.add(colorBox);

            if (multipleColors) {
                this._createRemoveButton(colorBox, colorLI, colorRow);
            }

            return { "colorLI": colorLI, "dropDown": colorBox };
        },

        _createTypeDropDown: function () {

            this.typeBox = CommonForm.createDropDown(this.value.name, "width:100%;");
            this.typeBox.placeAt(this.colorTypeLI);
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
        },

        _clearColorDropDownList: function () {

            this.colorDropDownList.forEach(function (item) {

                item.destroy();

            })

            this.colorDropDownList.clear();

           

        },

        _typeUpdated: function (name) {
            this.value.name = name;
            this.typeBox.set('label', name);           
           
            this._clearColorDropDownList();
        
            if (name === "Fixed") {

                this._createColorPalette(false);
             
            }
            else if (name === "Rainbow") {

            }
            else if (name === "ColorFade") {
               
                this._createColorPalette(true);
                this._createAddButton();

            }
            else if (name === "ChasingColors") {
                this._createColorPalette(true);
                this._createAddButton();
            }
            else {
            }

           
        },

        _colorUpdated: function (value, colorBox) {

          colorBox.set('label', value); 

        },

        _createAddButton: function () {

            var me = this;
            this.addButton = CommonForm.createButton("+", function () {

                me._createColorPalette(true);
                me._createAddButton();

            });

            this.colorDropDownList.add(this.addButton);

            domConstruct.place(this.addButton.domNode, this.colorTypeUL);

        },

        _createRemoveButton: function (colorDropDown, colorLI, colorRow) {

            var me = this;
            var removeButton = CommonForm.createButton("-", function () {

                me.colorDropDownList.remove(colorDropDown);
                colorDropDown.destroy();
                me.colorTypeUL.removeChild(colorLI);
                removeButton.destroy();

            });

            this.colorDropDownList.add(removeButton); 
            var removeCell = html.createCell();
            domConstruct.place(removeButton.domNode, removeCell);
            domConstruct.place(removeCell, colorRow);
        }

    });

});