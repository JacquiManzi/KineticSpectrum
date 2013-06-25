

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
        return declare("kui.PatternMenu.ColorEffect", null, {

            /*
             *   
             *
             */

            constructor: function () {


                this.type = "";
                this.color = 0x00;
                this.colors = new ArrayList();



            }


        });

    });
