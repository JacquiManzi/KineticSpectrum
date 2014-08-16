
/*
*   @author: Jacqui Manzi
*   August 22th, 2013
*   jacqui@revkitt.com
*
*   PatternComposerModel.js - Model view controller for the pattern composer menu.
*/
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
     "kui/ajax/SimState"
],
    function (declare, html, ContentPane, domStyle, domConstruct, three, ArrayList, array, SimState) {
        "use strict";
        return declare("kui.DesignMenu.GenerativeMenu.GenerativeModel", null, {

            constructor: function (simulation) {
                this.parameters = {};
                this.simulation = simulation;
            },
        });
    });





