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


                this.radius = 0.005;
                this.segmentsWidth = 3;
                this.segmentsHeight = 2;
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

                return sphere;

            },

            selectSphere: function (spheres, domElement, camera, event) {



                var mouse = { x: (event.layerX / domGeom.getMarginSize(domElement).w) * 2 - 1, y: -(event.layerY / domGeom.getMarginSize(domElement).h) * 2 + 1 };
                //var mouse = { x: (event.x / window.innerWidth) * 2 - 1, y: -(event.y / window.innerHeight) * 2 + 1 };
                var vector = new three.Vector3( mouse.x, mouse.y, 1);

                this.projector.unprojectVector( vector, camera );

                var raycaster = new three.Raycaster(camera.position, vector.sub(camera.position).normalize());

                var intersects = raycaster.intersectObjects(spheres);

                if (intersects.length > 0 ) {

                    var selectionMaterial = new three.MeshBasicMaterial({

                        color: 0xff0000
                    });


                    intersects[0].object.setMaterial(selectionMaterial);
                }

            },

            deseletSphere: function () {




            }




        });

    });
