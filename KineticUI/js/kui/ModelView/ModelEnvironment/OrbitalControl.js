/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*
*   Renderer.js - THREEJS OrbitalControl used in Kinetic Spectrum ModelViews. 
*   THREEJS orbit control example here: http://threejs.org/examples/misc_controls_orbit.html
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.OrbitalControl", [three.OrbitControls], {


        constructor: function (camera, domNode) {

            this.camera = camera;
            this.domElement = domNode;
        }

    });

}); 