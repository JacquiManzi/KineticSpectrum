/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*
*   AmbientLight.js - Ambient light used in Kinetic Spectrum ModelViews. 
*   Full THREEJS documention for ambient light here: http://threejs.org/docs/#Reference/Lights/AmbientLight
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.AmbientLight", [three.AmbientLight], {

        COLOR: 0x101030,

        constructor: function () {

            /*THREEJS ambient light properties*/
            this.color = this.COLOR;
        },

        /*Set ambient light color*/
        setColor: function (color) {
            this.color = color;
        }
    });

});