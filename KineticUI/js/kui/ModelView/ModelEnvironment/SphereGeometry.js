/*
*   @Author: Jacqui Manzi
*    September 11th, 2013
*    jacquimanzi@gmail.com
*
*   SphereGeometry.js - The default 3D SphereGeometry used in Kinetic Spectrum ModelViews.
*   Full THREEJS documention for SphereGeometry here: http://threejs.org/docs/#Reference/Extras.Geometries/SphereGeometry
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.SphereGeometry", [three.SphereGeometry], {

        RADIUS: 50,
        POS_X: 0,
        POS_Y: 0,
        POS_Z: 0,
        SEGMENT_WIDTH: 3,
        SEGMENT_HEIGHT: 2,

        constructor: function () {

            this.radius = this.RADIUS;
            this.segmentWidth = this.SEGMENT_WIDTH;
            this.segmentHeight = this.SEGMENT_HEIGHT;

            this.boundingSphere.center.x = this.POS_X;
            this.boundingSphere.center.y = this.POS_Y;
            this.boundingSphere.center.z = this.POS_Z;
            
        },

        setRadius: function (radius) {
            this.radius = radius;
        },

        setSegmentHeight: function (segmentHeight) {
            this.segmentHeight = segmentHeight;
        },

        setSegmentWidth: function (segmentWidth) {
            this.segmentWidth = segmentWidth;
        },

        setX: function (x) {
            this.boundingSphere.center.x = x;
        },

        setY: function (y) {
            this.boundingSphere.center.y = y;
        },

        setZ: function (z) {
            this.boundingSphere.center.z = z;
        }

    }); 

});