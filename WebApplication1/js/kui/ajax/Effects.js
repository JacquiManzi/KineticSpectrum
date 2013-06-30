define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr) {
        "use strict";

        var getEffects = function (onLoad) {
            var file = xhr.get({

                url: "Effects.svc/GetEffects",
                handleAs: "json",
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting list of effects");
                }


            });
        }

        var getEffectDefinition = function (effectName, onLoad) {
            var file = xhr.get({

                url: "Effects.svc/GetEffectDef",
                handleAs: "json",
                content:{effectName:effectName},
                load: onLoad,
                error: function (err1, err2) {
                    console.log("Error Getting Effect Definition for effect " + effectName);
                }
            });
        }

        return {
            getEffects: getEffects,
            getEffectDefinition: getEffectDefinition
        }

    });