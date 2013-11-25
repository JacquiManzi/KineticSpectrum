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
    "kui/DesignMenu/FileMenu/FileItem",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class",
    "kui/util/CommonFormItems"
],
    function (declare, html, ContentPane, domStyle, domConstruct,three, on, FileUploader, FileInterface, FileItem,
        AccordianItem, domClass, CommonForm) {
        "use strict";

        return declare("kui.DesignMenu.FileMenu.FileMenu", AccordianItem, {

            constructor: function() {

                this.title = "File Menu";
            },

            createFileMenu: function (container)
            {

                this.onShow = dojo.hitch(container.simulation, container.simulation.setSceneMode);
                this._createUploadSection();

                domConstruct.place(this.domNode, container.domNode);
            },

            _createUploadButton: function () {


                var func = function (event) {
                    var file = event.target.files[0];
                    var fileReader = new FileReader();

                    var thisObj = this;
                    fileReader.onload = (function (e) {

                        thisObj.sceneModel.loadFile(e.target.result, thisObj.sceneModel.getScene());

                    });
                    var fileToLoad = fileReader.readAsText(file);
                };

                var input = html.createInput('file', 200, 'model');
                on(input, "change", dojo.hitch(this, func));
                return {
                    valueContent: input
                }
            },

            _createLightConfigUploadButton: function(){

                var fileInterface = new FileInterface();
                var fileUploader = new FileUploader({

                    label: "Choose Light Configuration",
                    multiple: false,
                    uploadOnSelect: true,
                    url: "FileUpload.aspx",
                    onComplete: dojo.hitch(this.sceneModel, this.sceneModel.loadServerLEDs)
                });

                CommonForm.setButtonStyle(fileUploader, 90);

                return {
                    valueContent: fileUploader.domNode
                }
            },

            _createUploadSection: function(){

                var uploadTableItems = [];
                var fileDivs = this.createTitlePane("Upload Files");

                uploadTableItems.push(this._createUploadButton());
                uploadTableItems.push({ contentValue: html.createDiv("height:10px;") }); 
                uploadTableItems.push(this._createLightConfigUploadButton());
                this.addTableItem(uploadTableItems, fileDivs.contentDiv);

                domConstruct.place(fileDivs.paneDiv, this.domNode);
            }

        });

    });
