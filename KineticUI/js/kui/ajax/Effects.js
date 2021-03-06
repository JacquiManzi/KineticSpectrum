﻿/*
*@Author: Jacqui Manzi
*Novemeber 22nd, 2013
*jacqui@revkitt.com
*
* Effects.j - Ajax class for retrieving pattern effects from the server.
*/

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr) {

        var getEffects = function(onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetEffects",
                handleAs: "json",
                load: onLoad,
                error: function(err1) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getOrderingTypes = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetOrderingTypes",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getRepeatMethods = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetRepeatMethods",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getEasings = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetEasings",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };

        var getEffectDefinition = function(effectName, onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetEffectDef",
                handleAs: "json",
                content: { effectName: effectName },
                load: onLoad,
                error: function(err1) {
                    console.log(err1.stack);
                }
            });
        };
        
        var getOrderingForType = function (orderingType, onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetOrderingForType",
                handleAs: "json",
                content: { type: orderingType },
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };

        var getColorEffects = function (onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetColorEffects",
                handleAs: "json",
                load: onLoad,
                error: function (err1) {
                    console.log(err1.stack);
                }
            });
        };

        var getImages = function(onLoad) {
            onLoad = onLoad || function() { };
            return xhr.get({
                url: "Effects.svc/GetImages",
                handleAs: "json",
                load:onLoad,
                error: function(err1) {
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
            getColorEffects: getColorEffects,
            getImages:getImages
        };

    });