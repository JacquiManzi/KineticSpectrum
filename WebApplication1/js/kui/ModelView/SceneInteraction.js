

define([
    "dojo/_base/declare",
    "kui/ModelView/ModelSkeleton",
    "dojox/collections/ArrayList",
    "kui/LED/LEDNode",
    "threejs/three",
    "dojo/dom-geometry",
    "kui/ModelView/VertexSphere",
     "kui/util/CommonHTML",
     "kui/Pattern/patterns/PatternModel",
        "dojo/on"

],
    function (declare, ModelSkeleton, ArrayList, LEDNode, three, domGeom, VertexSphere, html, PatternModel, on) {
        "use strict";
        return declare("kui.ModelView.SceneInteraction", null, {

            /*
             *   
             *
             */

            constructor: function () {


                this.modelSkeleton = null;

                this.nodes = new ArrayList();
                this.vertexSpheres = new ArrayList();
                this.ledNodes = new ArrayList();

                this.selectedNodes = new ArrayList();
                this.selectedGroups = new ArrayList(); //Selected Vetices that are grouped

                this.projector = new three.Projector();
                this.camera = null;
                this.domNode = null;
                this.orbitControl = null;
                this.dragControls = null;
                this.scene = null;

                this.addModeOn = false;
                this.sceneMesh = null;

                this.fileSelectionType = null;

                this.patternModel = new PatternModel(this);

            },

            createVertexSpheres: function () {

                var geometryList =  this.modelSkeleton.geometryList;
                for (var i = 0; i < geometryList.count; i++) {

                    var vertices = geometryList.item(i).vertices;
                    for (var j = 0; j < vertices.length; j++) {

                        var distance = geometryList.item(i).boundingBox.min.distanceTo(geometryList.item(i).boundingBox.max);
                        var radius = distance * .01;
                        var vertexSphere = new VertexSphere(radius, vertices[j].x, vertices[j].y, vertices[j].z);

                        /*Adjust the sphere radius according to model scale*/

                        vertexSphere.radius = distance * 0.003;

                        this.scene.add(vertexSphere.sphere);
                        this.nodes.add(vertexSphere.sphere);
                        this.vertexSpheres.add(vertexSphere.sphere);

                    }
                }

                this.dragControls = new three.DragControls(this.camera, this.nodes, this.domNode, domGeom);

                dojo.connect(this.domNode, "onmousedown", dojo.hitch(this, this.findSelectionType));
                dojo.connect(this.domNode, "onmouseup", dojo.hitch(this, function (event) {

                    this.orbitControl.enabled = true;
                    this.dragControls.enabled = false;

                }));
                

            },


            /*Find the line segments between selected VertexSpheres*/
            findConnectingLines: function (amount) {

                var lineSegments = new ArrayList();

                this.getSelectedNodes();
                var nodes = this.selectedNodes;

                while (nodes.count > 1) {

                    var sphereOne = nodes.item(0).coords;
                    var sphereTwo = nodes.item(1).coords;

                    var deltaX = (sphereOne.x - sphereTwo.x) / (amount + 1);
                    var deltaY = (sphereOne.y - sphereTwo.y) / (amount + 1);
                    var deltaZ = (sphereOne.z - sphereTwo.z) / (amount + 1);

                    for (var i = 1; i <= amount; i++) {
                        var x = sphereTwo.x + i * deltaX;
                        var y = sphereTwo.y + i * deltaY;
                        var z = sphereTwo.z + i * deltaZ;

                        lineSegments.add(new three.Vector3(x, y, z));
                    }

                    nodes.remove(nodes.item(0));

                }

                this.x = this.selectedNodes.item(0).position.x;
                this.y = this.selectedNodes.item(0).position.y;
                this.z = this.selectedNodes.item(0).position.z;

                return lineSegments;

            },

            getSelectedNodes: function () {

                this.selectedNodes.clear();
                for (var i = 0; i < this.nodes.count; i++) {

                    if (this.nodes.item(i).isSelected) {

                        this.selectedNodes.add(this.nodes.item(i));

                    }


                }

            },
         
            drawNodes: function (lineSegments) {


                for (var i = 0; i < lineSegments.count; i++) {

                    var ledNode = new LEDNode();

                    ledNode.x = lineSegments.item(i).x;
                    ledNode.y = lineSegments.item(i).y;
                    ledNode.z = lineSegments.item(i).z;

                    var sphere = ledNode.createSphere();

                    this.nodes.add(sphere);
                    this.scene.add(sphere);
                }


            },

            removeNodes: function () {

                this.getSelectedNodes();

                for (var i = 0; i < this.selectedNodes.count; i++) {

                    if (!this.selectedNodes.item(i).isVertex) {
                        this.scene.remove(this.selectedNodes.item(i));
                        this.nodes.remove(this.selectedNodes.item(i));
                    }


                }



            },

            getLEDs: function () {

                this.leds.clear();
                for (var i = 0; i < this.nodes.count; i++) {

                    if (!this.nodes.item(i).isVertex) {
                        this.leds.add(this.nodes.item(i));
                    }

                }

            },

            addSelectedGroup: function (listBox) {


                this.getSelectedNodes();
                
                if (this.selectedNodes.count > 0 && !this.selectedGroups.contains(this.selectedNodes)) {

                    var countAmount = this.selectedGroups.count + 1;
                    this.selectedGroups.add(this.selectedNodes);

                    var option = html.createOption("Group" + " " + countAmount);
                    option.list = this.selectedNodes;
                    listBox.domNode.appendChild(option);

                    on(option, "click", dojo.hitch(this, function (listBox) {

                        this.selectedGroupOptions.clear();
                        for (var i = 0; i < listBox.getSelected().length; i++) {
                            this.selectedGroupOptions.add(listBox.getSelected()[i]);
                        }
                        this.showSelectedVertexGroups();


                    }, listBox));

                }

                this.patternModel.updateGroupDropDown();


            },

            removeSelectedGroup: function (listBox) {

                var selectedOptions = listBox.getSelected();

                for (var i = 0; i < selectedOptions.length; i++) {

                    listBox.domNode.removeChild(selectedOptions[i]);
                    this.selectedGroupOptions.remove(selectedOptions[i]);
                }
            },

            deselectAllVertexs: function () {

                for (var i = 0; i < this.nodes.count; i++) {

                    if (this.nodes.item(i).isVertex) {
                        this.nodes.item(i).isSelected = false;

                        var material = new three.MeshNormalMaterial();

                        this.nodes.item(i).setMaterial(material);

                    }

                }

            },

            selectAllVertexs: function () {
                for (var i = 0; i < this.nodes.count; i++) {

                    if (this.nodes.item(i).isVertex) {
                        this.nodes.item(i).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });


                        this.nodes.item(i).setMaterial(selectionMaterial);
                    }


                }
            },

            selectAllLEDs: function () {

                for (var i = 0; i < this.nodes.count; i++) {

                    if (!this.nodes.item(i).isVertex) {
                        this.nodes.item(i).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });


                        this.nodes.item(i).setMaterial(selectionMaterial);
                    }

                }
            },

            deselectAllLEDs: function () {
                for (var i = 0; i < this.nodes.count; i++) {

                    if (!this.nodes.item(i).isVertex) {
                        this.nodes.item(i).isSelected = false;

                        var material = new three.MeshNormalMaterial();

                        this.nodes.item(i).setMaterial(material);

                    }

                }
            },

            showSelectedVertexGroups: function () {

                this.deselectAllVertexs();
                this.deselectAllLEDs();
                for (var i = 0; i < this.selectedGroupOptions.count; i++) {

                    var option = this.selectedGroupOptions.item(i);

                    for (var j = 0; j < option.list.count; j++) {
                        option.list.item(j).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });

                        option.list.item(j).setMaterial(selectionMaterial);
                    }

                }

            },

            addSingleLED: function (intersects) {
                var distance = intersects[0].object.geometry.boundingBox.min.distanceTo(intersects[0].object.geometry.boundingBox.max);

                var led = new LEDNode();
                led.x = intersects[0].point.x;
                led.y = intersects[0].point.y;
                led.z = intersects[0].point.z;
                led.radius = distance * .003;

                var ledSphere = led.createSphere();

                /*Adjust the sphere radius according to model scale*/

                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);

            },

            findSelectionType: function (event) {
                if (!this.addModeOn) {


                    this.orbitControl.enabled = !event.ctrlKey;
                    this.dragControls.enabled = event.ctrlKey;


                    var intersects = this.findIntersects(this.nodes, event);


                    if (intersects.length > 0) {

                        if (!intersects[0].object.isSelected) {

                            this.selectSphere(intersects);

                        }
                        else {

                            this.deseletSphere(intersects);

                        }

                    }
                }
                else {


                    var meshList = new ArrayList();
                    for (var i = 0; i < this.sceneMesh.count; i++) {
                        meshList.add(this.sceneMesh.item(i));
                    }
                    var intersects = this.findIntersects(meshList, event);
                    if (intersects.length > 0) {
                        this.addSingleLED(intersects);
                    }


                }

            },


            selectSphere: function (intersects) {


                var selectionMaterial = new three.MeshBasicMaterial({

                    color: 0xff0000
                });


                intersects[0].object.setMaterial(selectionMaterial);

                intersects[0].object.isSelected = true;

            },

            deseletSphere: function (intersects) {


                var selectionMaterial = new three.MeshNormalMaterial({


                });

                intersects[0].object.setMaterial(selectionMaterial);

                intersects[0].object.isSelected = false;


            },

            findIntersects: function (objects, event) {

                var mouse = { x: (event.layerX / domGeom.getMarginSize(this.domNode).w) * 2 - 1, y: -(event.layerY / domGeom.getMarginSize(this.domNode).h) * 2 + 1 };

                var vector = new three.Vector3(mouse.x, mouse.y, 1);

                this.projector.unprojectVector(vector, this.camera);

                var raycaster = new three.Raycaster(this.camera.position, vector.sub(this.camera.position).normalize());

                return raycaster.intersectObjects(objects.toArray());

            },

            setAddMode: function (button) {

                if (this.addModeOn) {

                    this.setAddModeOff(button);

                }
                else {

                    this.setAddModeOn(button);

                }
            },

            setAddModeOn: function (button) {

                this.addModeOn = true;
                button.set('label', "Add Single LED ON");

            },

            setAddModeOff: function (button) {

                this.addModeOn = false;
                button.set('label', "Add Single LED OFF");
            }




        });

    });
