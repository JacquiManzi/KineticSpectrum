define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojo/_base/array",
    "kui/LED/LightAddress",
    "dojox/collections/ArrayList"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, array, LightAddress, ArrayList) {
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

        var getGroups = function (onSuccessFunc) {
               
            xhr.get({

                url: "Env.svc/GetGroups",
                handleAs: "json",
                load: function (jsonData) {
                    var groupList = new ArrayList();
                        
                    array.forEach(jsonData, function (group) {
                        var lightList = new ArrayList();
                        array.forEach(group.lights, function(lightAddress) {
                            lightList.add(new LightAddress(lightAddress));
                        });
                        group.lights = lightList;
                        groupList.add(group);
                    });
                    onSuccessFunc(groupList);

                },
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

        var selectGroups = function(groups) {
            xhr.post({
                url: "Env.svc/SelectGroups",
                content: { d: JSON.stringify(groups) }
            });
        };

        var selectLEDs = function(leds) {
            xhr.post({
                url: "Env.svc/SelectLights",
                content: { d: JSON.stringify(leds) }
            });
        };

        var addLED = function (led) {
            xhr.post({
                url: "Env.svc/AddLED",
                handleAs: "json",
                content: { d: JSON.stringify(led) }
            });
        };

        return {
            getGroupNames: getGroupNames,
            getPatternNames: getPatternNames,
            getGroups: getGroups,
            tryPattern: tryPattern,
            addLED: addLED,
            selectLEDs: selectLEDs,
            selectGroups: selectGroups
        };

    });