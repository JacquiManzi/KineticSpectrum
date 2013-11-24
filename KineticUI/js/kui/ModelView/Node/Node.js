/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacquimanzi@gmail.com
*
*   Node.js - The general class describing interactive nodes such as a Vertex object and an LED object in the Model.
*   Node extends from Mesh.js, which is the container class for THREE.js Mesh objects.
*/

define([
    "dojo/_base/declare",
    "kui/ModelView/ModelEnvironment/Vector3",
    "kui/ModelView/ModelEnvironment/Mesh",
    "kui/ModelView/ModelEnvironment/MeshBasicMaterial",
    "kui/ModelView/ModelEnvironment/MeshNormalMaterial",
    "kui/ModelView/ModelEnvironment/Color"
],
    function (declare, Vector3, Mesh, MeshBasicMaterial, MeshNormalMaterial, Color) {
        "use strict";
        return declare("kui.ModelMenu.Node.Node", Mesh, {

            segmentWidth: 3,
            segmentHeight: 2,

            constructor: function () {
              
                this.setMaterial(new MeshNormalMaterial());
                
                this.isVertex = true;       
                this.addModeOn = true;
                this.isSelected = false;
                this.address = "";

                this._createMeshSphere(0, 0, 0); 
            },

            _createMeshSphere: function( x, y, z){

                this.setX(x);
                this.setY(y);
                this.setZ(z);

                var vector = new Vector3();
                vector.setCoords(x, y, z);

                this.coords = vector;
                this.updateGeometryFaces();  
            },

            setColor: function (color) {
                if (color == 0x000000) {
                    this.unselect();
                    return;
                }
                if (!this.material.color) {
                    var material = new MeshBasicMaterial();
                    material.color.setHex(color);
                    this.setMaterial(material);
                } else if(this.material.color.getHex() !== color) {
                    this.material.color.setHex(color);
                }
            },

            select: function () {
                this.setColor(0xff0000);
                this.isSelected = true;
            },

            unselect: function () {
                if (this.material.color) {
                    var material = new MeshNormalMaterial();
                    this.setMaterial(material);
                    this.isSelected = false;
                    this.hasColor = false;
                }
            },
           
            setRadius: function (radius) {
                this.radius = radius;
                this.setSphereGeometry(radius, this.segmentWidth, this.segmentHeight);
            },

            setSegmentWidth: function (segmentWidth) {
                this.segmentWidth = segmentWidth;
                this.setSphereGeometry(this.radius, segmentWidth, this.segmentHeight);
            },

            setSegmentHeight: function (segmentHeight) {
                this.segmentHeight = segmentHeight;
                this.setSphereGeometry(this.radius, this.segmentWidth, segmentHeight);
            },
         
            setCoords: function (x, y, z) {
                this.setX(x);
                this.setY(y);
                this.setZ(z);
            }            
        });

    });
