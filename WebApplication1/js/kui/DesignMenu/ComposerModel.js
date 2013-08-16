

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/DesignMenu/Timeline"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList, array, Timeline) {
        "use strict";
        return declare("kui.DesignMenu.ComposerModel", null, {

            /*
             *   
             */

            constructor: function (sceneInteraction) {

                this.patternModel = sceneInteraction.patternModel;
                this.patternListBox = null;
                this.patternList = new ArrayList(); 
                this.timeline = new Timeline();

            },

            addPatternFromOption: function (options) {

                var xCount = 0;
                var width = 20;
                var height = 200;
                var y = 0;
                var color = "blue";

                var thisObj = this;
                array.forEach(options, function (option) {
                                        
                    thisObj.patternModel.patternList.forEach(function (pattern) {
                        if (pattern.name === option.innerHTML) {                            
                            
                            thisObj.patternList.add(pattern); 
                            thisObj.timeline.addBar(xCount, y, width, height, color);
                            xCount += 22;
                            y += 22; 
                        }
                    }); 
                });
            }, 

            removePattern: function (patterns) {

                array.forEach(patterns, function(pattern) {

                });

            },

            updatePatternListBox: function () {

                this.patternListBox.destroyDescendants();
                 
                var thisObj = this;
                this.patternModel.patternList.forEach(function (pattern) {

                    var option = html.createOption(pattern.name);
                    dojo.connect(option, "onclick", function () {

                    });

                    thisObj.patternListBox.domNode.appendChild(option);
                });
            }
            
        });

    });
