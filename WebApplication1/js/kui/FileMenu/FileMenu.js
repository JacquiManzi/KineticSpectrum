﻿

define([   
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
     "dojo/on"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct,three, on) {
        "use strict";
        return declare("kui.FileMenu.FileMenu", null, {

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
                    style: "background-color:transparent;"

                  });

                 container.addChild(contentPane);

                 this.createUploadSection(contentPane.domNode);

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
                
                    

            }


        });

    });
