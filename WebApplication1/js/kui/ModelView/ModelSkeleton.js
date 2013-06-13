define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/ModelView/VertexSphere"
],
    function (declare, html, dom, domStyle, domConstruct, three, VertexSphere) {
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

                this.spheres = [];
                this.vertexSpheres = [];

            },

            colorEachVertex: function () {

               
                for (var i = 0; i < this.vertices.length; i++) {
                    
                    var vertexSphere = new VertexSphere(this.vertices[i].x, this.vertices[i].y, this.vertices[i].z); 

                    this.scene.add(vertexSphere.sphere);

                    this.spheres.push(vertexSphere.sphere);
                    
                }

                dojo.connect(this.domNode, "onmousedown", dojo.hitch(vertexSphere, vertexSphere.findSelectionType, this.spheres, this.domNode, this.camera));

              
            }

           


        });

    });
