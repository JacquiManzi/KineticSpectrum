

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
        return declare("kui.PatternMenu.PatternStart", null, {

            /*
             *   
             *
             */

            constructor: function () {


                this.pattern = 0;
                this.priority = 0;
                this.startTime = 0.0;
                this.endTime = 0.0;
                this.id = 0;


            }


        });

    });
