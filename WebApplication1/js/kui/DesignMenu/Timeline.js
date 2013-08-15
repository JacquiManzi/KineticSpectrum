

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/on",
    "d3js/d3",
    "dojo/dom-geometry"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, on, d3, domGeom) {
        "use strict";
        return declare("kui.DesignMenu.Timeline", null, {

            /*
             */

            constructor: function () {

            },

            createCanvas: function (div) {

                var canvasWidth = domGeom.getMarginSize(div).w;
                var canvasHeight = domGeom.getMarginSize(div).h;

                var svg = d3.select(div).append("svg:svg")
		            .attr("id", "svg")
	                .attr("width", canvasWidth)
	                .attr("height", canvasHeight);

            }

        }); 

    });
