/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*    jacquimanzi@gmail.com
*
*   DragControls.js - Dragging functionality used in Kinetic Spectrum ModelViews. 
*   An example of DragControls in THREE.js can be found here: http://jsfiddle.net/S6Gq9/1/
*    
*   NOTE: I have modified THREE.js to include my own version of DragControls, full documentation here:
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.DragControls", [three.DragControls], {

      
        constructor: function (camera, nodes, domElement, domGeom) {


        }
    });

});