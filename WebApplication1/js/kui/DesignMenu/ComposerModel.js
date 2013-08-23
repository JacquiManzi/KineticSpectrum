
/*
*   @author: Jacqui Manzi
*   August 22th, 2013
*
*/
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
    "kui/DesignMenu/Timeline",
     "kui/ajax/SimState"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList, array, Timeline, SimState) {
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

            _getColorFromPattern: function(patternObj){

                var hexColor = "";
                var intColor = 0;

                if (patternObj.pattern.effectName === "Pulse") {

                    intColor = patternObj.pattern.effectProperties.background.colors[0];
                    hexColor = "#" + intColor.toString(16);
                }       
                 
                else if (patternObj.pattern.effectName === "Fixed") {

                    intColor = patternObj.pattern.effectProperties["color Effect"].colors[0];   
                    hexColor = "#" + intColor.toString(16);
                }
                else if (patternObj.pattern.effectName === "Sweep") {

                    intColor = patternObj.pattern.effectProperties["sweep Start"].colors[0];
                    hexColor = "#" + intColor.toString(16);
                }
                else { 

                }

                return hexColor;
            },     

            addPatternBars: function () {

                this.yCount = 30;
                this.xCount = 44;
                this.barData = [];
                this.timeline.canvasHeight = this.timeline.defaultCanvasHeight;
                this.timeline.canvasWidth = this.timeline.defaultCanvasWidth;

                this.timeline.svg.attr('width', this.timeline.canvasWidth)
                .attr('height', this.timeline.canvasHeight);
                 
                var width = 22;
                var height = 0;
                var color = "blue"; 
                var time = 0;   


                var thisObj = this;
                var maxY = 30;
                this.patternList.forEach(function (patternObj) {

                    if (patternObj.getHeight) {
                        maxY = Math.max(maxY, patternObj.yCount + patternObj.getHeight());
                    }

                });
                    this.patternList.forEach(function (patternObj) {

                        var getHeight = function(){
                            return this.pattern.effectProperties.duration * this.pattern.effectProperties["repeat Count"] * 5;
                        }
                        var getColor = function () {
                            return thisObj._getColorFromPattern(this);
                        }
                        var getText = function () {
                            return this.pattern.name;
                        }
                        var updateStartTime = function (yCount) {
                            this.startTime = (yCount - 30)/5;
                            this.endTime = this.startTime + (this.pattern.effectProperties.duration * this.pattern.effectProperties["repeat Count"]);
                        }
                        patternObj.getHeight = getHeight;
                        patternObj.getColor = getColor;
                        patternObj.getText = getText;
                        patternObj.updateStartTime = updateStartTime;

                        if (!patternObj.xCount) {
                            patternObj.xCount = thisObj.xCount;
                            patternObj.yCount = maxY;

                            maxY += patternObj.getHeight();
                        }

                        patternObj.updateStartTime(thisObj.yCount);

                        thisObj.barData.push(patternObj);

                        thisObj.xCount += 22;
                        thisObj.yCount = thisObj.yCount + patternObj.getHeight();
                      
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

            applyAllPatterns: function () {

                this.patternList.forEach(function (patternObj) {

                    SimState.addStart(patternObj.pattern.name, patternObj.countID, patternObj.startTime);

                });

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

                
            
       

