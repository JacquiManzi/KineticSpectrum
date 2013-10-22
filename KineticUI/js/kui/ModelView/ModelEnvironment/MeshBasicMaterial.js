/*
*   @Author: Jacqui Manzi
*    September 11th, 2013
*    jacquimanzi@gmail.com
*
*   MeshBasicMaterial.js - The default 3D MeshBasicMaterial used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for MeshBasicMaterial here: http://threejs.org/docs/#Reference/Materials/MeshBasicMaterial
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.MeshBasicMaterial", [three.MeshBasicMaterial], {

        COLOR: 0x000000,

        constructor: function () {
            this.color = this.COLOR;
        },

        setColor: function (color) {
            this.color = color;
        }

    });

});