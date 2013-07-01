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
                    console.log(err1.stack);
                }
            });
        };
        
        var getOrderingTypes = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetOrderingTypes",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getRepeatMethods = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetRepeatMethods",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getEasings = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetEasings",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };

        var getEffectDefinition = function(effectName, onLoad) {
            xhr.get({
                url: "Effects.svc/GetEffectDef",
                handleAs: "json",
                content: { effectName: effectName },
                load: onLoad,
                error: function(err1, err2) {
                    console.log(err1.stack);
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
                    console.log(err1.stack);
                }
            });
        };

        var getColorEffects = function (onLoad) {
            xhr.get({
                url: "Effects.svc/GetColorEffects",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log(err1.stack);
                }
            });
        };


        return {
            getEffects: getEffects,
            getEffectDefinition: getEffectDefinition,
            getOrderingForType: getOrderingForType,
            getOrderingTypes: getOrderingTypes,
            getRepeatMethods: getRepeatMethods,
            getEasings: getEasings,
            getColorEffects: getColorEffects
        };

    });