

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
        return declare("kui.PatternMenu.patterns.Group", null, {

            /*
             *  Group
             *
             */

            constructor: function () {

                
                this.name = "";
                this.lights = new ArrayList();
                this.groups = new ArrayList();              
            }


        });

    });
