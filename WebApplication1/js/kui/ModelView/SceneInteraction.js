

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
     "dojo/_base/array"

],
    function (declare, ModelSkeleton, ArrayList, LEDNode, LightAddress, three, domGeom, VertexSphere, html, PatternModel, on, Group, array) {
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
                this.nodes = new ArrayList();
                this.vertexSpheres = new ArrayList();
                this.selectedNodes = new ArrayList();

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
                        this.nodes.add(vertexSphere.sphere);
                        this.vertexSpheres.add(vertexSphere.sphere);

                    }
                }

                this.dragControls = new three.DragControls(this.camera, this.nodes, this.domNode, domGeom);

                dojo.connect(this.domNode, "onmousemove", dojo.hitch(this, this.doSelect));
                dojo.connect(this.domNode, "onmousedown", dojo.hitch(this, this.findSelectionType));
                dojo.connect(this.domNode, "onmouseup", dojo.hitch(this, function (event) {

                    this.orbitControl.enabled = true;
                    this.dragControls.enabled = false;

                }));
                

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


            /*Find the line segments between selected VertexSpheres*/
            findConnectingLines: function (amount) {

                var lineSegments = new ArrayList();

                var selectedNodes = this.selectedNodes;

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

                    this.nodes.add(sphere);
                    this.scene.add(sphere);
                }


            },

            removeAllNodes: function()
            {
                this.selectAllLEDs();
                this.selectAllVertexs();
                var selectedNodes = this.selectedNodes;

                for (var i = 0; i < selectedNodes.count; i++) {

                        this.scene.remove(selectedNodes.item(i));
                        this.nodes.remove(selectedNodes.item(i));
        
                }
                this.selectedNodes.clear();
            },

            removeNodes: function () {

                var selectedNodes = this.getSelectedNodes();

                for (var i = 0; i < selectedNodes.count; i++) {

                    if (!selectedNodes.item(i).isVertex) {
                        this.scene.remove(selectedNodes.item(i));
                        this.nodes.remove(selectedNodes.item(i));
                    }
                }
                this.selectedNodes.clear();

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
                var selectedNodes = this.selectedNodes;

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
                        var node = thisObj.getLEDNode(lightAddress);
                        selectedNodes.add(node);
                    });
                    thisObj.nameToGroup[name] = new Group(name, selectedNodes);
                });
            },
            
            addSelectedGroup: function (listBox, groupName) {

                var selectedNodes = this.getSelectedNodes();

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
                this.deselectAllVertexs();
                this.deselectAllLEDs();
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
                this.deselecteSperes(group.selectedNodes);
                group.remove();
                delete this.nameToGroup[groupName];
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
                    var node = this.nodes.item(i);
                    if (node.isVertex) {
                        node.isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });
                        


                        this.nodes.item(i).setMaterial(selectionMaterial);
                    }
                }
            },

            selectAllLEDs: function () {

                for (var i = 0; i < this.nodes.count; i++) {
                    var node = this.nodes.item(i);

                    if (!node.isVertex) {
                        node.isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });
                        
                        this.selectedNodes.add(node);

                        node.setMaterial(selectionMaterial);
                    }
                }
            },

            deselectAllLEDs: function () {
                this.selectedNodes.clear();
                
                for (var i = 0; i < this.nodes.count; i++) {
                    var node = this.nodes.item(i);

                    if (!node.isVertex) {
                        node.isSelected = false;

                        var material = new three.MeshNormalMaterial();

                        node.setMaterial(material);
                    }

                }
            },

            showSelectedVertexGroups: function (selectedGroupOptions) {

                this.deselectAllVertexs();
                this.deselectAllLEDs();
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

            addSingleLED: function (intersects) {
                var distance = intersects[0].object.geometry.boundingBox.min.distanceTo(intersects[0].object.geometry.boundingBox.max);

                var led = new LEDNode();
                led.x = intersects[0].point.x;
                led.y = intersects[0].point.y;
                led.z = intersects[0].point.z;
                led.radius = distance * .003;
                led.address = new LightAddress();

                var ledSphere = led.createSphere();

                /*Adjust the sphere radius according to model scale*/

                this.nodes.add(ledSphere);
                this.scene.add(ledSphere);

            },
            
            getLEDNode: function(lightAddress) {
                return this.addressToLED[lightAddress.toString()];
            },

            doSelect: function (event) {
                if (event.altKey)
                {
                    var intersects = this.findIntersects(this.nodes, event);
                    if (intersects.length > 0 && this._inObject != intersects[0].object.id) {
                        this._inObject = intersects[0].object.id;
                        if (!intersects[0].object.isSelected) {
                            this.selectSphere(intersects[0].object);
                        }
                        else {
                            this.deseletSphere(intersects[0].object);
                        }
                    }
                }
            },


            mouseSelect: function(event)
            {
                var intersects = this.findIntersects(this.nodes, event);
                if (intersects.length > 0) {

                    if (!intersects[0].object.isSelected) {
                        this.selectSphere(intersects[0].object);
                    }
                    else {
                        this.deseletSphere(intersects[0].object);
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
                        this.addSingleLED(intersects);
                    }


                }

            },

            selecteSperes: function(nodes) {
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
                if (!node.isVertex) {
                    this.selectedNodes.add(node);
                }
            },

            deseletSphere: function (node) {

                node.unselect();
                this.selectedNodes.remove(node);
            },
            
            deselecteSperes: function(nodes) {
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
                for (var i = 0; i < this.nodes.count; i++) {

                    if (!this.nodes.item(i).isVertex) {
                        this.leds.add(this.nodes.item(i));
                    }

                }

            },
            
            applyColorState: function(lightStateList) {

                var addressToLED = this.addressToLED;
                array.forEach(lightStateList, function (state) {
                    var addressStr = state.address.toString();
                    if (!!addressToLED[addressStr]) {
                        addressToLED[state.address.toString()].setColor(state.color);
                    }
                });
            },

            getSelectedNodes: function () {

                var selectedNodes = new ArrayList();
                for (var i = 0; i < this.nodes.count; i++) {

                    if (this.nodes.item(i).isSelected) {

                        selectedNodes.add(this.nodes.item(i));

                    }

                }

                return selectedNodes;

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
