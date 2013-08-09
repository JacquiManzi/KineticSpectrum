
/*
* @author: Jacqui Manzi
* August 6th, 2013
* SceneInteraction - All node interaction with the scene happens here.
*/
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
     "kui/ModelView/LEDSet",
     "kui/ModelView/groups/GroupSet"

],
    function (declare, ModelSkeleton, ArrayList, LEDNode, LightAddress, three, domGeom, VertexSphere, html,
        PatternModel, on, LEDSet, GroupSet) {
        "use strict";
        return declare("kui.ModelView.SceneInteraction", null, {

            /*
             *   
             *
             */

            constructor: function () {

                this.modelSkeleton = null;

                this.addressToLED = [];
                
                this.projector = new three.Projector();
                this.camera = null;
                this.domNode = null;
                this.orbitControl = null;
                this.dragControls = null;
                this.scene = null;

                this.addModeOn = false;
                this.sceneMesh = null;

                this.fileSelectionType = null;

                /*Models*/
                this.patternModel = new PatternModel(this);
                this.ledSet = new LEDSet(this.scene);
                this.groupSet = new GroupSet(this.ledSet, this.patternModel);
                
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
                        this.ledSet.nodes.add(vertexSphere.sphere);
                        this.ledSet.vertexSpheres.add(vertexSphere.sphere);

                    }
                }

                this.dragControls = new three.DragControls(this.camera, this.ledSet.nodes, this.domNode, domGeom);

                dojo.connect(this.domNode, "onmousemove", dojo.hitch(this, this.doSelect));
                dojo.connect(this.domNode, "onmousedown", dojo.hitch(this, this.findSelectionType));
                dojo.connect(this.domNode, "onmouseup", dojo.hitch(this, function (event) {

                    this.orbitControl.enabled = true;
                    this.dragControls.enabled = false;

                }));

                //Set LEDSet scene here since we now have a scene to draw on.
                this.ledSet.scene = this.scene;
            },

            /*Find the line segments between selected VertexSpheres*/
            findConnectingLines: function (amount) {

                var lineSegments = new ArrayList();
                var selectedNodes = this.ledSet.getSelectedNodes();

                while (selectedNodes.count > 1) {

                    var sphereOne = selectedNodes.item(0).coords;
                    var sphereTwo = selectedNodes.item(1).coords;

                    var deltaX = (sphereOne.x - sphereTwo.x) / (amount + 1);
                    var deltaY = (sphereOne.y - sphereTwo.y) / (amount + 1);
                    var deltaZ = (sphereOne.z - sphereTwo.z) / (amount + 1);

                    for (var i = 1; i <= amount; i++) {
                        var x = sphereTwo.x + i * deltaX;
                        var y = sphereTwo.y + i * deltaY;
                        var z = sphereTwo.z + i * deltaZ;

                        lineSegments.add(new three.Vector3(x, y, z));
                    }

                    selectedNodes.remove(selectedNodes.item(0));
                }

                this.x = selectedNodes.item(0).position.x;
                this.y = selectedNodes.item(0).position.y;
                this.z = selectedNodes.item(0).position.z;

                return lineSegments;
            },

         /*Draws an idividual LED node on the indicated (user clicked on) line segment*/
            drawNodes: function (lineSegments) {

                var thisObj = this;
                lineSegments.forEach(function (segment) {
                   
                    var ledNode = new LEDNode();

                    ledNode.x = segment.x;
                    ledNode.y = segment.y;
                    ledNode.z = segment.z;

                    var sphere = ledNode.createSphere();

                    thisObj.ledSet.nodes.add(sphere);
                    thisObj.scene.add(sphere);
                });
            },

            /*Removes all nodes from the scene- LEDs and Vertices*/
            removeAllNodes: function()
            {
                this.ledSet.selectAllLEDs();
                this.ledSet.selectAllVertexs();
                var selectedNodes = this.ledSet.getSelectedNodes();

                var thisObj = this;
                selectedNodes.forEach(function(node) {
                    thisObj.scene.remove(node);
                    thisObj.ledSet.nodes.remove(node);
                });
            },

            /*Removes all selected LED nodes- not vertices*/
            removeLEDNodes: function () {

                var selectedNodes = this.ledSet.getSelectedNodes();

                var thisObj = this;
                selectedNodes.forEach(function (node) {
                    if (!node.isVertex) {
                        thisObj.scene.remove(node);
                        thisObj.ledSet.nodes.remove(node);
                    }
                });
            },

            doSelect: function (event) {
                if (event.altKey)
                {
                    var intersects = this.findIntersects(this.ledSet.nodes, event);
                    if (intersects.length > 0 && this._inObject != intersects[0].object.id) {
                        this._inObject = intersects[0].object.id;
                        if (!intersects[0].object.isSelected) {
                            this.ledSet.selectNode(intersects[0].object);
                        }
                        else {
                            this.ledSet.deselectNode(intersects[0].object);
                        }
                    }
                }
            },


            mouseSelect: function(event)
            {
                var intersects = this.findIntersects(this.ledSet.nodes, event);
                if (intersects.length > 0) {

                    if (!intersects[0].object.isSelected) {
                        this.ledSet.selectNode(intersects[0].object);
                    }
                    else {
                        this.ledSet.deselectNode(intersects[0].object);
                    }
                }
            },

            findSelectionType: function (event) {
                if (!this.addModeOn) {

                    this.orbitControl.enabled = !event.ctrlKey;
                    this.dragControls.enabled = event.ctrlKey;
                    this.mouseSelect(event);
                }
                else {

                    var meshList = new ArrayList();
                    this.sceneMesh.forEach(function (mesh) {
                        meshList.add(mesh);
                    });

                    var intersects = this.findIntersects(meshList, event);

                    if (intersects.length > 0) {
                        this.ledSet.addSingleLED(intersects);
                    }
                    
                    //@TODO: Jacqui- This does not work on loaded light configuration files since they do not have meshes,
                    //Make this work without meshes.
                }

            },

          

            findIntersects: function (objects, event) {

                var mouse = { x: (event.layerX / domGeom.getMarginSize(this.domNode).w) * 2 - 1, y: -(event.layerY / domGeom.getMarginSize(this.domNode).h) * 2 + 1 };

                var vector = new three.Vector3(mouse.x, mouse.y, 1);

                this.projector.unprojectVector(vector, this.camera);

                var raycaster = new three.Raycaster(this.camera.position, vector.sub(this.camera.position).normalize());

                return raycaster.intersectObjects(objects.toArray());

            },
            
            /*GETTERS/SETTERS HERE*/

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
