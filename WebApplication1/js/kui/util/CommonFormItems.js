define(
    ["dojo/dom-construct",
     "dijit/form/NumberTextBox",
    "dijit/form/CheckBox",
    "dijit/ColorPalette",
    "dijit/form/DropDownButton",
    "dijit/MenuItem",
    "dijit/DropDownMenu",
    "dijit/form/Button",
    "kui/util/CommonHTML",
    "dijit/form/MultiSelect",
    "dijit/form/TextBox",
    "dojo/dom-style"
    ], function (domConstruct, NumberTextBox, CheckBox,
        ColorPalette, DropDownButton, MenuItem, DropDownMenu, Button, html, MultiSelect, TextBox, domStyle) {
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

        createIntNumberTextBox = function(style) {
            var numberBox = new NumberTextBox({
                style: style,
                constraints: { pattern: "######" }
            });

            return numberBox;
        },
            
        createTableNumberTextBox = function (style) {

            var numberBox = new NumberTextBox({
                style: style,
                constraints: { pattern: "######.##" }
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

         createColorPalette = function (clickFunc, style) {

             var colorPalette = new ColorPalette({

                 onChange: clickFunc,
                 style: style

             });

             return colorPalette
         },

         setButtonStyle = function(button)
             {
                 domStyle.set(button.domNode, "width", "90%");
                 domStyle.set(button.domNode.firstChild, "width", "100%");
             },


         createDropDown = function (label, style) {

             var dropDownMenu = new DropDownMenu({});

             var dropDownButton = new DropDownButton({

                 label: label,
                 style: style,
                 dropDown: dropDownMenu
             });      

             return dropDownButton;
         },

         createButton = function (label, func, div, style) {
             var button = new Button({

                 label: label,
                 onClick: func,
                 style: style
             });

             if (!!div) {
                 domConstruct.place(button.domNode, div);
             }

             return button;
         },

         createListBox = function (style) {
             var listBox = new MultiSelect({
                 style: style
             });
              
             return listBox;
         },

         createTextBox = function (value, placeHolder, style, inputFunc) {
             
            var textBox = new TextBox({
                 value: value /* no or empty value! */,
                 placeHolder: placeHolder,
                 style: style,
                 onInput: inputFunc
            });

            return textBox;
         };

        return {
            createNumberTextBox: createNumberTextBox,
            createTableNumberTextBox: createTableNumberTextBox,
            createCheckBox: createCheckBox,
            createDropDown: createDropDown,
            createColorPalette: createColorPalette,
            createButton: createButton,
            createListBox: createListBox,
            createTextBox: createTextBox,
            createIntNumberTextBox: createIntNumberTextBox,
            setButtonStyle: setButtonStyle
        };
    });
