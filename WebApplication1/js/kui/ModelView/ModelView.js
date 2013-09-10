/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*/

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
    "kui/ModelView/Axis",
    "kui/ajax/FileInterface",
    "kui/ModelView/ModelEnvironment/Camera",
    "kui/ModelView/ModelEnvironment/DirectionalLight"

],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom,
        ModelSkeleton, ArrayList, SceneInteraction, Axis, FileInterface, Camera, DirectionalLight) {
        "use strict";
        return declare("kui.ModelView.ModelView", ContentPane, {

            /*
             *   3D Model View area (is a ContentPane object)
             *
             */

            
            constructor: function () {

                this.style = "overflow:hidden"; 

                this.camera = new Camera();
                this.directionalLight = new DirectionalLight();

                /*Lighting Properties*/
                this.hasDirectionalLight = true;   
                this.hasAmbientLight = true;

                /*Ambient Color properties*/
                this.ambientColor = 0x101030;

                this.meshes = new ArrayList();
                
                this.sceneInteraction = new SceneInteraction();
                dojo.connect(this.simulation, "onStateChange", dojo.hitch(this.sceneInteraction.ledSet,
                    this.sceneInteraction.ledSet.applyColorState));

            },
                   
            displayModel: function (fileLocation) {

               
                var paneHeight = domGeom.getMarginSize(this.getParent().domNode).h;
                var paneWidth = domGeom.getMarginSize(this.getParent().domNode).w;
                
                /*Use the content pane demensions to determine the aspect ratio of the camera*/
                this.camera.setAspect(paneWidth / paneHeight);

                this.scene = new three.Scene();

                var renderer = new three.WebGLRenderer();
                renderer.setSize(paneWidth, paneHeight);
                this.set('content', renderer.domElement);
               

                if (this.hasDirectionalLight)
                {
                    var positions = 
                        {   
                            'x':  this.directionalLight.position.x, 
                            'y':  this.directionalLight.position.y, 
                            'z':  this.directionalLight.position.z
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

                this.axis = new Axis(this.scene);

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
                        //this.removeAllMeshes();
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
               this.directionalLight.setColor(color);
               this.directionalLight.setPosition(positions.x, positions.y, positions.z);

               scene.add(this.directionalLight);   
            },

            shineAmbientLight: function (scene, color) 
            {
                this.ambient = new three.AmbientLight(color);
                scene.add(this.ambient);

            }
             

        });

    });

