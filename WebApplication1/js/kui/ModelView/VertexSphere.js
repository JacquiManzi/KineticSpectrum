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


                this.radius = radius;
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
                sphere.address = this.address;

                sphere.setColor = function (color) {

                    if (!this.material.color) {
                        var material = new three.MeshBasicMaterial({
                            color: color
                        });
                        this.setMaterial(material);
                    } else if(this.material.color.getHex() !== color) {
                        this.material.color.set(color);
                    }
                };

                sphere.select = function() {
                    this.setColor(0xff0000);
                    this.isSelected = true;
                };

                sphere.unselect = function() {
                    var material = new three.MeshNormalMaterial();
                    this.setMaterial(material);
                    this.isSelected = false;
                    this.hasColor = false;
                };

                return sphere;

            }


        });

    });
