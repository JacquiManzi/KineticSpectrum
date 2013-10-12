define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/on",
    "dojo/dom-geometry",
    "kui/ModelView/Node/Vertex"
],
    function (declare, html, dom, domStyle, domConstruct, three, ArrayList, on, domGeom, Vertex) {
        "use strict";
        return declare("kui.ModelMenu.ModelSkeleton", null, {

            /*
             *   
             *
             */

            constructor: function (scene, nodeModel) {
              
                this.geometryList = new ArrayList();
                this.scene = scene;
                this.nodeModel = nodeModel;
            },

            createVertexSpheres: function () {

                for (var i = 0; i < this.geometryList.count; i++) {

                    var vertices = this.geometryList.item(i).vertices;
                    for (var j = 0; j < vertices.length; j++) {

                        var distance = this.geometryList.item(i).boundingBox.min.distanceTo(this.geometryList.item(i).boundingBox.max);

                        var vertex = new Vertex();
                        vertex.setCoords(vertices[j].x, vertices[j].y, vertices[j].z);

                        /*Adjust the sphere radius according to model scale*/
                        vertex.setRadius(distance * 0.007);

                        this.scene.add(vertex);
                        this.nodeModel.nodes.add(vertex);
                    }
                }
            }
        });

    });
