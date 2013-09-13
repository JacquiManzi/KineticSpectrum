/*
*   @Author: Jacqui Manzi
*    September 11th, 2013
*    jacquimanzi@gmail.com
*
*   Mesh.js - The default 3D Mesh used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for Mesh here: http://threejs.org/docs/#Reference/Objects/Mesh
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Mesh", [three.Mesh], {

        POS_X: 0,
        POS_Y: 0,
        POS_Z: 0,
        

        constructor: function () {
            this.geometry = new three.Geometry();
            this.material = new three.Material();

            this.position.x = this.POS_X;
            this.position.y = this.POS_Y;
            this.position.z = this.POS_Z;

            this.coords = new three.Vector3(this.position.x,
                this.position.y, this.position.z);
        },

        setSphereGeometry: function (radius, segmentWidth, segmentHeight) {
            this.setGeometry(new three.SphereGeometry(radius, segmentWidth, segmentHeight));
            
        },

        setMaterial: function (material) {
            this.material = material;
        },

        setX: function (x) {
            this.coords.x = x;
            this.position.x = x;
        },

        setY: function (y) {
            this.coords.y = y;
            this.position.y = y;
        },

        setZ: function (z) {
            this.coords.z = z;
            this.position.z = z;
        }




    });

});