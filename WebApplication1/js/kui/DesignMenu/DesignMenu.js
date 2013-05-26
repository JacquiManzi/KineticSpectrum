


define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dijit/layout/AccordionContainer",
    "dijit/form/NumberTextBox",
    "dijit/form/CheckBox",
    "dojox/widget/ColorPicker",
    "dijit/form/DropDownButton",
    "dijit/MenuItem",
    "dijit/DropDownMenu"
    
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, AccordionContainer, NumberTextBox, CheckBox,
        ColorPicker, DropDownButton, MenuItem, DropDownMenu) {
    "use strict";
    return declare("kui.DesignMenu.DesignMenu", AccordionContainer, {

        /*
         *   Left menu for Kinect 3D model design 
         *
         */

        constructor: function(obj, obj1, modelView) {

            this.style = "background-color:transparent;";
            this.modelView = modelView;
        },
         
        createMenu: function () {

          
            this.create3DMenu(this);
            this.createFileMenu(this); 

            this.startup();

        },

        create3DMenu: function (container)
        {

            var contentPane = new ContentPane(
                {
                    title: "3D Model",
                    style: "background-color:transparent;"

                });

            container.addChild(contentPane);

            this.create3DCameraSection(contentPane); 

        },

        createFileMenu: function (container)
        {
            var contentPane = new ContentPane(
              {
                  title: "File Menu",
                  style: "background-color:transparent;"

              });

            container.addChild(contentPane);

        },

        create3DCameraSection: function (menu)
        {
            
            var div = html.createDiv();

            var cameraDiv = html.createDiv();
            var camTitleDiv = html.createDiv("color: white;" +
                                     "padding-top: 10px;" +
                                     "text-align: center;"+
                                     "font-size: 1em;");

            domConstruct.place(camTitleDiv, cameraDiv);
             
            camTitleDiv.innerHTML = "Camera Settings";

            var cameraList = html.createUL("list-style-type: none;");
            
            //fov item
            this.createNumberTextBox("FOV", cameraList);

            //aspect item
            this.createNumberTextBox("Aspect", cameraList);

            //near item
            this.createNumberTextBox("Near", cameraList);

            //camera x, y ,and z items
            this.createNumberTextBox("X Pos", cameraList);
            this.createNumberTextBox("Y Pos", cameraList);
            this.createNumberTextBox("Z Pos", cameraList);

            domConstruct.place(cameraList, cameraDiv);


            /*Lighting Section*/

            var lightingList = html.createUL("list-style-type: none;");
            var lightingDiv = html.createDiv("padding-top:20px;");
            var lightTitleDiv = html.createDiv("color: white;text-align:center;" +
                                     "padding-top: 10px;" +
                                     "text-align: center;" +
                                     "font-size: 1em;");

            domConstruct.place(lightTitleDiv, lightingDiv);

            lightTitleDiv.innerHTML = "Lighting Settings";

            //Directional light
            
            //Directional check box
            this.createCheckBox("Directonal Light", lightingList);

            //Directional color selector
            this.createColorPicker("Directional Color", lightingList);
            
          
            domConstruct.place(lightingList, lightingDiv);

            //Directional Intensity Value
            this.createNumberTextBox("Intensity", lightingList);

            //Directional distance
            this.createNumberTextBox("Distance", lightingList);

            //Directional x, y, and z positions
            this.createNumberTextBox("X Pos", lightingList);
            this.createNumberTextBox("Y Pos", lightingList);
            this.createNumberTextBox("Z Pos", lightingList);


        
            domConstruct.place(cameraDiv, div);
            domConstruct.place(lightingDiv, div);
         
            domConstruct.place(div, menu.domNode);

        },

        createNumberTextBox: function (label, list)
        {

            var li = html.createLI("color:#3d8dd5;padding-bottom:10px;");
            var numberBox = new NumberTextBox({
                style: "width:80px;",
                constraints: { pattern: "0.######" }
            });

            li.innerHTML = label +" ";

            domConstruct.place(numberBox.domNode, li);
            domConstruct.place(li, list);
        },

        createCheckBox: function (label, list)
        {
            var li = html.createLI("color:#3d8dd5;padding-bottom:10px;");
            var checkBox = new CheckBox({
                style: "",
                value: "agreed",
                checked: false,
                onChange: function () { }
            });

            li.innerHTML = label + " ";

            domConstruct.place(checkBox.domNode, li);
            domConstruct.place(li, list);

        },

        createColorPicker: function (label, list)
        {
            var li = html.createLI("color:#3d8dd5;padding-bottom:10px;");

         
            var dropDownMenu = new DropDownMenu({


            });

            var dropDownButton = new DropDownButton({ 

                label: label,
                style: "color:#3d8dd5;",
                dropDown: dropDownMenu

            });

            

            var colorPicker = new ColorPicker({

            });

            var colorItem = new MenuItem({


            });

            dropDownMenu.addChild(colorItem);


            domConstruct.place(colorPicker.domNode, colorItem.domNode); 
            domConstruct.place(dropDownButton.domNode, li);
            domConstruct.place(li, list);


        }





         


         
    });

});

