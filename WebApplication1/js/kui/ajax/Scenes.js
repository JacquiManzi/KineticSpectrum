define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojo/_base/array",
    "kui/LED/LightAddress"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, array, LightAddress) {
        "use strict";


        var getGroupNames = function (onLoad) {
            xhr.get({
                url: "Env.svc/GetGroupNames",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };
        

        var getPatternNames = function (onLoad) {
            xhr.get({
                url: "Env.svc/GetPatternNames",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };

        var tryPattern = function(pattern, onLoad) {
            xhr.post({
                url: "PatternService.svc/TryPattern",
                content: { d: JSON.stringify(pattern) },
                handleAs: "json",
                load: function (lightStates) {
                    var time;
                    var endTime = 0;
                    for (time in lightStates) {
                        endTime = Math.max(time, endTime);
                        array.forEach(lightStates[time], function (lightState) {
                            lightState.address = new LightAddress(lightState.address);
                        });
                    }
                    onLoad(lightStates, endTime);  
                },
                error: function(err) {
                    console.log(err.stack);
                }
            });
        };

        return {
            getGroupNames: getGroupNames,
            getPatternNames: getPatternNames,
            tryPattern: tryPattern
        };

    });