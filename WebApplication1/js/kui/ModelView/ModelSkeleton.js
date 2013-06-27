define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/on",
    "dojo/dom-geometry"
],
    function (declare, html, dom, domStyle, domConstruct, three, ArrayList, on, domGeom) {
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
                
            }
        });

    });
