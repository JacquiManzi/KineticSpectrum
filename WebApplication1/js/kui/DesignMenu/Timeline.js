

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

                this.svg = null;
                this.div = null;
                this.barMap = d3.map();

                this.patternGroup = null;

            },

            createCanvas: function (div) {

                this.div = div;
                html.removeDomChildren(div); 

                var canvasWidth = domGeom.getMarginSize(div).w;
                var canvasHeight = domGeom.getMarginSize(div).h;

               this.svg = d3.select(div).append("svg:svg")
		            .attr("id", "svg")
	                .attr("width", canvasWidth)
	                .attr("height", canvasHeight);

               this.patternGroup = this.svg.append("g"); 

            },

            addBars: function (patternData) {

                this.clearCanvas();
                this.createCanvas(this.div); 

                var rectangles = this.patternGroup.selectAll("rect")
                .data(patternData)
                .enter()
                .append("rect");            

                  
                var rectangleAttributes = rectangles
                .attr("x", function (d) { return d.rx; })
                .attr("y", function (d) { return d.ry; })
                .attr("height", function (d) { return d.height; })
                .attr("width", function (d) { return d.width; })
                .style("fill", function (d) { return d.color; });
            },

            clearCanvas: function () {
               this.svg.remove();
            }






        }); 

    });
