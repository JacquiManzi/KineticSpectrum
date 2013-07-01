define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojo/_base/array"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, array) {
        "use strict";

        var getEffects = function(onLoad) {
            xhr.get({
                url: "Effects.svc/GetEffects",
                handleAs: "json",
                load: onLoad,
                error: function(err1, err2) {
                    console.log("Error Getting list of effects");
                }
            });
        };
        
        var getOrderingTypes = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetOrderingTypes",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting list of effects");
                }
            });
        };
        
        var getRepeatMethods = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetRepeatMethods",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting list of effects");
                }
            });
        };
        
        var getEasings = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetEasings",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting list of effects");
                }
            });
        };

        var getEffectDefinition = function(effectName, onLoad) {
            xhr.get({
                url: "Effects.svc/GetEffectDef",
                handleAs: "json",
                content: { effectName: effectName },
                load: function(defs) {
                    array.forEach(defs, function(def) {
                        for (key in def) {
                            var newKey = key.charAt(0).toUpperCase() + key.slice(1);
                            if (newKey !== key) {
                                def[newKey] = def[key];
                                delete def[key];
                            }
                        }
                    });
                    onLoad(defs);
                },
                error: function(err1, err2) {
                    console.log("Error Getting Effect Definition for effect " + effectName);
                }
            });
        };
        
        var getOrderingForType = function (orderingType, onLoad) {
            xhr.get({
                url: "Effects.svc/GetOrderingForType",
                handleAs: "json",
                content: { type: orderingType },
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting Effect Definition for effect " + effectName);
                }
            });
        };

        return {
            getEffects: getEffects,
            getEffectDefinition: getEffectDefinition,
            getOrderingForType: getOrderingForType,
            getOrderingTypes: getOrderingTypes,
            getRepeatMethods: getRepeatMethods,
            getEasings: getEasings
        };

    });