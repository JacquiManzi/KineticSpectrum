/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*
*   Scene.js - THREEJS Scene used in Kinetic Spectrum ModelViews. 
*   Full THREEJS documention for Scene here: http://threejs.org/docs/#Reference/Scenes/Scene
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Scene", [three.Scene], {


        constructor: function () {

        },

        removeScene: function () {

        },

        removeFromScene: function (obj){

            this.remove(obj);
        },

        addToScene: function (obj) {

            this.add(obj);
        }


    });

});