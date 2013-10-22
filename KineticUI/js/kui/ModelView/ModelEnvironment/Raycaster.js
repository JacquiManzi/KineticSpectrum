/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*    jacquimanzi@gmail.com
*
*   Raycaster.js - The default 3D raycaster used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for Raycaster here: http://threejs.org/docs/#Reference/Core/Raycaster
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Raycaster", [three.Raycaster], {
      
        constructor: function () {
        },

        setOriginAndDirection: function (origin, direction) {
            this.set(origin, direction);
        },

        setNear: function (near) {
            this.near = near;
        },

        setFar: function (far) {
            this.far = far;
        }

    });

});