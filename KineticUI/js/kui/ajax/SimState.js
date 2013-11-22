/*
*@Author: Jacqui Manzi
*Novemeber 22nd, 2013
*jacqui@revkitt.com
*
* SimState.js - Ajax class for sending and retrieving pattern effects to the server.
*               Also used for sending the current state: Simulation mode or Live mode.
*/
define([
    "dojo/_base/declare",
    "dojo/_base/xhr",
    "dojo/_base/array",
    "kui/ModelView/Node/LightAddress",
    "dojox/collections/ArrayList"
],
    function (declare, xhr, array, LightAddress, ArrayList) {

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

        var createPattern = function (patternDef) {
            xhr.post({
                url: "Env.svc/SetPattern",
                content: { d: JSON.stringify(patternDef) }
            });
        };

        var removePattern = function (patternName, onLoad) {
            xhr.get({
                url: "Env.svc/DeletePattern",
                handleAs: "json",
                sync: true,
                content: { patternName:patternName },
                load: function () {
                   onLoad();        
                },
                error: function (err1) {   
                    if (!!err1) {    
                        console.log(err1.stack);    
                    }
                }
            });
        };

        var getPatternNames = function (onLoad) {
            xhr.get({
                url: "Env.svc/GetPatternNames",
                handleAs: "json",
                sync: true,
                load: function (patternNames) {
                    //array.forEach(patternNames, function (name) {
                        
                    //});
                    onLoad(patternNames);
                },
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            }); 
        };

        var getPatterns = function(onLoad){

            xhr.get({
                url: "Env.svc/GetPatterns",
                handleAs: "json",
                sync: true, 
                load: function (patterns) {
                    //array.forEach(patterns, function (pattern) {

                    //});
                    onLoad(patterns);
                },
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });

        };

        var addStart = function (patternName, patternID, startTime) {

            return xhr.get({
                url: "SimService.svc/AddStart",
                content: { patternName: patternName, startTime: startTime, id: patternID }
            });

        };
        var removeStart = function ( patternID) {

            return xhr.get({
                url: "SimService.svc/RemoveStart",
                content: { id: patternID }
            });

        };

        var getPatternStarts = function (onLoad) {

            xhr.get({
                url: "SimService.svc/GetStarts",
                handleAs: "json",
                load: function (data) {
                    var list = new ArrayList();
                    array.forEach(data,function (start) {
                        start.countID = start.id;
                        start.startTime = start.startTime / 1000.0;
                        start.endTime = start.endTime / 1000.0;
                        delete start.id;
                        list.add(start);
                        start.yCount = 30 + start.startTime * 5

                    });
                    onLoad(list);
                },
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
                    array.forEach(states, function(state) {
                        state.address = new LightAddress(state.address);
                    });
                    onLoad(states);
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
                load: function (data) { onLoad(data.d); },
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
                load: function (data) { onLoad(data.d); },
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
                load: function (data) { onLoad(data.d); },
                error: function (err1) {
                    if (!!err1) {
                        console.log(err1.stack);
                    }
                }
            });
        };

        return {
            setMode: setMode,
            createPattern: createPattern,
            removePattern: removePattern, 
            getPatternNames: getPatternNames,
            getPatterns: getPatterns, 
            getLightState: getLightState,
            play: play,
            pause: pause,
            isPlaying: isPlaying,
            getTime: getTime,
            setTime: setTime,
            getEndTime: getEndTime,
            addStart: addStart,
            getPatternStarts: getPatternStarts,
            removeStart: removeStart
        };
    });