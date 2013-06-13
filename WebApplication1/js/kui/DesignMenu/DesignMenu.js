﻿


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
    "dijit/ColorPalette",
    "dijit/form/DropDownButton",
    "dijit/MenuItem",
    "dijit/DropDownMenu",
    "dijit/form/Button",
    "threejs/three"
    
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, AccordionContainer, NumberTextBox, CheckBox,
        ColorPalette, DropDownButton, MenuItem, DropDownMenu, Button, three) {
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

            var div = html.createDiv();
            this.create3DCameraSection(contentPane, div);
           // this.createLightingSection(contentPane, div);

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

        create3DCameraSection: function (menu, div)
        {

            var cameraDiv = html.createDiv("width:100%;");
            var camTitleDiv = html.createDiv("color: white;" +
                                     "padding-top: 10px;" +
                                     "text-align: center;"+
                                     "font-size: 1em;");

            domConstruct.place(camTitleDiv, cameraDiv);
             
            camTitleDiv.innerHTML = "Camera Settings";

            var cameraList = html.createUL("list-style-type: none;text-align:center;");
            
            //camera x, y ,and z items w/ change buttons
            var xChange = this.createButton('&#10003', null);
            var yChange = this.createButton('&#10003', null);
            var zChange = this.createButton('&#10003', null);

            /*Create x,y,z number text boxes*/
            this.camXBox = this.createNumberTextBox("X Pos", cameraList, xChange);
            this.camYBox = this.createNumberTextBox("Y Pos", cameraList, yChange);
            this.camZBox = this.createNumberTextBox("Z Pos", cameraList, zChange);

            /*Create and set the click functions of each check button*/
            xChange.set('onClick', dojo.hitch(this, this.changeXCamPos, this.modelView, this.camXBox));          
            yChange.set('onClick', dojo.hitch(this, this.changeYCamPos, this.modelView, this.camYBox));            
            zChange.set('onClick', dojo.hitch(this, this.changeZCamPos, this.modelView, this.camZBox));

            domConstruct.place(cameraList, cameraDiv);
            domConstruct.place(cameraDiv, div);           
            domConstruct.place(div, menu.domNode);

        },

        createLightingSection: function(menu, div)
        {
            /*Lighting Section*/

            var lightingList = html.createUL("list-style-type: none;" +
                                             "text-align:center;");

            var lightingDiv = html.createDiv("padding-top:20px;" +
                                             "width:100%;");

            var lightTitleDiv = html.createDiv("color: white;" +
                                               "text-align:center;" +
                                               "padding-top: 10px;" +
                                               "text-align: center;" +
                                               "font-size: 1em;");

            domConstruct.place(lightTitleDiv, lightingDiv);

            lightTitleDiv.innerHTML = "Lighting Settings";

            //Directional light

            //Directional check box
            this.hasDirectioalLight = this.createCheckBox("Directonal Light", lightingList);

            //Create color selection function for directional light
         
            var directSelect = dojo.hitch(this, function (modelView, designView, colorVal) {

                //Format of color value is in a hex string- need to convert to pure hex value
                colorVal = colorVal.substr(1);
                var colorToHex = parseInt("0x" + colorVal);

                //var colorToHex = parseInt(colorVal);
                designView.changeDirectionalColor(modelView, colorToHex);

            }, this.modelView, this);

            //Directional color selector
            this.directionalColor = this.createColorPalette("Directional Color", lightingList, directSelect);

            domConstruct.place(lightingList, lightingDiv);

            /*Create change button for light directional intensity*/
            var intensityClick = dojo.hitch(this, this.changeDirectIntensity, this.modelView, this);
            var intensityChange = this.createButton('&#10003', intensityClick);

            //Directional Intensity Value
            this.intensity = this.createNumberTextBox("Intensity", lightingList, intensityChange);

            /*Create change button for light directional distance*/
            var distanceClick = dojo.hitch(this, this.changeDirectDistance, this.modelView, this);
            var distanceChange = this.createButton('&#10003', distanceClick);

            //Directional distance
            this.distance = this.createNumberTextBox("Distance", lightingList, distanceChange);

            /*Create change buttons for each directional light coordinate*/

            var directXChange = this.createButton('&#10003', null);
            var directYChange = this.createButton('&#10003', null);
            var directZChange = this.createButton('&#10003', null);

            //Directional x, y, and z positions
            this.directX = this.createNumberTextBox("X Pos", lightingList, directXChange);
            this.directY = this.createNumberTextBox("Y Pos", lightingList, directYChange);
            this.directZ = this.createNumberTextBox("Z Pos", lightingList, directZChange);

            /*Create and set the click functions of each check button*/
            directXChange.set('onClick', dojo.hitch(this, this.changeDirectXpos, this.modelView, this));
            directYChange.set('onClick', dojo.hitch(this, this.changeDirectYpos, this.modelView, this));
            directZChange.set('onClick', dojo.hitch(this, this.changeDirectZpos, this.modelView, this));

            //Ambient Light 

            //Ambient light check box
            this.hasAmbientLight = this.createCheckBox("Ambient Light", lightingList);

            var ambientColorSelect = dojo.hitch(this, function (modelView, designView, colorVal) {

                //Format of color value is in a hex string- need to convert to pure hex value
                colorVal = colorVal.substr(1);
                var colorToHex = parseInt("0x" + colorVal); 
       
                //var colorToHex = parseInt(colorVal);
                designView.changeAmbientColor(modelView, colorToHex); 

            }, this.modelView, this);

            //Create color selection function for ambient color

            //Ambient light color selector
            this.ambientColor = this.createColorPalette("Ambient Color", lightingList, ambientColorSelect);


            //Submit Button Area
            var submitDiv = html.createDiv("text-align:center;");
            this.createButton("Submit", dojo.hitch(this, this.changeModelViewValues, this.modelView, this), submitDiv);

            domConstruct.place(lightingDiv, div);
            domConstruct.place(submitDiv, div);
        },

        createNumberTextBox: function (label, list, changeButton)
        {

            var li = html.createLI("color:#3d8dd5;" +
                                   "padding-bottom:10px;");

            var numberBox = new NumberTextBox({
                style: "width:70px;",
                constraints: { pattern: "######.######" }
            });

            li.innerHTML = label +" ";

            domConstruct.place(numberBox.domNode, li);
            domConstruct.place(changeButton.domNode, li);  
            domConstruct.place(li, list);

            return numberBox;
        },

        createCheckBox: function (label, list)
        {
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

        createColorPalette: function (label, list, clickFunc)
        {
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

        createButton: function (label, func, div)
        {
            var button = new Button({

                label: label,
                onClick: func
            });

            if (!!div) {
                domConstruct.place(button.domNode, div);
            }

            return button; 
        },

        /*
        * Change an individual item on the 3D model
        */
        changeXCamPos: function(modelView, designMenuValue)
        {
            var value = designMenuValue.get('value');
            if (!!value && !isNaN(value)) {
                modelView.camera.position.x = value;
            }

        },

        changeYCamPos: function(modelView, designMenuValue)
        {
            var value = designMenuValue.get('value');
            if (!!value && !isNaN(value)) {
                modelView.camera.position.y = value;
            }

        },

        changeZCamPos: function(modelView, designMenuValue)
        {
            var value = designMenuValue.get('value');
            if (!!value && !isNaN(value)) {
                modelView.camera.position.z = value;
            }

        },

        changeDirectXpos: function(modelView, designMenu)
        {
            var directX = designMenu.directX.get('value');
            if (!!directX && !isNaN(directX)) {
                modelView.directionalLight.position.x = directX;
            }

        },

        changeDirectYpos: function(modelView, designMenu)
        {
            var directY = designMenu.directY.get('value');
            if (!!directY && !isNaN(directY)) {
                modelView.directionalLight.position.y = directY;
            }

        },

        changeDirectZpos: function(modelView, designMenu) 
        {
            var directZ = designMenu.directZ.get('value');
            if (!!directZ && !isNaN(directZ)) {
                modelView.directionalLight.position.z = directZ;
            }

        },

        changeDirectIntensity: function(modelView, designMenu)
        {
            var directionalIntensity = designMenu.intensity.get('value');
            if (!!directionalIntensity && !isNaN(directionalIntensity)) {
                modelView.directionalLight.intensity = directionalIntensity;
            }
        },

        changeDirectDistance: function(modelView, designMenu)
        {
            var directionalLightDistance = designMenu.distance.get('value');
            if (!!directionalLightDistance && !isNaN(directionalLightDistance)) {
                modelView.directionalLight.distance = directionalLightDistance;
            }
        },

        changeDirectionalColor: function(modelView, newColor)
        {
            var directionalColor = newColor;
            if (!!directionalColor) {

                var threeColor = new three.Color(newColor);
                modelView.directionalLight.color = threeColor;

            }

        },

        changeAmbientColor: function(modelView, newColor)
        {
            var ambientColor = newColor
            if (!!ambientColor) {

                var threeColor = new three.Color(newColor);
                modelView.ambient.color = threeColor;
            }
        },

        changeModelViewValues: function(modelView, designMenu) 
        {

            /*Camera Settings*/
            //Change x Cam Pos
            this.changeXCamPos(modelView, designMenu.camXBox);

            //Change y Cam Pos
            this.changeYCamPos(modelView, designMenu.camYBox);

            //Change Z Cam Pos
            this.changeZCamPos(modelView, designMenu.camZBox);

            
            /*Lighting Settings*/
            //Change directional light x position
            this.changeDirectXpos(modelView, designMenu);

            //Change directional light y position
            this.changeDirectYpos(modelView, designMenu);

            //Change directional light z position
            this.changeDirectZpos(modelView, designMenu);

            //Change directional light intensity
            this.changeDirectIntensity(modelView, designMenu);

            //Change directional light distance
            this.changeDirectDistance(modelView, designMenu);


        }


    });

});

