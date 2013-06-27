

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList) {
        "use strict";
        return declare("kui.PatternMenu.patterns.Color", null, {

            /*
             *   
             *
             */

            constructor: function () {


                this.r = 0x00;
                this.g = 0x00;
                this.b = 0x00;
                this.a = 0x00;

            }


        });

    });
