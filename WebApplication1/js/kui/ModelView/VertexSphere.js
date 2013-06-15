define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/dom-geometry"
],
    function (declare, html, dom, domStyle, domConstruct, three, domGeom) {
        "use strict";
        return declare("kui.ModelMenu.VertexSphere", null, {

            /*
             *   
             *
             */

            constructor: function (x, y, z) {


                this.radius = 0.02;
                this.segmentsWidth = 3; //max 8 min 3
                this.segmentsHeight = 2; //max 6 min 2
                this.projector = new three.Projector();

                this.x = x;
                this.y = y;
                this.z = z;


                this.sphere = this.createSphere();

            },


            createSphere: function () {

                var sphere = new three.Mesh(new three.SphereGeometry(this.radius, this.segmentsWidth, this.segmentsHeight), new three.MeshNormalMaterial());
                sphere.overdraw = true;

                sphere.position.x = this.x;
                sphere.position.y = this.y;
                sphere.position.z = this.z;

                sphere.isSelected = false;
                sphere.coords = new three.Vector3(this.x, this.y, this.z);

                return sphere;

            },

            findSelectionType: function (spheres, domElement, camera, event)
            {
                var intersects = this.findIntersects(spheres, domElement, camera, event);

                if (intersects.length > 0) {

                    if (!intersects[0].object.isSelected) {

                        this.selectSphere(intersects);

                    }
                    else {

                        this.deseletSphere(intersects);

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

            findIntersects: function (spheres, domElement, camera, event) {

                var mouse = { x: (event.layerX / domGeom.getMarginSize(domElement).w) * 2 - 1, y: -(event.layerY / domGeom.getMarginSize(domElement).h) * 2 + 1 };

                var vector = new three.Vector3(mouse.x, mouse.y, 1);

                this.projector.unprojectVector(vector, camera);

                var raycaster = new three.Raycaster(camera.position, vector.sub(camera.position).normalize());

                return raycaster.intersectObjects(spheres.toArray());

            }



        });

    });
