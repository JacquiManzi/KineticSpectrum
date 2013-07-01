define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/LED/LightAddress"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, ArrayList, array, LightAddress) {
        "use strict";

        var setMode = function (mode) {
            var modeInt;
            switch(mode) {
                case "Scene":
                    modeInt = 1;
                    break;
                case "Pattern":
                    modeInt = 2;
                    break;
                case "Simulation":
                    modeInt = 3;
                    break;
                default:
                    throw new Error("Simulation mode '" + mode + "' is not a valid simulation mode");
            }
            xhr.get({
                url: "SimState.svc/SetMode",
                content: {mode:modeInt},
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };

        var getLightState = function (onLoad) {
            xhr.get({
                url: "SimState.svc/GetLightState",
                handleAs: "json",
                load: function(states) {
                    var statesList = new ArrayList();
                    array.forEach(states, function(state) {
                        state.address = new LightAddress(state.address);
                    });
                    onLoad(statesList);
                },
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };
        
        var play = function () {
            xhr.get({
                url: "SimState.svc/Play",
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };
        
        var pause = function () {
            xhr.get({
                url: "SimState.svc/Pause",
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };
        
        var isPlaying = function (onLoad) {
            xhr.get({
                url: "SimState.svc/IsPlaying",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };

        var getTime = function(onLoad) {
            xhr.get({
                url: "SimState.svc/GetTime",
                handleAs: "json",
                load: onLoad,
                error: function(err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };

        var setTime = function (time) {
            xhr.get({
                url: "SimState.svc/SetTime",
                content: { time: time },
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };
        
        var getEndTime = function (onLoad) {
            xhr.get({
                url: "SimState.svc/GetEndTime",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };

        return {
            setMode: setMode,
            getLightState: getLightState,
            play: play,
            pause: pause,
            isPlaying: isPlaying,
            getTime: getTime,
            setTime: setTime,
            getEndTime: getEndTime
        };

    });