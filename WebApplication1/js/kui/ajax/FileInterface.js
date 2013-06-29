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
        "use strict";
        return declare("kui.ajax.FileInterface", null, {

            /*
             *   
             *
             */

            constructor: function () {

              
            },

            loadLightConfig: function () {


                var file = xhr.get({

                    url: "Env.svc/GetLEDNodes",
                    handleAs: "json",
                    load: function (jsonData) {

                        //for( var i = 0; i < jsonData.
                        var file = null;

                    },
                    error: function (err1, err2) {
                        console.log("Error!");
                    }


                });


            }


        });

    });