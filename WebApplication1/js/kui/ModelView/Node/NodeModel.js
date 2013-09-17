/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacquimanzi@gmail.com
*
*   NodeModel.js - 
*   
*/

define([
        "dojo/_base/declare",
        "dojox/collections/ArrayList",
        "kui/ModelView/Node/LED",
        "kui/ModelView/Node/LightAddress",
        "dojo/dom-geometry",
        "kui/util/CommonHTML",
        "dojo/on",
        "kui/ModelView/groups/Group",
        "dojo/_base/array",
        "kui/ajax/Scenes"
    ],
    function (declare, ArrayList, LED, LightAddress, domGeom, html, on, Group, array, Scenes) {
        "use strict";
        return declare("kui.ModelView.Node.NodeModel", null, {
            nodes: null,
            addressToLED: null, 
                 
            constructor: function(scene) {
                this.nodes = new ArrayList();
                this.addressToLED = [];
                this.scene = scene;
                this.selected = new ArrayList();

                this.lightNo = 0;
                this.portNo = 0;
                this.fixtureNo = 0;
            },   

            createLEDNodes: function(ledList) {

                ledList.forEach(dojo.hitch(this, function(item) {

                    var ledNode = new LED();
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

                var led = new LED();
                led.updatePosition(position);
                led.setRadius(distance * .003);
                led.address = address;

                this.addressToLED[address] = led;

                Scenes.addLED({ address: address, position: led.position, color:0 });

                this.nodes.add(led);
                this.scene.add(led);
            },

            addGeneratedLED: function (segment, address) {

                var led = new LED();
                led.updatePosition(segment);
                led.address = address;
                led.setRadius(.003);

                Scenes.addLED({ address: led.address, position: led, color: 0 });

                this.addressToLED[address] = led;
                this.nodes.add(led);
                this.scene.add(led); 
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
            },

            addNodeToSet: function (node) {
                this.nodes.add(node);
            },

            removeNodeFromSet: function (node) {
                this.nodes.remove(node); 
            }
        });
    });