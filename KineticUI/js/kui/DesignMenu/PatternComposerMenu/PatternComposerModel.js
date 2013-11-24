
/*
*   @author: Jacqui Manzi
*   August 22th, 2013
*   jacqui@revkitt.com
*
*   PatternComposerModel.js - Model view controller for the pattern composer menu.
*/
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/DesignMenu/PatternComposerMenu/Timeline",
     "kui/ajax/SimState"
],
    function (declare, html, ContentPane, domStyle, domConstruct, three, ArrayList, array, Timeline, SimState) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu.PatternComposerModel", null, {

            constructor: function (patternModel) {

                this.patternModel = patternModel;
                this.patternListBox = null;
                this.patternList = new ArrayList();
                this.timeline = new Timeline();

                this.barData = [{}]; 
                this.xCount = 40;
                this.yCount = 30;
                this.countID = 0;

                this.timelineHeight = 200;

                //Get's passed an implicit pattern list from patternModel whenever the pattern list changes
                patternModel.addPatternUpdateListener(dojo.hitch(this, this.updatePatternListBox));
                
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
                    hexColor = intColor.toString(16);
                }       
                 
                else if (patternObj.pattern.effectName === "Fixed") {

                    intColor = patternObj.pattern.effectProperties["color Effect"].colors[0];
                    hexColor =intColor.toString(16);
                }
                else if (patternObj.pattern.effectName === "Sweep") {

                    intColor = patternObj.pattern.effectProperties["sweep Start"].colors[0];
                    hexColor = intColor.toString(16);
                }
                else { 

                }

                while (hexColor.length < 6) {
                    hexColor = "0" + hexColor;
                }
                hexColor = "#" + hexColor;

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
                        }
                        if(!patternObj.yCount)
                        {
                            patternObj.updateStartTime(maxY);
                            patternObj.yCount = maxY;

                            maxY += patternObj.getHeight();                         
                            SimState.addStart(patternObj.getText(), patternObj.countID, patternObj.startTime)
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
                    this.patternModel.sceneModel.simulation.setSimulationMode();
            },

            removePatternFromOption: function () {

                var thisObj = this;
                var removedOptions = this._getSelectedOptions();

                for (var i = 0; i < removedOptions.length; i++) {
                    for (var j = 0; j < this.patternList.count; j++) {

                        if (this.patternList.item(j).pattern.name === removedOptions[i].innerHTML) {

                            /*Remove pattern from pattern model*/
                            this.patternModel.removePattern(this.patternList.item(j));
                            /*Update list box for composer model*/
                            this.updatePatternListBox();
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
                    SimState.removeStart(patternObj.countID);
                }); 
                 
                this.addPatternBars();
            },

            updateComposerFromServer: function () {
                
                var thisObj = this;
                SimState.getPatternStarts(function (patternObjs) {

                    thisObj.patternList = patternObjs;
                    patternObjs.forEach(function (pattern) {
                        thisObj.countID = Math.max(thisObj.countID, pattern.countID + 1);
                    });
                    thisObj.addPatternBars();
                   
                });

            }
        });
    });

                
            
       

