/*
*   @Author: Jacqui Manzi
*    September 9th, 2013
*    jacquimanzi@gmail.com
*
*   Camera.js - The default camera used in Kinetic Spectrum ModelViews. This a THREEJS Perspective Camera.
*   Full THREEJS documention for perspective camera here: http://threejs.org/docs/#Reference/Cameras/PerspectiveCamera
*/

define([
    "dojo/_base/declare",
    "threejs/three"
], function (declare, three) {
    return declare("kui.ModelView.ModelEnvironment.Camera", [three.PerspectiveCamera], {

        FOV: 45,          //Field of view
        ASPECT: 0,        //Aspect ratio
        NEAR: 0.09,       //Near frustrum plane
        FAR: 10000,       //Far frustrum plane
        CAMERA_POS_X: 2,  //Camera X position
        CAMERA_POS_Y: 2,  //Camera Y position
        CAMERA_POS_Z: 2,  //Camera Z position

        constructor: function () {

            /*THREEJS Perspective Camera properties*/
            this.fov = this.FOV;
            this.near = this.NEAR;
            this.far = this.FAR;
            this.aspect = this.ASPECT;

            /*THREEJS Perspective Camera x,y,z position*/
            this.position.x = this.CAMERA_POS_X;
            this.position.y = this.CAMERA_POS_y;
            this.position.z = this.CAMERA_POS_Z;
        },

        /*Set and update the camera aspect ratio*/
        setAspect: function (aspect) {

            this.aspect = aspect;
            this.updateProjectionMatrix();
        },

        /*Set and update the camera field of view*/
        setFOV: function (fov) {

            this.fov = fov;
            this.updateProjectionMatrix();
        },
        
        /*Set and update the camera frustum near plane*/
        setNear: function (near) {

            this.near = near;
            this.updateProjectionMatrix();
        },

        /*Set and update the camera frustum far plane*/
        setFar: function (far) {

            this.far = far;
            this.updateProjectionMatrix();
        }

    
        
    });

});