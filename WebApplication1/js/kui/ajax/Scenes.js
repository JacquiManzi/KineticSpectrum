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

        var tryPattern = function(pattern) {
            xhr.post({
                url: "PatternService.svc/TryPattern",
                content: { d: JSON.stringify(pattern) }
            });
        };

       

        return {
            getGroupNames: getGroupNames,
            getPatternNames: getPatternNames,
            tryPattern: tryPattern
        };

    });