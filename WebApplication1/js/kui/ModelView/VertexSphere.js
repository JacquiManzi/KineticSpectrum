define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three"
],
    function (declare, html, dom, domStyle, domConstruct, three) {
        "use strict";
        return declare("kui.ModelMenu.VertexSphere", null, {

            /*
             *   
             *
             */

            constructor: function (radius, x, y, z) {


                this.radius = radius
                this.segmentsWidth = 3; //max 8 min 3
                this.segmentsHeight = 2; //max 6 min 2
               

                this.x = x;
                this.y = y;
                this.z = z;

                this.isVertex = true;
                this.sphere = this.createSphere();

                this.addModeOn = true;
            },


            createSphere: function () {

                var sphere = new three.Mesh(new three.SphereGeometry(this.radius, this.segmentsWidth, this.segmentsHeight), new three.MeshNormalMaterial());
                sphere.overdraw = true;

                sphere.position.x = this.x;
                sphere.position.y = this.y;
                sphere.position.z = this.z;

                sphere.isSelected = false;
                sphere.isVertex = this.isVertex;
                sphere.coords = new three.Vector3(this.x, this.y, this.z);
                sphere.geometry.computeFaceNormals();

                return sphere;

            }


        });

    });
