

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
    "dojo/dom-geometry",
    "dojo/_base/array"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, on, d3, domGeom, array) {
        "use strict";
        return declare("kui.DesignMenu.Timeline", null, {

            /*   
             */

            constructor: function () {

                this.svg = null;
                this.div = null;
                this.barMap = d3.map();
                this.canvasHeight = 300;
                this.canvasWidth = 0;
                this.defaultCanvasWidth = 0;
                this.defaultCanvasHeight = 300;
                  
                this.patternGroup = null;

            },

            createCanvas: function (div) {

                this.div = div;
                html.removeDomChildren(div); 

                var canvasWidth = domGeom.getMarginSize(div).w;
                this.defaultCanvasWidth = canvasWidth;

                if (this.canvasWidth === 0) {
                    this.canvasWidth = canvasWidth; 
                }
              
               this.svg = d3.select(div).append("svg:svg")
		            .attr("id", "svg")
	                .attr("width", this.canvasWidth)
	                .attr("height", this.canvasHeight)
                    .style("overflow", "auto"); 

               this.patternGroup = this.svg.append("g");

               this.createAxis();     

            },

            createAxis: function () {        

                var rangeVal = this.canvasHeight;   
                var axisScale = d3.scale.linear()
                                         .domain([0,rangeVal/5])
                                         .range([0,rangeVal+6]);   
                
                var yAxis = d3.svg.axis()
                                  .orient("left")                                    
                                  //.ticks(d3.time.seconds, 200)
                                  .scale(axisScale); 
                    
                var yAxisGroup = this.svg.append("g")
                .attr("transform", "translate(35,30)")
                .style('fill', 'white')   
                .call(yAxis);   

            },

            addBars: function (patternData) {

                this.clearCanvas();
                this.createCanvas(this.div); 

                var rectangles = this.patternGroup.selectAll("rect")
                .data(patternData)
                .enter() 
                .append("rect");

                this.svg.selectAll("rect").on("click", function () {
                  
                    if (this.getAttribute('selected') === "false") {
                        d3.select(this).attr('r', 25)
                            .style("fill", "lightcoral")
                            .style("stroke", "red");
                           
                        this.setAttribute('selected', 'true');
                    }
                    else {
                        d3.select(this).attr('r', 25)
                            .style("fill", this.getAttribute('color'))
                            .style("stroke", this.getAttribute('color'));
                              
                        this.setAttribute('selected', 'false'); 
                    }
                }); 
                   
                  
                var rectangleAttributes = rectangles
                .attr("x", function (d) { return d.rx; })
                .attr("y", function (d) { return d.ry; })
                .attr("height", function (d) { return d.height; })
                .attr("width", function (d) { return d.width; })
                .attr("pattern", function (d) { return d.pattern; })
                .attr("selected", function (d) { return d.selected; })
                .attr("color", function (d){ return d.color;})
                .attr("countID", function(d) { return d.countID;})
                .style("fill", function (d) { return d.color; });
            },

            getSelectedBars: function(){

                var bars = this.svg.selectAll("rect");
                var selectedBars = [];

                array.forEach(bars, function (bar) {

                    array.forEach(bar, function (rect) {
                        if (rect.getAttribute('selected') ===  "true") {
                            selectedBars.push(rect);
                        }
                    });     
                });

                return selectedBars; 
            },
               
            clearCanvas: function () {
               this.svg.remove();              
            }   






        }); 

    });
