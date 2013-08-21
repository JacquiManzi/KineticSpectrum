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
    "dojox/collections/ArrayList",
    "kui/ModelView/SceneInteraction",
    "kui/ajax/FileInterface"

],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom, ModelSkeleton, ArrayList, SceneInteraction, FileInterface) {
        "use strict";
        return declare("kui.ModelView.ModelView", ContentPane, {

            /*
             *   3D Model View area (is a ContentPane object)
             *
             */

            constructor: function (obj1, obj2) {

                this.style = "overflow:hidden"; 
                /*Camera properties*/
                this.fov = 45;
                this.aspect = 0;
                this.near = 0.09;
                this.far = 10000;
                this.cameriaPositionX = 2;
                this.cameriaPositionY = 2;
                this.cameriaPositionZ = 2;

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
                
                this.sceneInteraction = new SceneInteraction();
                dojo.connect(obj1.simulation, "onStateChange", dojo.hitch(this.sceneInteraction.ledSet,
                    this.sceneInteraction.ledSet.applyColorState));

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
            
            loadServerLEDs:function() {
                var fileInterface = new FileInterface();
                fileInterface.getLightConfigList(dojo.hitch(this, function (lightList) {
                    if (lightList.count > 0) {
                        this.sceneInteraction.removeAllNodes();
                        this.removeAllMeshes();
                        this.sceneInteraction.ledSet.createLEDNodes(lightList);
                        this.loadServerGroups();
                    }
                }));
            },
            
            loadServerGroups: function() {
                var fileInterface = new FileInterface();
                fileInterface.getGroups(dojo.hitch(this.sceneInteraction.groupSet, this.sceneInteraction.groupSet.addGroups));
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
            
            
                this.modelSkeleton = new ModelSkeleton();
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

                /*Set scene interaction properties once they have been established*/
                this.sceneInteraction.domNode = this.domNode;
                this.sceneInteraction.camera = this.camera;
                this.sceneInteraction.scene = this.scene;
                this.sceneInteraction.orbitControl = this.orbitControl;
                this.sceneInteraction.modelSkeleton = this.modelSkeleton;
                this.sceneInteraction.sceneMesh = this.meshes;
                this.sceneInteraction.createVertexSpheres();

                this.loadServerLEDs();

                render();

                
            },

            removeAllMeshes: function()
            {
                for (var i = 0; i < this.meshes.count; i++) {
                    this.scene.remove(this.meshes.item(i));
                }

                for (var i = 0; i <this.sceneInteraction.ledSet.nodes.count; i++) {
                    this.scene.remove(this.sceneInteraction.ledSet.nodes.item(i));
                }

               this.meshes.clear();
               this.sceneInteraction.ledSet.nodes.clear();
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
                for (var i = 0; i < this.sceneInteraction.ledSet.nodes.count; i++) {

                    if (this.sceneInteraction.ledSet.nodes.item(i).isVertex) {
                       this.sceneInteraction.ledSet.nodes.item(i).visible = false;
                    }

                }
            },

            showAllVertices: function()
            {
                for (var i = 0; i < this.sceneInteraction.ledSet.nodes.count; i++) {

                    if (this.sceneInteraction.ledSet.nodes.item(i).isVertex) {
                       this.sceneInteraction.ledSet.nodes.item(i).visible = true;
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

