define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojox/collections/ArrayList",
    "dojo/_base/array"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, ArrayList, array) {
        "use strict";
        return declare("kui.ajax.FileInterface", null, {

            /*
             *   
             *
             */

            constructor: function () {

              
            },

            getLightConfigList: function (onSuccessFunc) {

                var lightInfoList = new ArrayList();
                var file = xhr.get({

                    url: "Env.svc/GetLEDNodes",
                    handleAs: "json",
                    load: function (jsonData) {

                        array.forEach(jsonData, function (item) {

                            lightInfoList.add(item);

                        })

                        
                        onSuccessFunc(lightInfoList);

                    },
                    error: function (err1, err2) {
                        console.log("Error!");
                    }


                });


            }


        });

    });