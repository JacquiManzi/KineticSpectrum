define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/BorderContainer",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct"

],
    function (declare, html, BorderContainer, dom, ContentPane, domStyle, domConstruct) {
        "use strict";
        return declare("kui.ModelMenu.Model", null, {

            /*
             *   3D Model file
             *
             */

            constructor: function (file, name) {

                this.file = file;
                this.name = name;

                this.createModelFromFile(file)

            },

            createModelFromFile: function (file) {


            }

           


        });

    });

