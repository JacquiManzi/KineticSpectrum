define(
    ["dojo/dom-construct",
     "dijit/form/NumberTextBox",
    "dijit/form/CheckBox",
    "dijit/ColorPalette",
    "dijit/form/DropDownButton",
    "dijit/MenuItem",
    "dijit/DropDownMenu",
    "dijit/form/Button",
    "kui/util/CommonHTML"
    ], function (domConstruct, NumberTextBox, CheckBox,
        ColorPalette, DropDownButton, MenuItem, DropDownMenu, Button, html) {
        "use strict";

        var createNumberTextBox = function (label, list, changeButton) {

            var li = html.createLI("color:#3d8dd5;" +
                                   "padding-bottom:10px;");

            var numberBox = new NumberTextBox({
                style: "width:70px;",
                constraints: { pattern: "######.######" }
            });

            li.innerHTML = label + " ";

            domConstruct.place(numberBox.domNode, li);
            domConstruct.place(changeButton.domNode, li);
            domConstruct.place(li, list);

            return numberBox;
        },

        createTableNumberTextBox = function () {
           
            var numberBox = new NumberTextBox({
                style: "width:70px;",
                constraints: { pattern: "######.######" }
            });
                
            return numberBox;
        },

         createCheckBox = function (label, list) {
             var li = html.createLI("color:#3d8dd5;" +
                                    "padding-bottom:10px;");

             var checkBox = new CheckBox({
                 style: "",
                 value: "agreed",
                 checked: false,
                 onChange: function () { }
             });

             li.innerHTML = label + " ";

             domConstruct.place(checkBox.domNode, li);
             domConstruct.place(li, list);

             return checkBox;

         },

         createColorPalette = function (label, list, clickFunc) {
             var li = html.createLI("color:#3d8dd5;" +
                                    "padding-bottom:10px;");


             var dropDownMenu = new DropDownMenu({


             });

             var dropDownButton = new DropDownButton({

                 label: label,
                 style: "color:#3d8dd5;",
                 dropDown: dropDownMenu

             });


             var colorPalette = new ColorPalette({

                 onChange: clickFunc

             });

             var colorItem = new MenuItem({


             });

             dropDownMenu.addChild(colorItem);


             domConstruct.place(colorPalette.domNode, colorItem.domNode);
             domConstruct.place(dropDownButton.domNode, li);
             domConstruct.place(li, list);

             return dropDownMenu;
         },

         createDropDown = function (label, style) {

             var dropDownMenu = new DropDownMenu({
             });

             var dropDownButton = new DropDownButton({

                 label: label,
                 style: style,
                 dropDown: dropDownMenu
             });

             return dropDownButton;


         },

         createButton = function (label, func, div) {
             var button = new Button({

                 label: label,
                 onClick: func
             });

             if (!!div) {
                 domConstruct.place(button.domNode, div);
             }

             return button;
         };


        return {
            createNumberTextBox: createNumberTextBox,
            createTableNumberTextBox: createTableNumberTextBox,
            createCheckBox: createCheckBox,
            createDropDown: createDropDown,
            createColorPalette: createColorPalette,
            createButton: createButton
        };
    });
