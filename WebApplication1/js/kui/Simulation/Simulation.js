define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems",
    "dojo/_base/array",
    "kui/ajax/SimState",
    "kui/DesignMenu/EffectMenu/TimeItem",
    "kui/DesignMenu/EffectMenu/IntItem",
    "kui/DesignMenu/EffectMenu/OrderingItem",
    "kui/DesignMenu/EffectMenu/SelectItem"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm, array, SimState, TimeItem, IntItem, OrderingItem, SelectItem) {
    var modes = { scene: "Scene", pattern: "Pattern", Simulation: "Simulation" };
    return declare("Simulation", null, {

        mode: modes.scene,
        endTime: 0,
        currentTime: 0,
        isPlaying: false,
        intervalHandle: null,
            

        constructor: function () {
            SimState.setMode(this.mode);
            this.intervalHandle = setInterval(dojo.hitch(this, this._intervalUpdate), 1000 / 45);
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onTimeChange", dojo.hitch(this, this._onTimeChange));
            dojo.connect(this, "onEndTimeChange", dojo.hitch(this, this._onEndTimeChange));
            dojo.connect(this, "onPlayChange", dojo.hitch(this, this._onPlayChange));
        },
        
        _onSimMode: function(enabled) {
            SimState.setMode(this.mode);
            if (enabled) {
                this._updateState();
            }
        },
        
        onSimMode: function(enabled) {
            
        },
        
        _onEndTimeChange: function(endTime) {
            this.endTime = endTime;
        },
        
        onEndTimeChange: function(endTime) {
            
        },
        
        _onTimeChange: function(time) {
            this.currentTime = time;
        },
        
        onTimeChange: function(time) {
        },
        
        onStateChange: function(stateList) {
           
        },
        
        _onPlayChange: function (isPlaying) {
            this.isPlaying = isPlaying;
        },
        
        onPlayChange: function(isPlaying) {
        },
        
        setSceneMode: function() {
            this.mode = modes.scene;
            SimState.setMode(this.mode);
            this.onSimMode(false);
        },
        
        setPatternMode: function() {
            this.mode = modes.pattern;
            SimState.setMode(this.mode);
            this.onSimMode(true);
        },
        
        setSimulationMode: function() {
            this.mode = modes.Simulation;
            SimState.setMode(this.mode);
            this.onSimMode(true);
        },
        
        play: function() {
            SimState.play();
            this._stateUpdate();
        },
        
        pause: function() {
            SimState.pause();
            this._stateUpdate();
        },
        
        setTime: function(time) {
            SimState.setTime(time);
            this._stateUpdate();
        },
        
        _intervalUpdate: function() {
            if (this.isPlaying) {
                this._stateUpdate();
            }
        },
        
        _stateUpdate: function () {

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