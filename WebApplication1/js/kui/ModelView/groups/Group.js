
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/_base/xhr"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, xhr) {
        "use strict";

        return declare("kui.LED.LEDNode", null, {

            /*
             *  
             *
             */

            constructor: function (groupName, selectedNodes) {

                this.name = groupName;
                this.selectedNodes = selectedNodes;
                var addresses = this.addresses = [];
                
                this.selectedNodes.forEach(function (item) {

                    if (!!item.address) {
                        addresses.push(item.address);
                    }
                });
            },
            
            selectAll: function () {
                this.selectedNodes.forEach(function(node) {
                    node.select();
                });
            },
            
            deselectAll: function() {
                this.selectedNodes.forEach(function(node) {
                    node.unselect();
                });
            },

            applyGroup: function () {

                var group =
                {
                    name: this.name,
                    lights: this.addresses
                };

                xhr.post({

                    url: "Env.svc/EditGroup",
                    content: { d: JSON.stringify(group) }
                });

            },
            
            remove: function() {
                xhr.get({
                    url: "Env.svc/DeleteGroup",
                    content: { groupName: this.name }
                });
            },


           

        });

    });
