

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList) {
        "use strict";
        return declare("kui.LED.LightAddress", null, {

            /*
             *  Group
             *
             */

            constructor: function (address) {
                address = address ? address : {};
                this.fixtureNo = -1;
                this.portNo = -1;
                this.lightNo = -1;
                dojo.mixin(this, address);
            },

            toString: function() {
                return this.fixtureNo + '-' + this.portNo + '-' + this.lightNo;
            }
        });

    });
