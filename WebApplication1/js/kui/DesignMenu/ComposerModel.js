

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

                this.barData = [{}];
                this.xCount = 0;
                this.yCount = 0;

                this.timelineHeight = 200;

            },

            addPatternFromOption: function (options) {

                var width = 20;
                var height = 0;
                var color = "blue";
                var time = 0;

                var thisObj = this;
                array.forEach(options, function (option) {
                                        
                    thisObj.patternModel.patternList.forEach(function (pattern) {
                        if (pattern.name === option.innerHTML) {                            
                          
                            if (!thisObj.patternList.contains(pattern)) {
                                thisObj.patternList.add(pattern);
                            }
                                
                            height = (pattern.effectProperties.duration * pattern.effectProperties["repeat Count"]);

                            thisObj.barData.push({   
                                "rx": thisObj.xCount,
                                "ry": thisObj.yCount,
                                "height": height,
                                "width": width,   
                                "color": color
                            });

                            thisObj.xCount += 22;    
                            thisObj.yCount = thisObj.yCount + height;
                        }
                    });           
                });
              
                this.timeline.addBars(this.barData);   

            }, 

            removePatternFromOption: function (removedOptions) {

                var thisObj = this;
                var newOptions = [];

                for (var i = 0; i < removedOptions.length; i++) {
                    for (var j = 0; j < this.patternList.count; j++) {

                        if (this.patternList.item(j).name === removedOptions[i].innerHTML) {
                            this.patternList.remove(this.patternList.item(j));
                        }                     
                     } 
                }
                     
                this.patternList.forEach(function (pattern) {
                    newOptions.push(html.createOption(pattern.name));
                });

                this.yCount = 0;
                this.xCount = 0;
                this.patternList.clear();
                this.barData = [{}];

                this.addPatternFromOption(newOptions);  
            },

            updatePatternListBox: function () {

                html.removeDomChildren(this.patternListBox);
                  
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
