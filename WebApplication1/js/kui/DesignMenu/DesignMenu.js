


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
    "dijit/DropDownMenu",
    "dijit/form/Button"
    
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, AccordionContainer, NumberTextBox, CheckBox,
        ColorPicker, DropDownButton, MenuItem, DropDownMenu, Button) {
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

            var cameraDiv = html.createDiv("width:170px;");
            var camTitleDiv = html.createDiv("color: white;" +
                                     "padding-top: 10px;" +
                                     "text-align: center;"+
                                     "font-size: 1em;");

            domConstruct.place(camTitleDiv, cameraDiv);
             
            camTitleDiv.innerHTML = "Camera Settings";

            var cameraList = html.createUL("list-style-type: none;text-align:right;");
            
            //fov item
            this.fovBox = this.createNumberTextBox("FOV", cameraList);

            //aspect item
            this.aspectBox = this.createNumberTextBox("Aspect", cameraList);

            //near item
            this.nearBox = this.createNumberTextBox("Near", cameraList);

            //far item
            this.farBox = this.createNumberTextBox("Far", cameraList);

            //camera x, y ,and z items
            this.camXBox = this.createNumberTextBox("X Pos", cameraList);
            this.camYBox = this.createNumberTextBox("Y Pos", cameraList);
            this.camZBox = this.createNumberTextBox("Z Pos", cameraList);

            domConstruct.place(cameraList, cameraDiv);


            /*Lighting Section*/

            var lightingList = html.createUL("list-style-type: none;" +
                                             "text-align:right;");

            var lightingDiv = html.createDiv("padding-top:20px;" +
                                             "width:180px;");

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

            //Directional color selector
            this.directionalColor = this.createColorPicker("Directional Color", lightingList);
                      
            domConstruct.place(lightingList, lightingDiv);

            //Directional Intensity Value
            this.intensity = this.createNumberTextBox("Intensity", lightingList);

            //Directional distance
            this.distance = this.createNumberTextBox("Distance", lightingList);

            //Directional x, y, and z positions
            this.directX = this.createNumberTextBox("X Pos", lightingList);
            this.directY = this.createNumberTextBox("Y Pos", lightingList);
            this.directZ = this.createNumberTextBox("Z Pos", lightingList);

            //Ambient Light

            //Ambient light check box
            this.hasAmbientLight = this.createCheckBox("Ambient Light", lightingList);

            //Ambient light color selector
            this.ambientColor = this.createColorPicker("Ambient Color", lightingList);


            //Submit Button Area
            var submitDiv = html.createDiv("text-align:center;"); 
            this.createButton("Submit", submitDiv, dojo.hitch(this, this.changeModelViewValues, this.modelView, this));

       
            domConstruct.place(cameraDiv, div);
            domConstruct.place(lightingDiv, div);
            domConstruct.place(submitDiv, div);
         
            domConstruct.place(div, menu.domNode);

        },

        createNumberTextBox: function (label, list)
        {

            var li = html.createLI("color:#3d8dd5;" +
                                   "padding-bottom:10px;");

            var numberBox = new NumberTextBox({
                style: "width:70px;",
                constraints: { pattern: "######.######" }
            });

            li.innerHTML = label +" ";

            domConstruct.place(numberBox.domNode, li);
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

        createColorPicker: function (label, list)
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

            

            var colorPicker = new ColorPicker({

            });

            var colorItem = new MenuItem({


            });

            dropDownMenu.addChild(colorItem);


            domConstruct.place(colorPicker.domNode, colorItem.domNode); 
            domConstruct.place(dropDownButton.domNode, li);
            domConstruct.place(li, list);

            return dropDownMenu;
        },

        createButton: function (label, div, func)
        {
            var button = new Button({

                label: label,
                onClick: func
            });

            domConstruct.place(button.domNode, div); 
        },

        changeModelViewValues: function(modelView, designMenu)
        {

            /*Camera Settings*/
            var fov = designMenu.fovBox.get('value');
            if (!!fov && !isNaN(fov)) {
                modelView.camera.fov = fov;
            }

            var aspect = designMenu.aspectBox.get('value');
            if (!!aspect && !isNaN(aspect)) {
                modelView.camera.aspect = aspect;
            }

            var near = designMenu.nearBox.get('value');
            if (!!near && !isNaN(near)) {
                modelView.camera.near = near;
            }

            var far = designMenu.farBox.get('value');
            if (!!far && !isNaN(far)) {
                modelView.camera.far = far;
            }

            var camXPos = designMenu.camXBox.get('value');
            if (!!camXPos && !isNaN(camXPos)) {
                modelView.camera.position.x = camXPos;
            }

            var camYPos = designMenu.camYBox.get('value');
            if (!!camYPos && !isNaN(camYPos)) {
                modelView.camera.position.y = camYPos;
            }

            var camZPos = designMenu.camZBox.get('value');
            if (!!camZPos && !isNaN(camZPos)) {
                modelView.camera.position.z = camZPos;
            }

            
            /*Lighting Settings*/

            var directionalColor = designMenu.directionalColor.get('value');
            if (!!directionalColor && !isNaN(directionalColor)) {
                modelView.directionalColor.color = directionalColor;
            }

            var directX = designMenu.directX.get('value');
            if (!!directX && !isNaN(directX)) {
                modelView.directionalLightX.color = directX;
            }

            var directY = designMenu.directY.get('value');
            if (!!directY && !isNaN(directY)) {
                modelView.directionalLightY.color = directY;
            }

            var directZ = designMenu.directZ.get('value');
            if (!!directZ && !isNaN(directZ)) {
                modelView.directionalLightZ.color = directZ;
            }


            var ambientColor = designMenu.ambientColor.get('value');
            if (!!ambientColor && !isNaN(ambientColor)) {
                modelView.ambient.color = ambientColor;
            }         

            var directionalIntensity = designMenu.intensity.get('value');
            if (!!directionalIntensity && !isNaN(directionalIntensity)) {
                modelView.directionalLightIntensity = directionalIntensity;
            }

            var directionalLightDistance = designMenu.distance.get('value');
            if (!!directionalLightDistance && !isNaN(directionalLightDistance)) {
                modelView.directionalLight.directionalLightDistance = directionalLightDistance;
            }

         

        }


    });

});

