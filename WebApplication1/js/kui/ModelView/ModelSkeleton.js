define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/ModelView/VertexSphere",
    "dojox/collections/ArrayList",
    "kui/LEDMenu/LEDNode",
    "dojo/on",
    "dojo/dom-geometry"
],
    function (declare, html, dom, domStyle, domConstruct, three, VertexSphere, ArrayList, LEDNode, on, domGeom) {
        "use strict";
        return declare("kui.ModelMenu.ModelSkeleton", null, {

            /*
             *   
             *
             */

            constructor: function (domNode, scene, camera, orbitControl) {
              
                this.geometryList = new ArrayList();
                this.colors = null;
                this.normals = null;
                this.faces = null;
                this.faceUvs = null;
                this.faceVertexUvs = null;
                this.domNode = domNode;
                this.scene = scene;
                this.camera = camera;
                this.projector = new three.Projector();

                this.spheres = new ArrayList(); //Vertex spheres and LED spheres currently drawn
                this.leds = new ArrayList(); //VertexSpheres that are LED nodes

                this.selectedSpheres = new ArrayList(); //Selected vertex speheres and led nodes
                this.selectedVertexGroups = new ArrayList(); //Selected Vetices that are grouped
                this.selectedGroupVertexOptions = new ArrayList(); //The vertex group options that are selected in the list box

                this.addModeOn = false;

                this.sceneMesh = null;
                this.orbitControl = orbitControl;
            },

            colorEachVertex: function () {

                for (var i = 0; i < this.geometryList.count; i++) {

                    var vertices = this.geometryList.item(i).vertices;
                    for (var j = 0; j < vertices.length; j++) {

                        var distance = this.geometryList.item(i).boundingBox.min.distanceTo(this.geometryList.item(i).boundingBox.max);
                        var radius = distance * .01;
                        var vertexSphere = new VertexSphere( radius, vertices[j].x, vertices[j].y, vertices[j].z);

                        /*Adjust the sphere radius according to model scale*/
                        
                        vertexSphere.radius = distance * 0.003;

                        this.scene.add(vertexSphere.sphere);

                        this.spheres.add(vertexSphere.sphere);

                    }
                }

               
                dojo.connect(this.domNode, "onmousedown", dojo.hitch(this, this.findSelectionType));
                dojo.connect(this.domNode, "onmouseup", dojo.hitch(this, function (event) {

                    this.orbitControl.enabled = true;
                    this.dragControls.enabled = false;

                }));
                this.dragControls = new three.DragControls(this.camera, this.spheres, this.domNode, domGeom);

            },


            /*Find the line segments between selected VertexSpheres*/
            findConnectingLines: function (amount) {

                var lineSegments = new ArrayList();

                this.getSelectedSpheres();
                var spheres = this.selectedSpheres;

                while (spheres.count > 1) {

                    var sphereOne = spheres.item(0).coords;
                    var sphereTwo = spheres.item(1).coords;

                    var deltaX = (sphereOne.x - sphereTwo.x) / (amount + 1);
                    var deltaY = (sphereOne.y - sphereTwo.y) / (amount + 1);
                    var deltaZ = (sphereOne.z - sphereTwo.z) / (amount + 1);

                    for(var i = 1; i <= amount; i++)
                    {
                        var x = sphereTwo.x + i * deltaX;
                        var y = sphereTwo.y + i * deltaY;
                        var z = sphereTwo.z + i * deltaZ;

                        lineSegments.add(new three.Vector3(x, y, z));
                    }

                    spheres.remove(spheres.item(0));

                }

                this.x = this.selectedSpheres.item(0).position.x;
                this.y = this.selectedSpheres.item(0).position.y;
                this.z = this.selectedSpheres.item(0).position.z;
             
                return lineSegments;

            },

            getSelectedSpheres: function () {

                this.selectedSpheres.clear();
                for (var i = 0; i < this.spheres.count; i++) {

                    if (this.spheres.item(i).isSelected) {

                        this.selectedSpheres.add(this.spheres.item(i));

                    }
              

                }

            },

            getSelectedVertices: function () {

                var selectedVertices = new ArrayList();

                for (var i = 0; i < this.spheres.count; i++) {

                    if (this.spheres.item(i).isSelected) {

                        selectedVertices.add(this.spheres.item(i));

                    }

                }

                return selectedVertices;
            },

            drawNodes: function (lineSegments) {


                for (var i = 0; i < lineSegments.count; i++) {

                    var ledNode = new LEDNode();

                    ledNode.x = lineSegments.item(i).x;
                    ledNode.y = lineSegments.item(i).y;
                    ledNode.z = lineSegments.item(i).z;

                    var sphere = ledNode.createSphere();

                    this.spheres.add(sphere);
                    this.scene.add(sphere);
                }


            },

            removeNodes: function () {

                this.getSelectedSpheres();

                for (var i = 0; i < this.selectedSpheres.count; i++) {

                    if (!this.selectedSpheres.item(i).isVertex) {
                        this.scene.remove(this.selectedSpheres.item(i));
                        this.spheres.remove(this.selectedSpheres.item(i));
                    }


                }



            },

            getLEDs: function () {

                this.leds.clear();
                for (var i = 0; i < this.spheres.count; i++) {

                    if(!this.spheres.item(i).isVertex)
                    {
                        this.leds.add(this.spheres.item(i));
                    }

                }

            },

            addSelectedGroup: function (listBox) {

               
                var vertices = this.getSelectedVertices();
                
                if (vertices.count > 0 && !this.selectedVertexGroups.contains(vertices)) {

                    var countAmount = this.selectedVertexGroups.count + 1;
                    this.selectedVertexGroups.add(vertices);
                    
                    var option = html.createOption("Group" + " " + countAmount);
                    option.list = vertices;
                    listBox.domNode.appendChild(option);

                    on(option, "click", dojo.hitch(this, function (listBox) {

                        this.selectedGroupVertexOptions.clear();
                        for (var i = 0; i < listBox.getSelected().length; i++) {
                            this.selectedGroupVertexOptions.add(listBox.getSelected()[i]);
                        }
                        this.showSelectedVertexGroups();


                    }, listBox));

                }


            },

            removeSelectedGroup: function (listBox) {

                var selectedOptions = listBox.getSelected();

                for (var i = 0; i < selectedOptions.length; i++) {

                    listBox.domNode.removeChild(selectedOptions[i]);
                    this.selectedGroupVertexOptions.remove(selectedOptions[i]); 
                }
            },

            deselectAllVertexs: function()
            {
               
                for(var i = 0; i < this.spheres.count; i++)
                {

                    if(this.spheres.item(i).isVertex)
                    {
                        this.spheres.item(i).isSelected = false;

                        var material = new three.MeshNormalMaterial();

                        this.spheres.item(i).setMaterial(material);

                    }

                }

            },

            selectAllVertexs: function()
            {
                for (var i = 0; i < this.spheres.count; i++) {

                    if (this.spheres.item(i).isVertex) {
                        this.spheres.item(i).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });


                        this.spheres.item(i).setMaterial(selectionMaterial);
                    }


                }
            },

            selectAllLEDs: function()
            {

                for (var i = 0; i < this.spheres.count; i++) {

                    if (!this.spheres.item(i).isVertex) {
                        this.spheres.item(i).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });


                        this.spheres.item(i).setMaterial(selectionMaterial);
                    }

                }
            },

            deselectAllLEDs: function()
            {
                for (var i = 0; i < this.spheres.count; i++) {

                    if (!this.spheres.item(i).isVertex) {
                        this.spheres.item(i).isSelected = false;

                        var material = new three.MeshNormalMaterial();

                        this.spheres.item(i).setMaterial(material);

                    }

                }
            },

            showSelectedVertexGroups: function()
            {

                this.deselectAllVertexs();
                this.deselectAllLEDs();
                for(var i = 0; i < this.selectedGroupVertexOptions.count; i++)
                {

                    var option = this.selectedGroupVertexOptions.item(i);

                    for(var j = 0; j < option.list.count; j++)
                    {
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

                this.spheres.add(ledSphere);
                this.scene.add(ledSphere);

            },

            findSelectionType: function (event) {
                if (!this.addModeOn) {


                        this.orbitControl.enabled = !event.ctrlKey;
                        this.dragControls.enabled = event.ctrlKey;
     

                    var intersects = this.findIntersects(this.spheres, event);


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

            setAddMode: function(button)
            {

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
