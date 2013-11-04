
/*
* @author: Jacqui Manzi
* August 6th, 2013
* jacquimanzi@gmail.com
* SceneInteraction - All node interaction with the scene happens here.
*/
define([
    "dojo/_base/declare",
    "dojox/collections/ArrayList",
    "kui/ModelView/Node/LED",
    "kui/ModelView/Node/LightAddress",
    "dojo/dom-geometry",
    "kui/ModelView/Node/Node",
    "kui/util/CommonHTML",
    "dojo/on",
    "kui/ModelView/Node/NodeModel",
    "kui/ModelView/groups/GroupModel",
    "kui/ModelView/ModelEnvironment/DragControls",
    "kui/ModelView/ModelEnvironment/OrbitalControl",
    "kui/ModelView/ModelEnvironment/Vector3",
    "kui/ModelView/ModelEnvironment/Raycaster",
    "kui/ModelView/ModelEnvironment/Projector"
],
    function (declare, ArrayList, LED, LightAddress, domGeom, Node, html,
        on, NodeModel, GroupModel, DragControls, OrbitalControl, Vector3, Raycaster, Projector) {
        "use strict";
        return declare("kui.ModelView.SceneInteraction", null, {

            constructor: function (domNode, camera, scene) {
                              
                this.camera = camera; 
                this.domNode = domNode;
                this.scene = scene;

                this.projector = new Projector();
                this.meshList = new ArrayList();
                 
                this.nodeModel = new NodeModel(this.scene);
                this.groupModel = new GroupModel(this.nodeModel);

                this.orbitControl = new OrbitalControl(this.camera, this.domNode);  
                this.dragControls = new DragControls(this.camera, this.nodeModel.nodes, this.domNode, domGeom);
                this.addModeOn = false;

                this._initiateMouseEvents();
                
                this.lightNo = 0;
                this.portNO = 0;
                this.fixtureNo = 0;

                
            },

            updateMeshes: function(meshList){
                this.meshList.clear();
                this.meshList = meshList;
            },

            updateOrbitControl: function(){
                this.orbitControl.update();
            },

            updateDragControl: function(){
                this.dragControls = new DragControls(this.camera, this.nodeModel.nodes, this.domNode, domGeom);
            },

            _initiateMouseEvents: function(){

                dojo.connect(this.domNode, "onmousemove", dojo.hitch(this, this.doSelect));
                dojo.connect(this.domNode, "onmousedown", dojo.hitch(this, this.findSelectionType));

                dojo.connect(this.domNode, "onmouseup", dojo.hitch(this, function (event) {

                    this.orbitControl.enabled = true;
                    this.dragControls.enabled = false;
                }));
            },

            /*Find the line segments between selected VertexSpheres*/
            findConnectingLines: function (amount) {

                var lineSegments = new ArrayList();
                var selectedNodes = this.nodeModel.getSelectedNodes();

                while (selectedNodes.count > 1) {

                    var sphereOne = selectedNodes.item(0).coords;
                    var sphereTwo = selectedNodes.item(1).coords;

                    var deltaX = (sphereOne.x - sphereTwo.x) / (amount + 1);
                    var deltaY = (sphereOne.y - sphereTwo.y) / (amount + 1);
                    var deltaZ = (sphereOne.z - sphereTwo.z) / (amount + 1);

                    for (var i = 1; i <= amount; i++) {
                        var x = sphereOne.x - i * deltaX;
                        var y = sphereOne.y - i * deltaY;
                        var z = sphereOne.z - i * deltaZ;

                        var vector = new Vector3();
                        vector.setCoords(x, y, z);
                        lineSegments.add(vector);
                    }

                    selectedNodes.remove(selectedNodes.item(0));
                }

                this.x = selectedNodes.item(0).position.x; //What am I saving x, y, and z for?
                this.y = selectedNodes.item(0).position.y;
                this.z = selectedNodes.item(0).position.z;

                return lineSegments;
            },

         /*Draws an idividual LED node on the indicated (user clicked on) line segment*/
            drawNodes: function (lineSegments, startAddress) {

                var thisObj = this;
                lineSegments.forEach(function (segment) {

                    //var lightAddress = new LightAddress();
                    startAddress.lightNo = thisObj.lightNo;
                    startAddress.fixtureNo = thisObj.fixtureNo;
                    startAddress.portNo = thisObj.portNo;

                    thisObj.nodeModel.addGeneratedLED(segment, new LightAddress(startAddress));
                    startAddress.inc();
                });
            },

            /*Removes all nodes from the scene- LEDs and Vertices*/
            removeAllNodes: function()
            {
                this.nodeModel.selectAllLEDs();
                this.nodeModel.selectAllVertexs();
                var selectedNodes = this.nodeModel.getSelectedNodes();

                var thisObj = this;
                selectedNodes.forEach(function(node) {
                    thisObj.scene.remove(node);
                    thisObj.nodeModel.nodes.remove(node);
                });
            },

            /*Removes all selected LED nodes- not vertices*/
            removeLEDNodes: function () {

                var selectedNodes = this.nodeModel.getSelectedNodes();

                var thisObj = this;
                selectedNodes.forEach(function (node) {
                    if (!node.isVertex) {
                        thisObj.scene.remove(node);
                        thisObj.nodeModel.nodes.remove(node);
                    }
                });
            },

            doSelect: function (event) {
                if (event.altKey)
                {
                    var intersects = this.findIntersects(this.nodeModel.nodes, event);
                    if (intersects.length > 0 && this._inObject != intersects[0].object.id) {
                        this._inObject = intersects[0].object.id;
                        if (!intersects[0].object.isSelected) {
                            this.nodeModel.selectNode(intersects[0].object);
                        }
                        else {
                            this.nodeModel.deselectNode(intersects[0].object);
                        }
                    }
                }
            },


            mouseSelect: function(event)
            {
                var intersects = this.findIntersects(this.nodeModel.nodes, event);
                if (intersects.length > 0) {

                    if (!intersects[0].object.isSelected) {
                        this.nodeModel.selectNode(intersects[0].object);
                    }
                    else {
                        this.nodeModel.deselectNode(intersects[0].object);
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
                    this.meshList.forEach(function (mesh) {
                        meshList.add(mesh);
                    });

                    var intersects = this.findIntersects(meshList, event);

                    if (intersects.length > 0) {
                        this.nodeModel.addSingleLED(intersects);
                    }
                    
                    //@TODO: Jacqui- This does not work on loaded light configuration files since they do not have meshes,
                    //Make this work without meshes.
                }

            },

          

            findIntersects: function (objects, event) {

                var mouse = {
                    x: ((event.layerX - domGeom.getContentBox(this.domNode).l) / domGeom.getContentBox(this.domNode).w) * 2 - 1,
                    y: -((event.layerY - domGeom.getContentBox(this.domNode).l) / domGeom.getContentBox(this.domNode).h) * 2 + 1
                };

                var vector = new Vector3();
                vector.setCoords(mouse.x, mouse.y, 1);

                this.projector.unproject(vector, this.camera);

                var raycaster = new Raycaster();
                raycaster.setOriginAndDirection(this.camera.position, vector.sub(this.camera.position).normalize());

                return raycaster.intersectObjects(objects.toArray());
            },
            
            /*GETTERS/SETTERS HERE*/

            setAddMode: function (button, getAddress) {
                this.getAddressFunc = getAddress;
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
