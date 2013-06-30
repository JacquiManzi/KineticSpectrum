
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three) {
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
            }


           

        });

    });
