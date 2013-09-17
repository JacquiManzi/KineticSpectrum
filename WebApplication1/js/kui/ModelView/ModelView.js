/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacquimanzi@gmail.com
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
    "kui/ModelView/ModelEnvironment/DirectionalLight",
    "kui/ModelView/ModelEnvironment/AmbientLight",
    "kui/ModelView/ModelEnvironment/Scene",
    "kui/ModelView/ModelEnvironment/Renderer",
    "kui/ModelView/SceneModel"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, domGeom,
        ModelSkeleton, ArrayList, SceneInteraction, Axis, FileInterface, Camera, DirectionalLight,
        AmbientLight, Scene, Renderer, SceneModel) {
        ///"use strict";
        return declare("kui.ModelView.ModelView", ContentPane, {

            /*
             *   3D Model View area (is a ContentPane object)
             *
             */
           
            constructor: function () {

                this.style = "overflow:hidden"; 

                this.camera = new Camera();
                this.directionalLight = new DirectionalLight();
                this.ambientLight = new AmbientLight();
                this.scene = new Scene();
                this.renderer = new Renderer();

                /*Lighting Properties*/
                this.hasDirectionalLight = true;   
                this.hasAmbientLight = true;

                this.meshes = new ArrayList();
            },

            buildRendering: function(){

                var domNode = this.inherited(arguments);
                this.set('content', this.renderer.domElement);

                this.sceneInteraction = new SceneInteraction(this.renderer.domElement, this.camera, this.scene);
                this.sceneModel = new SceneModel(this.scene, this.sceneInteraction, this);

                dojo.connect(this.simulation, "onStateChange", dojo.hitch(this.sceneInteraction.nodeModel,
                this.sceneInteraction.nodeModel.applyColorState));

                return domNode;
            },
                   
            displayModel: function (fileLocation) {
               
                var paneHeight = domGeom.getMarginSize(this.getParent().domNode).h;
                var paneWidth = domGeom.getMarginSize(this.getParent().domNode).w;
                 
                /*Use the content pane demensions to determine the aspect ratio of the camera*/
                this.camera.setAspect(paneWidth / paneHeight);              
                this.renderer.setRenderSize(paneWidth, paneHeight);

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
                    this.shineAmbientLight(this.scene);
                }


                this.render = dojo.hitch(this, function () {

                    requestAnimationFrame(this.render);

                    this.camera.lookAt(this.scene.position);
                    this.renderer.render(this.scene, this.camera);
                });

                this.axis = new Axis(this.scene);
                this.load(fileLocation, this.scene);

                var animate = dojo.hitch(this, function () {

                    requestAnimationFrame(animate);
                    this.sceneInteraction.updateOrbitControl();
                });

                animate(); 

            },
            
            loadServerLEDs:function() {
                var fileInterface = new FileInterface();
                fileInterface.getLightConfigList(dojo.hitch(this, function (lightList) {
                    if (lightList.count > 0) {
                        this.sceneInteraction.removeAllNodes();
                        //this.removeAllMeshes();
                        this.sceneInteraction.nodeModel.createLEDNodes(lightList);
                        this.loadServerGroups();
                    }
                }));
            },
            
            loadServerGroups: function() {
                var fileInterface = new FileInterface();
                fileInterface.getGroups(dojo.hitch(this.sceneInteraction.groupSet, this.sceneInteraction.groupSet.addGroups));
            },

            load: function(fileLocation, scene)
            {
                var loader = new three.OBJLoader();
               
                this.loadObj = dojo.hitch(this,function () { loader.load(fileLocation, dojo.hitch(this, this.loadObject, scene)); });
                this.loadObj();
            },

            loadFile: function(data, scene)
            {
                var loader = new three.OBJLoader();
                var object = loader.parse(data);

                this.loadObj = dojo.hitch(this, this.loadObject, scene, object);
                this.loadObj();

            },

            loadObject: function(scene, object)
            {
               
                if (this.meshes.count > 0) {

                    this.removeAllMeshes();
                } 
                       
                var modelSkeleton = new ModelSkeleton(this.scene, this.sceneInteraction.nodeModel);
                for (var i = 0; i < object.children.length; i++) { 

                    three.GeometryUtils.center(object.children[i].geometry);
                    modelSkeleton.geometryList.add(object.children[i].geometry);
                   
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

                this.sceneInteraction.updateDragControl();
                this.sceneInteraction.updateMeshes(this.meshes);
                modelSkeleton.createVertexSpheres(this.scene); 

                this.loadServerLEDs();

                this.render();
            },

            removeAllMeshes: function()
            {
                for (var i = 0; i < this.meshes.count; i++) {
                    this.scene.remove(this.meshes.item(i));
                }

                for (var i = 0; i <this.sceneInteraction.nodeModel.nodes.count; i++) {
                    this.scene.remove(this.sceneInteraction.nodeModel.nodes.item(i));
                }

               this.meshes.clear();
               this.sceneInteraction.nodeModel.nodes.clear();
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
                for (var i = 0; i < this.sceneInteraction.nodeModel.nodes.count; i++) {

                    if (this.sceneInteraction.nodeModel.nodes.item(i).isVertex) {
                       this.sceneInteraction.nodeModel.nodes.item(i).visible = false;
                    }
                }
            },

            showAllVertices: function()
            {
                for (var i = 0; i < this.sceneInteraction.nodeModel.nodes.count; i++) {

                    if (this.sceneInteraction.nodeModel.nodes.item(i).isVertex) {
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

            shineAmbientLight: function (scene) {
             scene.add(this.ambientLight);
            }
             

        });

    });

