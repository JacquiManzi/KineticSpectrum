define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
    "kui/ajax/SimState",
    "kui/ajax/Scenes",
    "kui/Simulation/TimeManager"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm, array, SimState, Scenes, TimeManager) {
    var modes = { scene: "Scene", pattern: "Pattern", Simulation: "Simulation" };
    return declare("kui.Simulation.Simulation", null, {

        mode: modes.scene,
        endTime: 0,
        isPlaying: false,
        intervalHandle: null,
        playStartTime: null,
        pauseTime: 0,
        timeManager: null,

        constructor: function () {
            SimState.setMode(this.mode);
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onTimeChange", dojo.hitch(this, this._onTimeChange));
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onPlayChange", dojo.hitch(this, this._onPlayChange));
            this.timeManager = new TimeManager();
        },
        
        _onSimMode: function(enabled) {
            SimState.setMode(this.mode);
            if (!enabled) {
                this.pause();
            }
        },
        
        onSimMode: function(enabled) { },
        
        _onEndTimeChange: function(endTime) {
            this.endTime = endTime;
        },
        
        onEndTimeChange: function(endTime) { },
        
        _onTimeChange: function (time) {
            if (this.mode == modes.pattern) {
                var deferred = this.timeManager.getState(time);
                if (!deferred.isResolved()) {
                    console.log("Buffering....");
                    this.pause();
                }
                deferred.then(dojo.hitch(this, function(state) {
                    this.onStateChange(state);
                    this.play();
                }));
            }
        },
        
        onTimeChange: function(time) { }, 
        onStateChange: function(stateList) { },
        
        _onPlayChange: function (isPlaying) {
            this.isPlaying = isPlaying;
            clearInterval(this.intervalHandle);
            if (isPlaying) {
                this.playStartTime = Date.now() - this.pauseTime;
                this.intervalHandle = setInterval(dojo.hitch(this, this._intervalUpdate), 1000/30);
            }
        },
        
        onPlayChange: function(isPlaying) { }, 
        
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
                this.pauseTime = this.getTime();
                this.onPlayChange(false);
                SimState.setTime(this.getTime());
                SimState.pause();
            }
        },
        
        getTime: function() {
            var sinceStart = ((Date.now() - this.playStartTime) / 1000);
            return this.isPlaying ? sinceStart % this.endTime : this.pauseTime;
        },
        
        setTime: function (time) {
            if (Math.abs(this.getTime() - time) > .01) {
                this.pauseTime = time;
                this.playStartTime = Date.now() - time;
                this.onTimeChange(time);
                SimState.setTime(time);
            }
        },
        
        _intervalUpdate: function () {
            this.onTimeChange(this.getTime());
        },
        
        simulatePattern: function(pattern) {
            var thisObj = this;
            this.pause();
            this.pauseTime = 0;
            Scenes.tryPattern(pattern, function(endTime) {
                thisObj.timeManager.newSimulation(endTime)
                    .then(function() {
                        thisObj.onEndTimeChange(endTime);
                        thisObj.onTimeChange(0);
                        thisObj.play();
                    });
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