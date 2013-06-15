define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/dom-geometry",
    "kui/ModelView/ModelSkeleton"

],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom, ModelSkeleton) {
        "use strict";
        return declare("kui.ModelView.ModelView", ContentPane, {

            /*
             *   3D Model View area (is a ContentPane object)
             *
             */

            constructor: function (obj, obj2) {

                this.style = "overflow:hidden"; 
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

               
                var paneHeight = domGeom.getMarginSize(this.getParent().domNode).h;
                var paneWidth = domGeom.getMarginSize(this.getParent().domNode).w;
                
                this.aspect = (paneWidth / paneHeight);


                var scene = new three.Scene();
                this.camera = new three.PerspectiveCamera(this.fov, this.aspect, this.near, this.far);
                

                this.camera.position.x = this.cameriaPositionX;
                this.camera.position.y = this.cameriaPositionY;
                this.camera.position.z = this.cameriaPositionZ;

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

                    this.camera.lookAt(scene.position);
                    renderer.render(scene, this.camera);
                });

                var modelViewNode = this.domNode;
                var orbitControl = new three.OrbitControls(this.camera, modelViewNode);

                loader.load(fileLocation, dojo.hitch(this, this.loadObject, scene, render));

                var animate = function () {

                    requestAnimationFrame(animate);
                    orbitControl.update();                    
                }

                animate();

            },

            loadObject: function(scene, render, object)
            {
                var geometry = object.children[0].geometry;
                three.GeometryUtils.center(geometry);

                this.modelSkeleton = new ModelSkeleton(geometry, this.domNode, scene, this.camera);
                this.modelSkeleton.colorEachVertex();

                var mesh = new three.Mesh(this.modelSkeleton.geometry, new three.MeshBasicMaterial(
                    {
                        color: 0x999999,
                        wireframe: true,
                        transparent: false,
                        opacity: 0.85,
                        vertexColors: true
                    }));


                scene.add(mesh);

                render();
            },


            setCameraPosition: function(x, y, z)
            {
                this.camera.position.x = x;
                this.camera.position.y = y;
                this.camera.position.z = z;

            },


            shineDirectionalLight: function (scene, color, intensity, distance, positions) 
            {

               this.directionalLight = new three.DirectionalLight(color);
                this.directionalLight.position.set(positions.x, positions.y, positions.z).normalize();
                scene.add(this.directionalLight);

            },

            shineAmbientLight: function (scene, color) 
            {
                this.ambient = new three.AmbientLight(color);
                scene.add(this.ambient);

            }
             

        });

    });

