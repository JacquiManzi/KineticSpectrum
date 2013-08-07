

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
     "kui/ModelView/LEDSet"

],
    function (declare, ModelSkeleton, ArrayList, LEDNode, LightAddress, three, domGeom, VertexSphere, html, PatternModel, on, Group, array, LEDSet) {
        "use strict";
        return declare("kui.ModelView.SceneInteraction", null, {

            /*
             *   
             *
             */

            constructor: function () {


                this.modelSkeleton = null;

                this.addressToLED = [];
                this.nameToGroup = [];
                this.selectedGroupNames = [];

                this.selectedGroupOptions = new ArrayList();
                this.groupOptionList = new ArrayList(); 

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
                this.ledSet = new LEDSet(this.scene);
                
            },

            getIdealLEDRadius: function()
            {
                var distance = this.modelSkeleton.geometryList.item(0).boundingBox.min.distanceTo(this.modelSkeleton.geometryList.item(0).boundingBox.max);
                var radius = distance * .01;

                return radius;

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

                var selectedNodes = ledSet.getSelectedNodes();

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

         
            drawNodes: function (lineSegments) {


                for (var i = 0; i < lineSegments.count; i++) {

                    var ledNode = new LEDNode();

                    ledNode.x = lineSegments.item(i).x;
                    ledNode.y = lineSegments.item(i).y;
                    ledNode.z = lineSegments.item(i).z;

                    var sphere = ledNode.createSphere();

                    this.ledSet.nodes.add(sphere);
                    this.scene.add(sphere);
                }


            },

            removeAllNodes: function()
            {
                this.ledSet.selectAllLEDs();
                this.ledSet.selectAllVertexs();
                var selectedNodes = this.ledSet.getSelectedNodes();

                for (var i = 0; i < selectedNodes.count; i++) {

                        this.scene.remove(selectedNodes.item(i));
                        this.ledSet.nodes.remove(selectedNodes.item(i));
        
                }
            },

            removeNodes: function () {

                var selectedNodes = this.ledSet.getSelectedNodes();

                for (var i = 0; i < selectedNodes.count; i++) {

                    if (!selectedNodes.item(i).isVertex) {
                        this.scene.remove(selectedNodes.item(i));
                        this.ledSet.nodes.remove(selectedNodes.item(i));
                    }
                }
            },
            
            generateGroupName: function() {
                var largest = 0;
                var pattern = /group\s(\d+)/;
                array.forEach(Object.keys(this.nameToGroup), function(groupName) {
                    var match = pattern.exec(groupName);
                    if (!!match) {
                        largest = Math.max(largest, match[1]);
                    }
                });
                return "group " + (largest + 1);
            },

            getGroups:function() {
                return Object.keys(this.nameToGroup);
            },
            
            deleteGroup: function(groupName) {
                delete this.nameToGroup[groupName];
            },

            createGroupFromSelected: function(groupName) {
                groupName = groupName ? groupName : this.generateGroupName();
                var selectedNodes = this.ledSet.getSelectedNodes();

                var group = new Group(groupName, selectedNodes);

                this.nameToGroup[groupName] = group;
                group.applyGroup();

                return groupName;
            },
            
            addGroups: function (serialGroups) {
                var thisObj = this;
                serialGroups.forEach(function (serialGroup) {
                    var selectedNodes = new ArrayList();
                    var name = serialGroup.name;
                    serialGroup.lights.forEach(function(lightAddress) {
                        var node = thisObj.ledSet.getLEDNode(lightAddress);
                        selectedNodes.add(node);
                    });
                    thisObj.nameToGroup[name] = new Group(name, selectedNodes);
                });
            },
            
            addSelectedGroup: function (listBox, groupName) {

                var selectedNodes = this.ledSet.getSelectedNodes();

                if (selectedNodes.count > 0) {

                    var countAmount = this.groupOptionList.count + 1;

                    var group = new Group(countAmount, selectedNodes, groupName);
                  
                    listBox.domNode.appendChild(group.groupOption);

                    this.groupOptionList.add(group.groupOption);

                    group.applyGroup();

                    on(group.groupOption, "click", dojo.hitch(this, function (listBox) {

                        this.selectedGroupOptions.clear();
                        for (var i = 0; i < listBox.getSelected().length; i++) {
                            this.selectedGroupOptions.add(listBox.getSelected()[i]);
                        }
                        this.showSelectedVertexGroups(this.selectedGroupOptions);


                    }, listBox));

                }

                this.patternModel.updateGroupDropDown();
            },
            
            selectGroups: function (groupNames) {
                this.ledSet.deselectAllVertexs();
                this.ledSet.deselectAllLEDs();
                array.forEach(groupNames, dojo.hitch(this, this.selectGroup));
            },
            
            selectGroup: function(groupName) {
                var group = this.nameToGroup[groupName];
                this.selectSpheres(group.selectedNodes);
            },
            
            deselectGroup: function(groupName) {
                var group = this.nameToGroup[groupName];
                this.deselectSpheres(group.selectedNodes);
            },
            
            removeGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                group.deselectAll();
                this.deselectSpheres(group.selectedNodes);
                group.remove();
                delete this.nameToGroup[groupName];
            },
         
            showSelectedVertexGroups: function (selectedGroupOptions) {

                this.ledSet.deselectAllVertexs();
                this.ledSet.deselectAllLEDs();
                for (var i = 0; i < selectedGroupOptions.count; i++) {

                    var option = selectedGroupOptions.item(i);

                    for (var j = 0; j < option.list.count; j++) {
                        option.list.item(j).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });   

                        option.list.item(j).setMaterial(selectionMaterial);
                    }

                }

            },

            doSelect: function (event) {
                if (event.altKey)
                {
                    var intersects = this.findIntersects(this.ledSet.nodes, event);
                    if (intersects.length > 0 && this._inObject != intersects[0].object.id) {
                        this._inObject = intersects[0].object.id;
                        if (!intersects[0].object.isSelected) {
                            this.selectSphere(intersects[0].object);
                        }
                        else {
                            this.deselectSphere(intersects[0].object);
                        }
                    }
                }
            },


            mouseSelect: function(event)
            {
                var intersects = this.findIntersects(this.ledSet.nodes, event);
                if (intersects.length > 0) {

                    if (!intersects[0].object.isSelected) {
                        this.selectSphere(intersects[0].object);
                    }
                    else {
                        this.deselectSphere(intersects[0].object);
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
                    for (var i = 0; i < this.sceneMesh.count; i++) {
                        meshList.add(this.sceneMesh.item(i));
                    }
                    var intersects = this.findIntersects(meshList, event);
                    if (intersects.length > 0) {

                        this.ledSet.addSingleLED(intersects);
                    }
                    
                    //@TODO: Jacqui- This does not work on loaded light configuration files since they do not have meshes,
                    //Make this work without meshes.
                }

            },

            selectSpheres: function(nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var thisObj = this;
                nodes.forEach(function(node) {
                    thisObj.selectSphere(node);
                });
            },
            
            selectSphere: function (node) {               
                node.select();
            },

            deselectSphere: function (node) {
                node.unselect();
            },
            
            deselectSpheres: function(nodes) {
                if (!(nodes instanceof ArrayList)) {
                    nodes = new ArrayList(nodes);
                }
                var thisObj = this;
                nodes.forEach(function(node) {
                    thisObj.deselectSphere(node);
                });
            },

            findIntersects: function (objects, event) {

                var mouse = { x: (event.layerX / domGeom.getMarginSize(this.domNode).w) * 2 - 1, y: -(event.layerY / domGeom.getMarginSize(this.domNode).h) * 2 + 1 };

                var vector = new three.Vector3(mouse.x, mouse.y, 1);

                this.projector.unprojectVector(vector, this.camera);

                var raycaster = new three.Raycaster(this.camera.position, vector.sub(this.camera.position).normalize());

                return raycaster.intersectObjects(objects.toArray());

            },
            
            /*GETTERS/SETTERS HERE*/


            getLEDs: function () {

                this.leds.clear();
                for (var i = 0; i < this.ledSet.nodes.count; i++) {

                    if (!this.ledSet.nodes.item(i).isVertex) {
                        this.leds.add(this.ledSet.nodes.item(i));
                    }

                }

            },
            
            getGroupOptions: function () {

                var groupOptions = new ArrayList();
                
                for (var i = 0; i < this.groupOptionList.count; i++) {

                    groupOptions.add(this.groupOptionList.item(i));
                }

                return groupOptions;
                
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
