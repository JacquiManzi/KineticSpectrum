
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

            constructor: function (groupCount, selectedNodes, groupName) {


                this.name = groupName;
                this.defaultName = "Group" +  " " + groupCount;
                this.address = null;
                this.groupOption = this.createGroupOption();
                this.groupOption.list = selectedNodes;

            },

            createGroupOption: function () {
                
                
                if (!this.name) {
                    this.name = this.defaultName;
                }


                var option = html.createOption(this.name);

                return option;
            },

            applyGroup: function () {

                var addresses = [];

                this.groupOption.list.forEach(function(item){
                
                    if(!!item.address)
                    {
                        addresses.push(item.address);
                    }
                })

                var group = 
                    {
                        name: this.name, 
                        lights: addresses
                    }

                xhr.post({

                    url: "Env.svc/EditGroup",
                    content: { d: JSON.stringify(group) }
                });


            }


           

        });

    });
