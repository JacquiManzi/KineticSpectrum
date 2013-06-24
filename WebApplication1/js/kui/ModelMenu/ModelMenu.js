define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/BorderContainer",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "kui/util/CommonFormItems"
],
    function (declare, html, BorderContainer, dom, ContentPane, domStyle, domConstruct, CommonForm) {
        "use strict";
        return declare("kui.ModelMenu.ModelMenu", null, {

            /*
             *   Bottom menu for Kinect 3D model files
             *
             */

            constructor: function (modelView) {

            this.modelView = modelView;

            },

                 
            create3DMenu: function (container)
        {

            var contentPane = new ContentPane(
                {
                    title: "3D Model",
                    style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;width:100%;"

                });

            container.addChild(contentPane);

            var div = html.createDiv("text-align:center;color:#3d8dd5;");
            this.create3DCameraSection(contentPane, div);
            var hideDiv = html.createDiv("text-align:center;color:#3d8dd5;padding-bottom: 10px;");
            domConstruct.place(hideDiv, contentPane.domNode);
            this.createHideVerticesSection(hideDiv);
            var resetDiv = html.createDiv("text-align:center;color:#3d8dd5;padding-bottom: 10px;");
            domConstruct.place(resetDiv, contentPane.domNode);
            this.createResetModelSection(resetDiv);

                // this.createLightingSection(contentPane, div);

            },

            create3DCameraSection: function (menu, div) {

                var cameraDiv = html.createDiv("width:100%;");
                var camTitleDiv = html.createDiv("color: white;" +
                                         "padding-top: 10px;" +
                                         "text-align: center;" +
                                         "font-size: 1em;");

                domConstruct.place(camTitleDiv, cameraDiv);

                camTitleDiv.innerHTML = "Camera Settings";

                var cameraList = html.createUL("list-style-type: none;text-align:center;");

                //camera x, y ,and z items w/ change buttons
                var xChange = CommonForm.createButton('&#10003', null);
                var yChange = CommonForm.createButton('&#10003', null);
                var zChange = CommonForm.createButton('&#10003', null);

                /*Create x,y,z number text boxes*/
                this.camXBox = CommonForm.createNumberTextBox("X Pos", cameraList, xChange);
                this.camYBox = CommonForm.createNumberTextBox("Y Pos", cameraList, yChange);
                this.camZBox = CommonForm.createNumberTextBox("Z Pos", cameraList, zChange);

                /*Create and set the click functions of each check button*/
                xChange.set('onClick', dojo.hitch(this, this.changeXCamPos, this.modelView, this.camXBox));
                yChange.set('onClick', dojo.hitch(this, this.changeYCamPos, this.modelView, this.camYBox));
                zChange.set('onClick', dojo.hitch(this, this.changeZCamPos, this.modelView, this.camZBox));

                domConstruct.place(cameraList, cameraDiv);
                domConstruct.place(cameraDiv, div);
                domConstruct.place(div, menu.domNode);

            },

            createLightingSection: function (menu, div) {
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
                this.hasDirectioalLight = CommonForm.createCheckBox("Directonal Light", lightingList);

                //Create color selection function for directional light

                var directSelect = dojo.hitch(this, function (modelView, designView, colorVal) {

                    //Format of color value is in a hex string- need to convert to pure hex value
                    colorVal = colorVal.substr(1);
                    var colorToHex = parseInt("0x" + colorVal);

                    //var colorToHex = parseInt(colorVal);
                    designView.changeDirectionalColor(modelView, colorToHex);

                }, this.modelView, this);

                //Directional color selector
                this.directionalColor = CommonForm.createColorPalette("Directional Color", lightingList, directSelect);

                domConstruct.place(lightingList, lightingDiv);

                /*Create change button for light directional intensity*/
                var intensityClick = dojo.hitch(this, this.changeDirectIntensity, this.modelView, this);
                var intensityChange = CommonForm.createButton('&#10003', intensityClick);

                //Directional Intensity Value
                this.intensity = CommonForm.createNumberTextBox("Intensity", lightingList, intensityChange);

                /*Create change button for light directional distance*/
                var distanceClick = dojo.hitch(this, this.changeDirectDistance, this.modelView, this);
                var distanceChange = CommonForm.createButton('&#10003', distanceClick);

                //Directional distance
                this.distance = CommonForm.createNumberTextBox("Distance", lightingList, distanceChange);

                /*Create change buttons for each directional light coordinate*/

                var directXChange = CommonForm.createButton('&#10003', null);
                var directYChange = CommonForm.createButton('&#10003', null);
                var directZChange = CommonForm.createButton('&#10003', null);

                //Directional x, y, and z positions
                this.directX = CommonForm.createNumberTextBox("X Pos", lightingList, directXChange);
                this.directY = CommonForm.createNumberTextBox("Y Pos", lightingList, directYChange);
                this.directZ = CommonForm.createNumberTextBox("Z Pos", lightingList, directZChange);

                /*Create and set the click functions of each check button*/
                directXChange.set('onClick', dojo.hitch(this, this.changeDirectXpos, this.modelView, this));
                directYChange.set('onClick', dojo.hitch(this, this.changeDirectYpos, this.modelView, this));
                directZChange.set('onClick', dojo.hitch(this, this.changeDirectZpos, this.modelView, this));

                //Ambient Light 

                //Ambient light check box
                this.hasAmbientLight = CommonForm.createCheckBox("Ambient Light", lightingList);

                var ambientColorSelect = dojo.hitch(this, function (modelView, designView, colorVal) {

                    //Format of color value is in a hex string- need to convert to pure hex value
                    colorVal = colorVal.substr(1);
                    var colorToHex = parseInt("0x" + colorVal);

                    //var colorToHex = parseInt(colorVal);
                    designView.changeAmbientColor(modelView, colorToHex);

                }, this.modelView, this);

                //Create color selection function for ambient color

                //Ambient light color selector
                this.ambientColor = CommonForm.createColorPalette("Ambient Color", lightingList, ambientColorSelect);


                //Submit Button Area
                var submitDiv = html.createDiv("text-align:center;");
                CommonForm.createButton("Submit", dojo.hitch(this, this.changeModelViewValues, this.modelView, this), submitDiv);

                domConstruct.place(lightingDiv, div);
                domConstruct.place(submitDiv, div);
            },

            createHideVerticesSection: function(div)
            {
                var hideButton = CommonForm.createButton('Hide Vertices', dojo.hitch(this, function () { this.modelView.showHideVertices(hideButton); }));
                hideButton.hidden = false;

                domConstruct.place(hideButton.domNode, div);


            },

            createResetModelSection: function(div)
            {
                var resetButton = CommonForm.createButton('Reset Model', dojo.hitch(this,
                    function () { this.modelView.resetObject(); }));

                domConstruct.place(resetButton.domNode, div);

            },



            /*
            * Change an individual item on the 3D model
            */
            changeXCamPos: function (modelView, designMenuValue) {
                var value = designMenuValue.get('value');
                if (!!value && !isNaN(value)) {
                    modelView.camera.position.x = value;
                }

            },

            changeYCamPos: function (modelView, designMenuValue) {
                var value = designMenuValue.get('value');
                if (!!value && !isNaN(value)) {
                    modelView.camera.position.y = value;
                }

            },

            changeZCamPos: function (modelView, designMenuValue) {
                var value = designMenuValue.get('value');
                if (!!value && !isNaN(value)) {
                    modelView.camera.position.z = value;
                }

            },

            changeDirectXpos: function (modelView, designMenu) {
                var directX = designMenu.directX.get('value');
                if (!!directX && !isNaN(directX)) {
                    modelView.directionalLight.position.x = directX;
                }

            },

            changeDirectYpos: function (modelView, designMenu) {
                var directY = designMenu.directY.get('value');
                if (!!directY && !isNaN(directY)) {
                    modelView.directionalLight.position.y = directY;
                }

            },

            changeDirectZpos: function (modelView, designMenu) {
                var directZ = designMenu.directZ.get('value');
                if (!!directZ && !isNaN(directZ)) {
                    modelView.directionalLight.position.z = directZ;
                }

            },

            changeDirectIntensity: function (modelView, designMenu) {
                var directionalIntensity = designMenu.intensity.get('value');
                if (!!directionalIntensity && !isNaN(directionalIntensity)) {
                    modelView.directionalLight.intensity = directionalIntensity;
                }
            },

            changeDirectDistance: function (modelView, designMenu) {
                var directionalLightDistance = designMenu.distance.get('value');
                if (!!directionalLightDistance && !isNaN(directionalLightDistance)) {
                    modelView.directionalLight.distance = directionalLightDistance;
                }
            },

            changeDirectionalColor: function (modelView, newColor) {
                var directionalColor = newColor;
                if (!!directionalColor) {

                    var threeColor = new three.Color(newColor);
                    modelView.directionalLight.color = threeColor;

                }

            },

            changeAmbientColor: function (modelView, newColor) {
                var ambientColor = newColor
                if (!!ambientColor) {

                    var threeColor = new three.Color(newColor);
                    modelView.ambient.color = threeColor;
                }
            },

            changeModelViewValues: function (modelView, designMenu) {

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

