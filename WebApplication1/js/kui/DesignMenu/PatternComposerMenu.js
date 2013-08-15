

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/on",
    "kui/DesignMenu/Timeline",
    "dojo/dom-geometry"
 
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, on, Timeline, domGeom) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu", null, {

            /*
             */

            constructor: function (modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;
            },

            createComposerMenu: function (container) {
                this.contentPane = new ContentPane(
                {
                    title: "Pattern Composer",
                    style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;width:100%;height:100%;",

                    onShow: dojo.hitch(this, this.createTimeline)
                });    
                    
                container.addChild(this.contentPane);
            },

            createTimeline: function () {     
                 
                var timeline = new Timeline(); 

                var timelineDiv = html.createDiv("width:100%;height:300px;");   
                domConstruct.place(timelineDiv, this.contentPane.domNode);

                timeline.createCanvas(timelineDiv);                                      
            }
                    
        });
               
    });
