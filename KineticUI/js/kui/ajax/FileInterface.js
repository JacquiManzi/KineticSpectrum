/*
*@Author: Jacqui Manzi
*Novemeber 22nd, 2013
*jacqui@revkitt.com
*
* FileInterface.j - Ajax class for retrieving saved LED nodes and groups from the server.
*                   Allows for LEDs to be present again after page reload.
*/

define([
    "dojo/_base/declare",
    "dojo/_base/xhr",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/ModelView/Node/LightAddress"
],
    function (declare, xhr, ArrayList, array, LightAddress) {
        "use strict";
        return declare("kui.ajax.FileInterface", null, {

            /*Retrieve saved groups from the server*/
            /*Returns ArrayList<Map<String>>*/
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
            /*Retrieves the saved LED Nodes from the server*/
            /*Returns ArrayList<Map<String>>*/
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