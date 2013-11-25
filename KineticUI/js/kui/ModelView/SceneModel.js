/*
*@Author: Jacqui Manzi
*September 11th, 2013
*jacqui@revkitt.com
*/

define([
    "dojo/_base/declare",
    "kui/Simulation/Simulation",
    "dojox/collections/ArrayList"
],
    function (declare, Simulation, ArrayList) {
        return declare("kui.ModelView.SceneModel", null, {

            constructor: function (scene, sceneInteraction, modelView) {

                this.scene = scene;
                this.sceneInteraction = sceneInteraction;
                this.groupModel = this.sceneInteraction.groupModel;
                this.modeView = modelView;
                this.allLEDsSelected = false;
                this.allVerticesSelected = false;

                this.simulation = new Simulation();
            },

            getScene: function(){

                return this.scene;
            },

            getNodeModel: function(){
                return this.sceneInteraction.nodeModel;
            },

            loadFile: function(file, scene){

                this.modelView.loadFile(file, scene);
            },

            loadServerLEDs: function(){
                
                this.modelView.loadServerLEDs();
            },


            addNodes: function (nodes) {

                var thisObj = this;
                nodes.forEach(function (node) {
                    thisObj.scene.addToScene(node);
                });
            },

            addSingleNode: function(node){
                this.scene.addToScene(node);
            },

            /*Remove the three.js object speheres from the Scene*/
            createLEDForSegments: function(lineSegments, initialAddress) {
                this.sceneInteraction.createLEDForSegments(lineSegments, initialAddress);
            },

            removeNodes: function (nodes) {

                var thisObj = this;
                nodes.forEach(function (node) {
                    thisObj.scene.removeFromScene(node);
                });

                //Also remove the nodes from the NodeModel Set and from server.
                this.getNodeModel().removeNodesFromSet(nodes);
            },

            removeNode: function(node){

                this.scene.removeFromScene(node);
            },

            tryPattern: function () {


            },

            setMode: function () {


            },

            lockPatternMode: function () {

            },

            createVertices: function (vertexList) {

            },

            setSelectedGroups: function (/*Array<String>*/groupNames) {
                this.groupModel.selectGroups(groupNames);
            },

            getGroupModel: function(){

                return this.groupModel;
            },

            getGroupFromName: function(/*String*/groupName){

               var group = this.groupModel.getGr
                
               return group;
            },

            createGroupFromSelected: function(/*String*/groupName) {
                this.groupModel.createGroupFromSelected(groupName);
            },

            deleteSelectedGroups: function() {
                this.groupModel.deleteSelectedGroups();
            },

            deselectGroup: function(/*String*/groupName){
                this.groupModel.deselectGroup(groupName);
            },

            selectGroup: function(/*String*/groupName) {
                this.groupModel.selectGroup(groupName);
            },

            selectGroups: function(/*Array<String>*/ groupNames) {
                this.groupModel.selectGroups(groupNames);
            },

            /**
             * Retrieves an array of the group names in the scene
             * Array<String>
             */
            getGroups: function(){
                return this.groupModel.getGroups();
            },

            /**
              * Retrieves an array of the names of the selected groups in the scene
              * Array<String>
              */
            getSelectedGroupNames: function() {
                return this.groupModel.getSelectedGroupNames();
            },

            /*Retrieves the selected nodes stored in the Node model*/
            /*Returns ArrayList<String>*/
            getSelectedNodes: function(){

                var selectedNodes = this.getNodeModel().getSelectedNodes();
                return selectedNodes;
            },

            /**
             * Registers a function to be called whenever the group membership changes. The provided function will be called
             * whenever a group is added or removed with the full list of group names
             */
            addGroupsUpdatedListener: function (groupsUpdated /*void groupChanged(Array<String> groupNames, Array<String> selectedGroupNames)*/) {
                this.groupModel.addGroupsUpdatedListener(groupsUpdated);
            },
        });

    });
