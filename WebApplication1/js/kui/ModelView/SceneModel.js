/*
*@Author: Jacqui Manzi
*September 11th, 2013
*jacquimanzi@gmail.com

*/

define([
    "dojo/_base/declare",
    "kui/Simulation/Simulation"
],
    function (declare, Simulation) {
        "use strict";
        return declare("kui.ModelView.SceneModel", null, {
            /*
             */
           
            constructor: function (scene, sceneInteraction, modelView) {

                this.scene = scene;
                this.sceneInteraction = sceneInteraction;
                this.groupSet = this.sceneInteraction.groupSet;
                this.modeView = modelView;

                this.simulation = new Simulation();

            },

            getScene: function(){

                return this.scene;
            },

            loadFile: function(file, scene){

                this.modelView.loadFile(file, scene);
            },

            loadServerLEDs: function(){
                
                this.modelView.loadServerLEDs();
            },

            getSelectedgroups: function(){

            },

            setSelectedGroups: function () {

            },

            addLeds: function(){

            },

            tryPattern: function () {


            },

            setMode: function () {


            },

            lockPatternMode: function () {

            },

            createVertices: function (vertexList) {

            },

            getGroupFromName: function(groupName){

               var group = this.groupSet.nameToGroup[groupName];
                
               return group;
            },

            deselectGroup: function(groupName){
                this.groupSet.deselectGroup(groupName);
            },

            selectGroup: function(groupName){
                this.groupSet.selectGroup(groupName)
            },

            getGroups: function(){

                return this.groupSet.getGroups();
            },

            /**
             * Registers a function to be called whenever the group membership changes. The provided function will be called
             * whenever a group is added or removed with the full list of group names
             */
            addGroupsUpdatedListener: function (groupsUpdated /*void groupChanged(Array<String> groupNames)*/) {
                this.groupSet.addGroupsUpdatedListener(groupsUpdated);
            },
        });

    });
