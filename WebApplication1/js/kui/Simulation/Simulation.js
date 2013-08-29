define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
    "kui/ajax/SimState",
    "kui/ajax/Scenes"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm, array, SimState, Scenes) {
    var modes = { scene: "Scene", pattern: "Pattern", Simulation: "Simulation" };
    return declare("Simulation", null, {

        mode: modes.scene,
        endTime: 0,
        timeIndex: 0,
        isPlaying: false,
        intervalHandle: null,
        patternStates: null,
            

        constructor: function () {
            SimState.setMode(this.mode);
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onTimeChange", dojo.hitch(this, this._onTimeChange));
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onPlayChange", dojo.hitch(this, this._onPlayChange));
        },
        
        _onSimMode: function(enabled) {
            SimState.setMode(this.mode);
            if (!enabled) {
                this.pause();
            }
        },
        
        onSimMode: function(enabled) {
            
        },
        
        _onEndTimeChange: function(endTime) {
            this.endTime = endTime;
        },
        
        onEndTimeChange: function(endTime) {
            
        },
        
        _onTimeChange: function (time) {
            if (this.mode == modes.pattern) {
                this.onStateChange(this.patternStates[time]);
            }
        },
        
        onTimeChange: function(time) {
        },
        
        onStateChange: function(stateList) {
           
        },
        
        _onPlayChange: function (isPlaying) {
            this.isPlaying = isPlaying;
            clearInterval(this.intervalHandle);
            if (isPlaying) {
                this.intervalHandle = setInterval(dojo.hitch(this, this._intervalUpdate), 1000/30);
            }
        },
        
        onPlayChange: function(isPlaying) {
        },
        
        setSceneMode: function() {
            this.mode = modes.scene;
            SimState.setMode(this.mode);
            this.onSimMode(false);
            this.pause();
        },
        
        setPatternMode: function() {
            this.mode = modes.pattern;
            SimState.setMode(this.mode);
            this.onSimMode(true);
            this.play();
        },
        
        setSimulationMode: function() {
            this.mode = modes.Simulation;
            SimState.setMode(this.mode);
            this.onSimMode(true);
            SimState.getEndTime(dojo.hitch(this, function (endTime) {
                this.onEndTimeChange(endTime);
                this.play();
            }));
        },
        
        play: function () {
            if (!this.isPlaying) {
                this.onPlayChange(true);
                SimState.setTime(this.getTime());
                SimState.play();
            }
        },
        
        pause: function () {
            if (this.isPlaying) {
                this.onPlayChange(false);
                SimState.setTime(this.getTime());
                SimState.pause();
            }
        },
        
        getTime: function() {
            return this.timeList ? this.timeList[this.timeIndex] : 0;
        },
        
        setTime: function (time) {
            if (this.timeList && time !== this.getTime()) {
                for (var i = 0; i < this.timeList.length; i++) {
                    if (this.timeList[i] <= time) {
                        this.timeIndex = i;
                        this.onTimeChange(this.timeList[i]);
                        SimState.setTime(this.timeList[i]);
                        return;
                    }
                }
            }
        },
        
        _intervalUpdate: function () {
            this.timeIndex++;
            if (this.mode == modes.pattern && this.timeList) {
                if (this.timeIndex >= this.timeList.length) {
                    this.timeIndex = 0;
                }
                this.onTimeChange(this.timeList[this.timeIndex]);
            }
            else if (this.mode == modes.Simulation) {
                if (this.timeIndex >= this.endTime * 30) {
                    this.timeIndex = 0;
                }
                this.onTimeChange(this.timeIndex/30.0);
            }

            
        },
        
        simulatePattern: function(pattern) {
            var thisObj = this;
            this.pause();
            Scenes.tryPattern(pattern, function(patternStates, endTime) {
                thisObj.patternStates = patternStates;
                thisObj.timeList = Object.keys(patternStates).sort();
                thisObj.onEndTimeChange(endTime);
                thisObj.timeIndex = 0;
                thisObj.onTimeChange(0);
                thisObj.play();
            });
        },
        
        _serverStateUpdate: function () {

            var thisObj = this;
            if (this.mode !== modes.scene) {
                SimState.getLightState(dojo.hitch(this, this.onStateChange));
                SimState.getTime(function(time) {
                    if (time !== thisObj.currentTime) {
                        thisObj.onTimeChange(time);
                    }
                });
                SimState.getEndTime(function(endTime) {
                    if (endTime !== thisObj.endTime) {
                        thisObj.onEndTimeChange(endTime);
                    }
                });
                SimState.isPlaying(function(isPlaying) {
                    if (isPlaying !== thisObj.isPlaying) {
                        thisObj.onPlayChange(isPlaying);
                    }
                });
            }
        },
    });

});