define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojo/dom-geometry",
    "kui/ModelView/ModelSkeleton",
    "dojox/collections/ArrayList"

],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom, ModelSkeleton, ArrayList) {
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

                this.meshes = new ArrayList();



            },
                   
            displayModel: function (fileLocation) {

               
                var paneHeight = domGeom.getMarginSize(this.getParent().domNode).h;
                var paneWidth = domGeom.getMarginSize(this.getParent().domNode).w;
                
                this.aspect = (paneWidth / paneHeight);

                this.scene = new three.Scene();
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

                    this.shineDirectionalLight(this.scene, this.shineDirectionalLightColor, this.directionalLightIntensity, this.directionalLightDistance, 
                        positions);
                }

                if (this.hasAmbientLight)
                {
                    this.shineAmbientLight(this.scene, this.ambientColor);
                }


                this.render = dojo.hitch(this, function () {

                    requestAnimationFrame(this.render);

                    this.camera.lookAt(this.scene.position);
                    renderer.render(this.scene, this.camera);
                });

                var modelViewNode = this.domNode;
                this.orbitControl = new three.OrbitControls(this.camera, modelViewNode);

                this.load(fileLocation, this.scene, this.render);

                var animate = dojo.hitch(this, function () {

                    requestAnimationFrame(animate);
                    this.orbitControl.update();
                });

                animate(); 

            },

            load: function(fileLocation, scene, render)
            {
                var loader = new three.OBJLoader();
               
                this.loadObj = dojo.hitch(this,function () { loader.load(fileLocation, dojo.hitch(this, this.loadObject, scene, render)); });
                this.loadObj();
            },

            loadFile: function(data, scene, render)
            {
                var loader = new three.OBJLoader();
                var object = loader.parse(data);

                this.loadObj = dojo.hitch(this, this.loadObject, scene, render, object);
                this.loadObj();

            },

            loadObject: function(scene, render, object)
            {
               
                if (this.meshes.count > 0) {

                    this.removeAllMeshes();
                }
            
                this.modelSkeleton = new ModelSkeleton(this.domNode, scene, this.camera, this.orbitControl);
                for (var i = 0; i < object.children.length; i++) {

                 
                    three.GeometryUtils.center(object.children[i].geometry);

                    this.modelSkeleton.geometryList.add(object.children[i].geometry);
                   
                    var mesh = new three.Mesh(object.children[i].geometry, new three.MeshBasicMaterial(
                        {
                            color: 0x999999,
                            wireframe: true,
                            transparent: false,
                            opacity: 0.85,
                            vertexColors: true
                        }));

                    this.meshes.add(mesh);
                                  
                    scene.add(mesh);
                }
                this.modelSkeleton.sceneMesh = this.meshes;
                this.modelSkeleton.colorEachVertex();

                render();
            },

            removeAllMeshes: function()
            {
                for (var i = 0; i < this.meshes.count; i++) {
                    this.scene.remove(this.meshes.item(i));
                }

                for (var i = 0; i < this.modelSkeleton.spheres.count; i++) {

                    this.scene.remove(this.modelSkeleton.spheres.item(i));

                }

                this.meshes.clear();
                this.modelSkeleton.spheres.clear();
            },

            resetObject: function()
            {
                this.removeAllMeshes();
                this.showHideButton.hidden = false;
                this.showHideButton.set('label', "Hide Vertices");

                this.loadObj();
            },

            showHideVertices: function(button)
            {
                this.showHideButton = button;
                if (button.hidden) {

                    this.showAllVertices();
                    button.hidden = false;
                    button.set('label', "Hide Vertices");
                }
                else {

                    this.hideAllVertices();
                    button.hidden = true;
                    button.set('label', "Show Vertices");
                }
            },

            hideAllVertices: function()
            {
                for (var i = 0; i < this.modelSkeleton.spheres.count; i++) {

                    if (this.modelSkeleton.spheres.item(i).isVertex) {
                        this.modelSkeleton.spheres.item(i).visible = false;
                    }

                }
            },

            showAllVertices: function()
            {
                for (var i = 0; i < this.modelSkeleton.spheres.count; i++) {

                    if (this.modelSkeleton.spheres.item(i).isVertex) {
                        this.modelSkeleton.spheres.item(i).visible = true;
                    }

                }
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

