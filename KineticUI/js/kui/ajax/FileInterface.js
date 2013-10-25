﻿define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/ModelView/Node/LightAddress"
],
    function (declare, html, dom, domStyle, domConstruct, three, xhr, ArrayList, array, LightAddress) {
        "use strict";
        return declare("kui.ajax.FileInterface", null, {

            /*
             *   
             *
             */

            constructor: function () {

              
            },
            getGroups: function (onSuccessFunc) {

                
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


            },
            getLightConfigList: function (onSuccessFunc) {

                var lightInfoList = new ArrayList();
                var file = xhr.get({

                    url: "Env.svc/GetLEDNodes",
                    handleAs: "json",
                    load: function (jsonData) {

                        array.forEach(jsonData, function(item) {
                            item.address = new LightAddress(item.address);
                            lightInfoList.add(item);
                        });

                        
                        onSuccessFunc(lightInfoList);

                    },
                    error: function (err1, err2) {
                        console.log(err1.stack);
                    }


                });


            }


        });

    });