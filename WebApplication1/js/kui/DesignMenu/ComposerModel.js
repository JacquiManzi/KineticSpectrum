﻿

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
                this.xCount = 40;
                this.yCount = 30;
                this.countID = 0;

                this.timelineHeight = 200;

            },

            _getSelectedOptions: function () {

                return this.patternListBox.getSelected();
            },

            addPattern: function (options) {

                var thisObj = this;

                this.patternModel.patternList.forEach(function (pattern) {
                    array.forEach(options, function (option) {
                        
                        if (pattern.name === option.innerHTML) {
                            var patternObj = {
                                "pattern": pattern,
                                "countID": thisObj.countID
                            };
                            thisObj.countID++;
                            thisObj.patternList.add(patternObj);
                        }
                    });   
                });

                this.addPatternBars();
            },

            addPatternBars: function () {

                this.yCount = 30;
                this.xCount = 35;
                this.barData = [{}];
                this.timeline.canvasHeight = this.timeline.defaultCanvasHeight;
                this.timeline.canvasWidth = this.timeline.defaultCanvasWidth;

                this.timeline.svg.attr('width', this.timeline.canvasWidth)
                .attr('height', this.timeline.canvasHeight);
                 
                var width = 20;
                var height = 0;
                var color = "blue"; 
                var time = 0;   

                var thisObj = this;

                    this.patternList.forEach(function (patternObj) {

                        height = (patternObj.pattern.effectProperties.duration * patternObj.pattern.effectProperties["repeat Count"]) * 5;
                        patternObj.startTime = thisObj.yCount;
                        patternObj.endTime = patternObj.startTime + height; 

                        thisObj.barData.push({
                            "rx": thisObj.xCount,
                            "ry": thisObj.yCount,
                            "height": height,
                            "width": width,
                            "color": color,
                            "pattern": patternObj.pattern,
                            "selected": false,
                            "countID": patternObj.countID
                        });

                        thisObj.xCount += 22;
                        thisObj.yCount = thisObj.yCount + height;
                      
                        if (thisObj.yCount > thisObj.timeline.svg.attr()[0][0].height.baseVal.value) {
                            thisObj.timeline.canvasHeight = thisObj.yCount;
                        }

                        if (thisObj.xCount > thisObj.timeline.svg.attr()[0][0].width.baseVal.value) {
                            thisObj.timeline.canvasWidth = thisObj.xCount;
                        }
                    });

                this.timeline.addBars(this.barData);
            },

            removePatternFromOption: function () {

                var thisObj = this;

                var removedOptions = this._getSelectedOptions();

                for (var i = 0; i < removedOptions.length; i++) {
                    for (var j = 0; j < this.patternList.count; j++) {

                        if (this.patternList.item(j).pattern.name === removedOptions[i].innerHTML) {
                            this.patternList.removeAt(j);
                        }
                    }
                }

                this.addPatternBars();
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
            },

            removeSelectedPattern: function () {
                var selectedBars = this.timeline.getSelectedBars();
                var remove = new ArrayList();

                var thisObj = this;
                this.patternList.forEach(function (patternObj) {

                    for (var i = 0; i < selectedBars.length; i++) {

                        if (selectedBars[i].getAttribute('countID') == patternObj.countID) {
                            remove.add(patternObj);
                        }
                    }
                });

                remove.forEach(function (patternObj) {
                    thisObj.patternList.remove(patternObj); 
                }); 
                 
                this.addPatternBars();
            }
        });
    });

                
            
       

