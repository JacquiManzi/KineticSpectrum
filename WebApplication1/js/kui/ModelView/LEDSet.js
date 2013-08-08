define([
        "dojo/_base/declare",
        "kui/ModelView/ModelSkeleton",
        "dojox/collections/ArrayList",
        "kui/LED/LEDNode",
        "kui/LED/LightAddress",
        "threejs/three",
        "dojo/dom-geometry",
        "kui/ModelView/VertexSphere",
        "kui/util/CommonHTML",
        "kui/Pattern/patterns/PatternModel",
        "dojo/on",
        "kui/ModelView/groups/Group"
    ],
    function(declare, ModelSkeleton, ArrayList, LEDNode, LightAddress, three, domGeom, VertexSphere, html, PatternModel, on, Group) {
        "use strict";
        return declare("kui.ModelView.LEDSet", null, {
            nodes: null,
            vertexSpheres: null,
            addressToLED: null,
            ledRadius: 3,
            
            constructor: function(scene) {
                this.nodes = new ArrayList();
                this.vertexSpheres = new ArrayList();
                this.addressToLED = [];
                this.scene = scene;
            },
            
            createLEDNodes: function(ledList) {

                ledList.forEach(dojo.hitch(this, function(item) {

                    var ledNode = new LEDNode();
                    ledNode.updatePosition(item.position);
                    ledNode.address = item.address;
                    ledNode.radius = 3;

                    var ledSphere = ledNode.createSphere();
                    this.scene.add(ledSphere);
                    this.nodes.add(ledSphere);
                    this.addressToLED[item.address.toString()] = ledSphere;
                }));
            },
            
            deselectAllVertexs: function () {

                this.nodes.forEach(function (node) {
                    if (node.isVertex) {
                        node.unselect();
                    }
                });


            },

            selectAllVertexs: function () {
                this.nodes.forEach(function (node) {
                    if (node.isVertex) {
                        node.select();
                    }
                });
            },
            
            selectAllLEDs: function () {

                this.nodes.forEach(function(node) {
                    if (!node.isVertex) {
                        node.select();
                    }
                });

            },

            deselectAllLEDs: function () {
                this.nodes.forEach(function (node) {
                    if (!node.isVertex) {
                        node.unselect();
                    }
                });

            },

            selectNodes: function (nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var thisObj = this;
                nodes.forEach(function (node) {
                    thisObj.selectNode(node);
                });
            },

            selectNode: function (node) {
                node.select();
            },

            deselectNode: function (node) {
                node.unselect();
            },

            deselectNodes: function (nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var thisObj = this;
                nodes.forEach(function (node) {
                    thisObj.deselectNode(node);
                });
            },
            
            
            addSingleLED: function (intersects, address) {
                address = address ? address : new LightAddress();
                
                var distance = intersects[0].object.geometry.boundingBox.min.distanceTo(intersects[0].object.geometry.boundingBox.max);
                var position = intersects[0].point;

                var led = new LEDNode();
                led.updatePosition(position);
                led.radius = distance * .003;
                this.ledRadius = led.radius;
                led.address = address;

                var ledSphere = led.createSphere();

                /*Adjust the sphere radius according to model scale*/

                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);
            },
            
            getLEDNode: function(lightAddress) {
                return this.addressToLED[lightAddress.toString()];
            },
            
            getSelectedNodes: function () {

                var selectedNodes = new ArrayList();

                this.nodes.forEach(function(node) {
                    if (node.isSelected) {
                        selectedNodes.add(node);
                    }
                });
                
                return selectedNodes;
            },
            
            applyColorState: function(lightStateList) {

                var addressToLED = this.addressToLED;
                array.forEach(lightStateList, function(state) {
                    addressToLED[state.address.toString()].setColor(state.color);
                });
            },

            getAllLEDs: function () {

                this.leds.clear();
                for (var i = 0; i < this.ledSet.nodes.count; i++) {

                    if (!this.ledSet.nodes.item(i).isVertex) {
                        this.leds.add(this.ledSet.nodes.item(i));
                    }

                }

            }
            
        });
    });