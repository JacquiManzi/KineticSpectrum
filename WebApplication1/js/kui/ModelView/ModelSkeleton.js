define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/ModelView/VertexSphere",
    "dojox/collections/ArrayList",
    "kui/LEDMenu/LEDNode"
],
    function (declare, html, dom, domStyle, domConstruct, three, VertexSphere, ArrayList, LEDNode) {
        "use strict";
        return declare("kui.ModelMenu.ModelSkeleton", null, {

            /*
             *   
             *
             */

            constructor: function (object, domNode, scene, camera) {
              
                this.geometry = object;
                this.vertices = this.geometry.vertices;
                this.colors = this.geometry.colors;
                this.normals = this.geometry.normals;
                this.faces = this.geometry.faces;
                this.faceUvs = this.geometry.faceUvs;
                this.faceVertexUvs = this.geometry.faceVertexUvs;
                this.domNode = domNode;
                this.scene = scene;
                this.camera = camera;

                this.spheres = new ArrayList();

                this.selectedSpheres = new ArrayList();

            },

            colorEachVertex: function () {

               
                for (var i = 0; i < this.vertices.length; i++) {
                    
                    var vertexSphere = new VertexSphere(this.vertices[i].x, this.vertices[i].y, this.vertices[i].z); 

                    this.scene.add(vertexSphere.sphere);

                    this.spheres.add(vertexSphere.sphere);
                    
                }

                dojo.connect(this.domNode, "onmousedown", dojo.hitch(vertexSphere, vertexSphere.findSelectionType, this.spheres, this.domNode, this.camera));

              
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

                    this.scene.remove(this.selectedSpheres.item(i));
                    this.spheres.remove(this.selectedSpheres.item(i)); 


                }



            }


           


        });

    });
