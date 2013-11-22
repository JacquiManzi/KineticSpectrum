﻿/*
*@Author: Jacqui Manzi
*September 11th, 2013
*jacquimanzi@gmail.com

*/

define([
    "dojo/_base/declare",
    "kui/Simulation/Simulation",
    "dojox/collections/ArrayList"
],
    function (declare, Simulation, ArrayList) {
        "use strict";
        return declare("kui.ModelView.SceneModel", null, {
            /*
             */
           
            constructor: function (scene, sceneInteraction, modelView) {

                this.scene = scene;
                this.sceneInteraction = sceneInteraction;
                this.groupModel = this.sceneInteraction.groupModel;
                this.modeView = modelView;

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

            removeNodes: function (nodes) {

                var thisObj = this;
                nodes.forEach(function (node) {
                    thisObj.scene.removeFromScene(node);
                }); 
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

            getGroupFromName: function(groupName){

               var group = this.groupModel.nameToGroup[groupNamde];
                
               return group;
            },

            createGroupFromSelected: function(groupName) {
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
