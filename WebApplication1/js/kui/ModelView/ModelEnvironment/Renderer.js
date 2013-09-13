/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*
*   Renderer.js - THREEJS WebGLRenderer used in Kinetic Spectrum ModelViews. 
*   Full THREEJS documention for WebGLRenderer here: http://threejs.org/docs/#Reference/Renderers/WebGLRenderer
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Renderer", [three.WebGLRenderer], {


        constructor: function () {

        },

        setRenderSize: function (width, height) {

            this.setSize(width, height);
        }
    

    });

});