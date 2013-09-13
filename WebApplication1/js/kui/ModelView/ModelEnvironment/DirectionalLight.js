/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*
*   DirectionalLight.js - Directional light used in Kinetic Spectrum ModelViews. 
*   Full THREEJS documention for directional light here: http://threejs.org/docs/#Reference/Lights/DirectionalLight
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.DirectionalLight", [three.DirectionalLight], {

        COLOR: 0xffeedd,
        INTENSITY: 1.0,
        POS_X: 1,
        POS_Y: 0,
        POS_Z: 1,

        constructor: function () {

            /*THREEJS directional light properties*/
            this.color = this.COLOR;
            this.intensity = this.INTENSITY;

            /*THREEJS directional light x,y,z position*/
            this.position.set(this.POS_X, this.POS_Y, this.POS_Z);
        },

        /*Set directional light color*/
        setColor: function (color) {

            this.color = color;          
        },

        /*Set directional light position of x, y, and z*/
        setPosition: function (x, y, z) {

            this.position.set(x, y, z).normalize();
        }
           
    });

});