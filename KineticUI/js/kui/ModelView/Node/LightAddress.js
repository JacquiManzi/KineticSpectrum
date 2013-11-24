

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
        return declare("kui.ModelView.Node.LightAddress", null, {

            constructor: function (address) {
                address = address ? address : {};
                this.fixtureNo = -1; 
                this.portNo = -1;
                this.lightNo = -1;
                dojo.mixin(this, address);
            },

            isDefined: function () {
                return !(this.fixtureNo === -1 && this.portNo === -1 && this.lightNo === -1);
            },

            inc: function() {
                this.lightNo++;
                if (this.lightNo > 49) {
                    this.portNo++;
                    this.lightNo = 0;
                }

                if (this.portNo > 7) {
                    this.fixtureNo++;
                    this.portNo = 0;
                }
            },

            toString: function() {
                return this.fixtureNo + '-' + this.portNo + '-' + this.lightNo;
            }
        });

    });
