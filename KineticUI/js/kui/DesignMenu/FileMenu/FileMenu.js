/*
* @Author: Jacqui Manzi
* jacqui.manzi@gmail.com
*
* August 5th, 2013
*
* FileMenu.js - Section in DesignMenu for uploading 3D model files or light configuration files to the ModelView
*
*/

define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/on",
    "dojox/form/Uploader",
    "kui/ajax/FileInterface",
    "kui/DesignMenu/FileMenu/FileItem"
],
    function (declare, html, ContentPane, domStyle, domConstruct,three, on, FileUploader, FileInterface, FileItem) {
        "use strict";

        return declare("kui.DesignMenu.FileMenu.FileMenu", [FileItem], {

            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function() {

                this.style = "background-color:transparent;";
            },

            buildRendering: function(){
            },

            createFileMenu: function (container)
            {
                 var contentPane = new ContentPane(
                 {
                    title: "File Menu",
                    style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;" +
                "width:100%;",
                     onShow: dojo.hitch(container.simulation, container.simulation.setSceneMode)
                  });

                 container.addChild(contentPane);

                 this.createUploadSection(contentPane.domNode);
                 this.createLightUploadSection(contentPane.domNode); 

            },


            createUploadSection: function (div) {

                var func = function(event) {
                    var file = event.target.files[0];
                    var fileReader = new FileReader();

                    var thisObj = this;
                    fileReader.onload = (function(e) {

                        thisObj.sceneModel.loadFile(e.target.result, thisObj.sceneModel.getScene());

                    });
                    var fileToLoad = fileReader.readAsText(file);
                };

                var input = html.createInput('file', 200, 'model');
                domConstruct.place(input, div);
                on(input, "change", dojo.hitch(this, func));
                
            },

            createLightUploadSection: function (div) {

                var fileInterface = new FileInterface();
                var fileUploader = new FileUploader({

                    label: "Choose Light Configuration",
                    multiple: false, 
                    uploadOnSelect: true,
                    url: "FileUpload.aspx",
                    onComplete: dojo.hitch(this.sceneModel,this.sceneModel.loadServerLEDs)
                });
            
                domConstruct.place(fileUploader.domNode, div);


            }


        });

    });
