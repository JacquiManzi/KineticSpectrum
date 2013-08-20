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
            this._colorList = new ArrayList();

            domConstruct.place(colorTitle, this.domNode);
            domConstruct.place(this.colorTypeLI, this.colorTypeUL);
            domConstruct.place(colorCell, this.domNode);
            this._typeUpdated(this.value.name);
        },

        _createColorPalette: function(multipleColors, isFirst)
        {
            
            this.addButton.destroy();

            var colorRow = html.createRow("width:90%;");
            var dropDownCell = html.createCell("width:90%;");
           
            var colorLI = html.createLI();
            var colorBox = CommonForm.createDropDown("Select Color", "width:100%;");
            CommonForm.setButtonStyle(colorBox);     
                 
            var thisObj = this;
            var colorOnClick = function () {
                dojo.hitch(thisObj, thisObj._colorUpdated, colorPalette.get('value'), colorBox)();
            };
            var colorPalette = CommonForm.createColorPalette(colorOnClick, "width:100%");

            var colorPaletteItem = new MenuItem({});

            domConstruct.place(colorPalette.domNode, colorPaletteItem.domNode);

            colorBox.dropDown.addChild(colorPaletteItem);
             
            domConstruct.place(colorLI, this.colorTypeUL);

            this.colorDropDownList.add(colorBox);

            if (!isFirst && multipleColors) {

                colorBox.placeAt(dropDownCell);
                domConstruct.place(dropDownCell, colorRow);
                domConstruct.place(colorRow, colorLI);

                this._createRemoveButton(colorBox, colorLI, colorRow);
            }
            else {

                colorBox.placeAt(colorLI);
                CommonForm.setButtonStyle(colorBox);
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

            this.colorDropDownList.forEach(function(item) {

                item.destroy();

            });

            this.colorDropDownList.clear();
        },

        _typeUpdated: function (name) {
            this.value.name = name;
            this.typeBox.set('label', name);           
           
            this._clearColorDropDownList();
        
            if (name === "Fixed") {

                this.typeBox.isMulti = false;
                this._createColorPalette(false, true);             
            }
            else if (name === "Rainbow") {
                this.typeBox.isMulti = false;
            }
            else if (name === "ColorFade") {
               
                this.typeBox.isMulti = true;
                this._createColorPalette(true, true);
                this._createAddButton();                 
            }
            else if (name === "ChasingColors") {

                this.typeBox.isMulti = true;
                this._createColorPalette(true, true);
                this._createAddButton();
            }
            else { 
            }
        },

        _colorUpdated: function (value, colorBox) {

            colorBox.set('label', value);
            var thisObj = this;
            var value = value.substr(1);   
            value = parseInt(value, 16);


            var found = false;
            for (var i = 0; i < this._colorList.count; i++) {
                var colorItem = this._colorList.item(i);
                if (colorItem.id === colorBox.id) {
                    colorItem.value = value;
                    found = true;
                }
            }
            if (!found) {
                this._colorList.add({ 'id': colorBox.id, 'value': value });
            }
            
            if (this.typeBox.isMulti) {   

                var colorArray = [];
                thisObj._colorList.forEach(function (colorItem) {

                    colorArray.push(colorItem.value);  
                });
                this.onUpdate(this.key,
                    {
                        colors: colorArray,  
                        name: this.value.name
                    }); 
            }
            else { 
                this.onUpdate(this.key,
                    {
                        color: value,
                        name: this.value.name
                    });
            }
        },

        _createAddButton: function () {

            var thisObj = this;
            this.addButton = CommonForm.createButton("+", function () {

                thisObj._createColorPalette(true, false);
                thisObj._createAddButton();

            });

            this.colorDropDownList.add(this.addButton);

            domConstruct.place(this.addButton.domNode, this.colorTypeUL);
        },

        _createRemoveButton: function (colorDropDown, colorLI, colorRow) {

            var thisObj = this;
            var removeButton = CommonForm.createButton("-", function () {

                thisObj.colorDropDownList.remove(colorDropDown);
                colorDropDown.destroy();
                thisObj.colorTypeUL.removeChild(colorLI);
                removeButton.destroy();
            });

            this.colorDropDownList.add(removeButton); 
            var removeCell = html.createCell("width:10%;");
            domConstruct.place(removeButton.domNode, removeCell);
            domConstruct.place(removeCell, colorRow);    
        }
    });

});