﻿

define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
     "dojo/on",
"dojox/form/Uploader",
"kui/ajax/FileInterface"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct,three, on, FileUploader, FileInterface) {
        "use strict";
        return declare("kui.DesignMenu.FileMenu", null, {

            /*
             *   Left menu for Kinect 3D model design 
             *
             */

            constructor: function(modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;


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
                "background-size: 20px 20px;width:100%;"

                  });

                 container.addChild(contentPane);

                 this.createUploadSection(contentPane.domNode);
                 this.createLightUploadSection(contentPane.domNode); 

            },


            createUploadSection: function (div) {

                var func = function(event)
                {
                    var file = event.target.files[0];
                    var fileReader = new FileReader();

                    var scene = this.modelView.scene;
                    var render = this.modelView.render;
                    var modelView = this.modelView;
                    fileReader.onload = ( function (e) {
                                            
                     modelView.loadFile(e.target.result, scene, render);
                       
                    });
                    var fileToLoad = fileReader.readAsText(file);
                }

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
                    onComplete: dojo.hitch(this, function(){

                      var onSuccessFunc = dojo.hitch(this,function (lightList) {

                            
                          this.modelView.sceneInteraction.removeAllNodes();
                          this.modelView.removeAllMeshes();
                            this.modelView.sceneInteraction.createLEDNodes(lightList);

                           
                        });

                       
                        fileInterface.getLightConfigList(onSuccessFunc);
                      

                    })
               


                });
            
                domConstruct.place(fileUploader.domNode, div);


            }


        });

    });
