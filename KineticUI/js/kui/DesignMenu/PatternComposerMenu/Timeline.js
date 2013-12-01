

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
    "dojo/_base/array",
     "kui/ajax/SimState"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, on, d3, domGeom, array, SimState) {
        "use strict";
        return declare("kui.DesignMenu.PatternComposerMenu.Timeline", null, {

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
                .style('fill', '#3d8dd5')
                .call(yAxis);   
            },

            addBars: function (patternData) {

                this.clearCanvas();
                this.createCanvas(this.div);

                var dragmove = function(d) {
                    var x = Math.max(44, Math.round(d3.event.sourceEvent.offsetX / 22) * 22);
                    var y = Math.round(d3.event.sourceEvent.offsetY/5) * 5;
                    d.updateStartTime(y);   
                    d3.select(this)
                        .attr("x", d.priority = (x-44)/22)
                        .attr("x", d.xCount = x)
                        .attr("y", d.yCount = y);
                };   

                var dragend = function(d){

                    SimState.addStart(d.getText(), d.countID, d.startTime, d.priority);
                };

                var drag = d3.behavior.drag()
                .origin(Object)
                .on("drag", dragmove)
                .on("dragend", dragend);

                var rectangles = this.svg.selectAll("rect")
                    .data(patternData)
                    .enter().append("rect")
                    .attr("x", function (d) { return d.xCount; })
                    .attr("y", function (d) { return d.yCount; })
                    .attr("height", function (d) { return d.getHeight(); })
                    .attr("width", 22)
                    //.attr("pattern", function (d) { return d.pattern; })
                    .attr("selected",false)
                    .attr("color", function (d) { return d.getColor(); })
                    .attr("countID", function (d) { return d.countID; })
                    .attr("text", function (d) { return d.getText(); })
                    .style("fill", function (d) { return d.getColor(); })
                    .style("stroke", "#FFFFFF")
                    .call(drag);

                rectangles.append("svg:title")
                  .text(function (d) { return d.getText(); });
               
                rectangles.on("click", function () {
                  
                    if (this.getAttribute('selected') === "false") {
                        d3.select(this).attr('r', 25)
                            .style("stroke", "red");
                           
                        this.setAttribute('selected', 'true');
                    }
                    else {
                        d3.select(this).attr('r', 25)
                            .style("fill", this.getAttribute('color'))
                            .style("stroke", "#FFFFFF");
                              
                        this.setAttribute('selected', 'false'); 
                    }
                });


                    
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
