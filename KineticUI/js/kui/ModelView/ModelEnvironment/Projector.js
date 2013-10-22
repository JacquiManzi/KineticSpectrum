/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*    jacquimanzi@gmail.com
*
*   Projector.js - The default 3D projector used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for Projector here: http://threejs.org/docs/#Reference/Core/Projector
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Projector", [three.Projector], {

        constructor: function () {
        },

        project: function (vector, camera) {

            this.projectVector(vector, camera);
        },

        unproject: function (vector, camera) {

            this.unprojectVector(vector, camera);
        }

        

    });

});