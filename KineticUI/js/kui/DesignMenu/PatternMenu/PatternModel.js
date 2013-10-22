
/*
*   @author: Jacqui Manzi
*   August 7th, 2013
*
*/
define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dijit/MenuItem",
    "dojo/_base/array",
    "kui/ajax/Scenes",
    "kui/ajax/SimState"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList, MenuItem, array, Scenes, SimState) {
        "use strict";
        return declare("kui.DesignMenu.PatternMenu.PatternModel", null, {

            constructor: function (sceneModel) {

                this.groupList = new ArrayList();
                this.patternList = new ArrayList();
                this.patternsUpdatedListeners = new ArrayList();
                this.patternMenuElementListeners = new ArrayList();

                this.patternDef = {};
                this.effectsDef = {}; 
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;

                this._patternMenuElements = {
                    groupDropDown: null,
                    groupListBox: null,
                    nameField: null,
                    priorityDropDown: null,
                    patternDropDown: null
                };


                this.sceneModel = sceneModel;
                this.simulation = null;

                /*Add pattern menu element listeners*/
                this.addPatternMenuElementListener(dojo.hitch(this, this._updatePatternDropDown));
                this.addPatternMenuElementListener(dojo.hitch(this,this._updateGroupDropDown));

                this.updatePatternList();
            },

            /*
            *  Pattern Menu Elements Setters
            */
            setNameField: function(nameField){
                this._patternMenuElements.nameField = nameField;
            },

            setGroupDropDown: function(groupDropDown){
                this._patternMenuElements.groupDropDown = groupDropDown;
            },

            setGroupListBox: function(groupListBox){
                this._patternMenuElements.groupListBox = groupListBox;
            },

            setPriorityDropDown: function (priorityDropDown) {
                this._patternMenuElements.priorityDropDown = priorityDropDown;
            },

            setPatternDropDown: function(patternDropDown){
                this._patternMenuElements.patternDropDown = patternDropDown;
            },

            /*
            *  Pattern Menu Elements Getters
            */
           
            getGroupDropDownLabel: function () {
                return this._patternMenuElements.groupDropDown.get('label');
            },

            getGrouplistBoxSelection: function(){
                return this._patternMenuElements.groupListBox.getSelected();
            },

            getPatternNameValue: function () {
                return this._patternMenuElements.nameField.get('displayedValue');
            },

            getPatternPriorityValue: function () {
                return this._patternMenuElements.get('value');
            },

            /*Pattern Model Getters*/

            getSelectedGroups: function () {
                var selected = [];
                array.forEach(this._patternMenuElements.groupListBox.getSelected(), function (group) {
                    selected.push(group);
                });
                return selected;
            },

            getGroupsFromSceneModel: function(){
                return this.sceneModel.getGroups();
            },

            getSelectedPattern: function () {

                this.updatePatternList();

                var pattern = null;
                var thisObj = this;
                this.patternList.forEach(function (patternDef) {
                    if (thisObj._patternMenuElements.patternDropDown.get('value') === patternDef.name) {
                        pattern = patternDef;
                    }
                });

                return pattern;
            },

            getGroups: function () {
                return this.groupList;
            },

            getGroupNames: function () {

                var nameArray = [];
                this.groupList.forEach(function (group) {

                    nameArray.push(group.name);
                });

                return nameArray;
            },

            createPattern: function(){

                this._createPatternDefinition();
                SimState.createPattern(this.patternDef);

                var newPattern = dojo.clone(this.patternDef);

                var thisObj = this;
                this.patternList.forEach(function (pattern) {

                    if (pattern.name === newPattern.name) {
                        thisObj.patternList.remove(pattern);                      
                    }                     
                });

                this.patternList.add(newPattern);
                this._dispatchPatternsToListeners();

                this._updatePatternDropDown();
            },

            applyPattern: function(){

                var selectedPattern = this.getSelectedPattern();
                if (!!selectedPattern) {
                    this.simulation.simulatePattern(selectedPattern);
                } 
            },

            removePattern: function(){
                 
                var selectedPattern = this.getSelectedPattern();

                var thisObj = this;   
                if (!!selectedPattern) {
                    SimState.removePattern(selectedPattern.name, function () {

                        thisObj.updatePatternDropDown();
                        thisObj.patternList.remove(selectedPattern); 

                    });
                }
            },

            _createPatternDefinition: function () {

                var patternDef = {};

                patternDef.name = this.getPatternName();
                patternDef.groups = this.getGroupNames();
                patternDef.priority = this.getPatternPriority() * 1.0;

                dojo.mixin(this.patternDef, this.effectsDef);
                dojo.mixin(this.patternDef, patternDef);
            },

            updatePatternDefinition: function (effectsDef) {

                this.effectsDef = effectsDef;
                dojo.mixin(this.patternDef, effectsDef);
            },

            

            addGroup: function(groupName){
    
                var group = this.sceneModel.getGroupFromName(groupName);
                
                if(!!group && !this.groupList.contains(group)){

                    this.groupList.add(group);
                    this._updateGroupListBox(this.groupList);
                }
            },

            removeGroups: function (groups) {

                var thisObj = this;
                groups.forEach(function (group) {
                    thisObj.sceneModel.deselectGroup(group.name); 
                    thisObj.groupList.remove(group);                       
                });   

                this._updateGroupListBox(this.getGroups());
            },

            _updateGroupListBox: function(groupList)
            {
                html.removeDomChildren(this.groupListBox);

                var thisObj = this;
                this.groupList.forEach(function (group) {
                    var option = html.createOption(group.name);
                    dojo.connect(option, "onclick", thisObj.sceneModel.selectGroup(group.name)); 
                    thisObj._patternMenuElements.groupListBox.domNode.appendChild(option);
                });
            },

            _updateGroupDropDown: function()
            {
                this._patternMenuElements.groupDropDown.dropDown.destroyDescendants();
                this._patternMenuElements.groupDropDown.set('label', "Select Group");
                
                var ledGroupList = this.getGroupsFromSceneModel();
                var thisObj = this;

                array.forEach(ledGroupList, function (groupName) {
                    var label = groupName;

                    var menuItem = new MenuItem({
                        label: label,
                        onClick: dojo.hitch(thisObj, function (label) {

                            thisObj._patternMenuElements.groupDropDown.set('label', label);
                        }, label)
                    }); 
                    thisObj._patternMenuElements.groupDropDown.dropDown.addChild(menuItem);
                }); 
            },

            _updatePatternDropDown: function(){ 

                this._patternMenuElements.patternDropDown.dropDown.destroyDescendants();
                this._patternMenuElements.patternDropDown.set('label', "Select Pattern");
                                  
                var thisObj = this;
                SimState.getPatternNames(function (patterns) {
                    array.forEach(patterns, function (patternName) {

                        var menuItem = new MenuItem({
                            label: patternName,
                            onClick: dojo.hitch(thisObj, function (patternName) {

                                thisObj.patternDropDown.set('label', patternName);
                                thisObj.patternDropDown.set('value', patternName); 
                            }, patternName)
                        });
                        thisObj.patternDropDown.dropDown.addChild(menuItem);
                    }); 
                      
                });

            },

            _dispatchPatternsToListeners: function(){
                var thisObj = this;
                this.patternsUpdatedListeners.forEach(function (updateListener) {
                    updateListener(thisObj.patternList.clone());
                });
            },

            addPatternUpdateListener: function (patternsUpdated) {
                this.patternsUpdatedListeners.add(patternsUpdated);
            },

            dispatchUpdatesToMenuElementListeners: function(){

                var thisIbj = this;
                this.patternMenuElementListeners.forEach(function (updateListener) {
                    updateListener();
                });
            },

            addPatternMenuElementListener: function(elementUpdateListener){
                this.patternMenuElementListeners.add(elementUpdateListener);
            },
         
            updatePatternList: function(){

                this.patternList.clear();

                var thisObj = this;
                var patterns = SimState.getPatterns(function(patterns){                 
                    array.forEach(patterns, function (pattern) {

                        thisObj.patternList.add(pattern);
                    });   
                });
            }, 

            resetAllProperties: function () {
                 
                this.name = "";
                this.groupList = new ArrayList();
                this.priority = 0;
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;

            }
        });

    });
    