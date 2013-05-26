define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/dom-geometry"

],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom) {
        "use strict";
        return declare("kui.ModelView.ModelView", ContentPane, {

            /*
             *   3D Model View area (is a ContentPane object)
             *
             */

            constructor: function (obj, obj2) {


                /*Camera properties*/
                this.fov = 45;
                this.aspect = 0;
                this.near = 0.09;
                this.far = 1000;
                this.cameriaPositionX = 3;
                this.cameriaPositionY = 3;
                this.cameriaPositionZ = 3;

                /*Lighting Properties*/
                this.hasDirectionalLight = true;
                this.hasAmbientLight = true;

                /*Directional Light properties*/
                this.directionalLightColor = 0xffeedd;
                this.directionalLightIntensity = 0;
                this.directionalLightDistance = 0;

                //directional light position properties
                this.directionalLightX = 1;
                this.directionalLightY = 0;
                this.directionalLightZ = 1;

                /*Ambient Color properties*/
                this.ambientColor = 0x101030;


            },
                   
            displayModel: function (fileLocation) {


                var paneHeight = domGeom.getMarginSize(this.getParent().domNode).h - 20;
                var paneWidth = domGeom.getMarginSize(this.getParent().domNode).w - 20;
                
                this.aspect = (paneWidth / paneHeight);


                var scene = new three.Scene();
                var camera = new three.PerspectiveCamera(this.fov, this.aspect, this.near, this.far);
                camera.position.x = this.cameriaPositionX;
                camera.position.y = this.cameriaPositionY;
                camera.position.z = this.cameriaPositionZ;

                var renderer = new three.WebGLRenderer();
                renderer.setSize(paneWidth, paneHeight);
                this.set('content', renderer.domElement);
               

                if (this.hasDirectionalLight)
                {
                    var positions = 
                        {
                            'x':  this.directionalLightX, 
                            'y':  this.directionalLightY, 
                            'z': this.directionalLightX
                        };

                    this.shineDirectionalLight(scene, this.shineDirectionalLightColor, this.directionalLightIntensity, this.directionalLightDistance, 
                        positions);
                }

                if (this.hasAmbientLight)
                {
                    this.shineAmbientLight(scene, this.ambientColor)
                }


                var loader = new three.OBJLoader();


                var render = dojo.hitch(this, function () {

                    requestAnimationFrame(render);

                    camera.lookAt(scene.position);
                    renderer.render(scene, camera);
                });


                loader.load(fileLocation, function (object) {
                    scene.add(object);

                    render();
                });


            },


            shineDirectionalLight: function (scene, color, intensity, distance, positions) 
            {

                var directionalLight = new three.DirectionalLight(color);
                directionalLight.position.set(positions.x, positions.y, positions.z).normalize();
                scene.add(directionalLight);

            },

            shineAmbientLight: function (scene, color) 
            {
                var ambient = new three.AmbientLight(color);
                scene.add(ambient);

            }



         




        });

    });

