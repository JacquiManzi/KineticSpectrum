/*
*   @Author: Jacqui Manzi
*    September 11th, 2013
*    jacquimanzi@gmail.com
*
*   MeshNormalMaterial.js - The default 3D MeshNormalMaterial used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for MeshNotmalMaterial here: http://threejs.org/docs/#Reference/Materials/MeshNormalMaterial
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.MeshNormalMaterial", [three.MeshNormalMaterial], {

        constructor: function () {

        }

    });

});