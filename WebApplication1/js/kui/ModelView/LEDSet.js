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
        "kui/ModelView/groups/Group",
        "dojo/_base/array",
        "kui/ajax/Scenes"
    ],
    function (declare, ModelSkeleton, ArrayList, LEDNode, LightAddress, three, domGeom, VertexSphere, html,
        PatternModel, on, Group, array, Scenes) {
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
                this.selected = new ArrayList();

                this.lightNo = 0;
                this.portNo = 0;
                this.fixtureNo = 0;
            },   

            createLEDNodes: function(ledList) {

                ledList.forEach(dojo.hitch(this, function(item) {

                    var ledNode = new LEDNode();
                    ledNode.updatePosition(item.position);
                    ledNode.address = item.address;
                    ledNode.radius = 0.005;   

                    var ledSphere = ledNode.createSphere();
                    this.scene.add(ledSphere);
                    this.nodes.add(ledSphere);
                    this.addressToLED[item.address.toString()] = ledSphere;
                }));
            },
            
            updateServerSelection: function() {
                var addresses = [];
                this.nodes.forEach(function(node) {
                    if (node.isSelected && node.address) {
                        addresses.push(node.address);
                    }
                });
                Scenes.selectLEDs(addresses);
            },
            
            deselectAllVertexs: function () {
                var selected = this.selected;
                this.nodes.forEach(function (node) {
                    if (node.isVertex) {
                        node.unselect();
                        selected.remove(node);
                    }
                });
                this.updateServerSelection();
            },   

            selectAllVertexs: function () {
                var selected = this.selected;
                this.nodes.forEach(function (node) {
                    if (node.isVertex) {
                        node.select();
                        selected.add(node);
                    }
                });
                this.updateServerSelection();
            },
            
            selectAllLEDs: function () {
                var selected = this.selected;
                this.nodes.forEach(function(node) {
                    if (!node.isVertex) {
                        node.select();
                        selected.add(node);
                    }
                });
                this.updateServerSelection();
            },

            deselectAllLEDs: function () {
                var selected = this.selected;
                this.nodes.forEach(function (node) {
                    if (!node.isVertex) {
                        node.unselect();
                        selected.remove(node);
                    }
                });
                this.updateServerSelection();
            },

            selectNodes: function (nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var selected = this.selected;

                nodes.forEach(function (node) {
                    node.select();
                    selected.add(node);
                });
                this.updateServerSelection();
            },

            selectNode: function (node) {
                node.select();
                this.selected.add(node);
                this.updateServerSelection();
            },

            deselectNode: function (node) {
                node.unselect();
                this.selected.remove(node);
                this.updateServerSelection();
            },

            deselectNodes: function (nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var selected = this.selected;

                nodes.forEach(function (node) {
                    node.unselect();
                    selected.remove(node);
                });
                this.updateServerSelection();
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

                this.addressToLED[address] = ledSphere;

                Scenes.addLED({ address: address, position: ledSphere.position, color:0 });

                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);
            },

            /*addGeneratedLED: function(segment, address){

                address = address ? address : new LightAddress();
                var led = new LEDNode();
                led.updatePosition(segment);
                led.address = address;

                var ledSphere = led.createSphere();

                this.addressToLED[address] = ledSphere;
                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);
            },*/

            addGeneratedLED: function (segment, address) {

                var led = new LEDNode();
                led.updatePosition(segment);
                led.address = address;

                var ledSphere = led.createSphere();

                Scenes.addLED({ address: led.address, position: led.position, color: 0 });

                this.addressToLED[address] = ledSphere;
                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);
            },
            
            getLEDNode: function(lightAddress) {
                return this.addressToLED[lightAddress.toString()];
            },
            
            getSelectedNodes: function () {

                var selectedNodes = new ArrayList();

                this.selected.forEach(function(node) {
                        selectedNodes.add(node);
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