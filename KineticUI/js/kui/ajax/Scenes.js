/*
*@Author: Jacqui Manzi
*Novemeber 22nd, 2013
*jacqui@revkitt.com
*
* Scenes.j - Ajax class for retrieving group information for the Scene Model.
*/
define([
    "dojo/_base/declare",
    "dojo/_base/xhr",
    "dojo/_base/array",
    "kui/ModelView/Node/LightAddress",
    "dojox/collections/ArrayList"
],
    function (declare, xhr, array, LightAddress, ArrayList) {

        var getGroupNames = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Env.svc/GetGroupNames",
                handleAs: "json", 
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getPatternNames = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Env.svc/GetPatternNames",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };

        var getGroups = function (onSuccessFunc) {
            onSuccessFunc = onSuccessFunc || function() {};

            return xhr.get({

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
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };

        var deleteGroup = function (groupName) {

            return xhr.get({
                url: "Env.svc/DeleteGroup",
                handleAs: "json",
                content: { groupName: groupName }
            });
        };

        

        var tryPattern = function (pattern, onLoad) {
            onLoad = onLoad || function() { };
            return xhr.post({
                url: "PatternService.svc/TryPattern",
                content: { d: JSON.stringify(pattern) },
                handleAs: "json",
                load: function(endTime) {
                    onLoad(endTime / 1000);
                },
                error: function(err) {
                    console.log(err.stack);
                }
            });
        };

        var selectGroups = function(groups) {
            return xhr.post({
                url: "Env.svc/SelectGroups",
                content: { d: JSON.stringify(groups) }
            });
        };

        var selectLEDs = function(leds) {
            return xhr.post({
                url: "Env.svc/SelectLights",
                content: { d: JSON.stringify(leds) }
            });
        };

        var addLEDs = function (leds, addressFunc) {
            addressFunc = addressFunc || function() {};
            return xhr.post({
                url: "Env.svc/AddLEDs",
                handleAs: "json",
                content: { d: JSON.stringify(leds) },
                load: function(data) {
                    addressFunc(data);
                }
            });
        };

        var addLED = function (led, addressFunc) {
            addressFunc = addressFunc || function() {};
            return xhr.post({
                url: "Env.svc/AddLED",
                handleAs: "json",
                content: { d: JSON.stringify(led) },
                load: addressFunc
            });
        };
        
        var removeLED = function (led) {
            return xhr.post({
                url: "Env.svc/RemoveLED",
                handleAs: "json",
                content: { d: JSON.stringify(led.address) }
            });
        };

        return {
            getGroupNames: getGroupNames,
            getPatternNames: getPatternNames,
            getGroups: getGroups,
            deleteGroup: deleteGroup,
            tryPattern: tryPattern,
            addLED: addLED,
            addLEDs: addLEDs,
            selectLEDs: selectLEDs,
            selectGroups: selectGroups,
            removeLED:removeLED,
        };

    });