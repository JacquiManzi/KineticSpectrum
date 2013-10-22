/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*    jacquimanzi@gmail.com
*
*   Vector3.js - The default 3D vecotor used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for Vector3 here: http://threejs.org/docs/#Reference/Math/Vector3
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Vector3", [three.Vector3], {

        POS_X: 0,
        POS_Y: 0,
        POS_Z: 0,

        constructor: function () {
            
            this.x = this.POS_X;
            this.y = this.POS_Y;
            this.z = this.POS_Z;
        },

        setCoords: function(x, y, z){            
            this.set(x, y, z);
        },

        setXPos: function(x){
            this.setX(x);
        },

        setYPos: function(y){
            this.setY(y);
        },

        setZPos: function (z) {
            this.setZ(z);
        }

    });

});