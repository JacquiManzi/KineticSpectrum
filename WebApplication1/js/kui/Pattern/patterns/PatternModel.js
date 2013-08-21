
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
        return declare("kui.PatternMenu.patterns.PatternModel", null, {

            /*
             *   
             *
             */

            constructor: function (sceneInteraction) {

                this.groupList = new ArrayList();
                this.patternList = new ArrayList();
                this.patternDef = {};
                this.effectsDef = {}; 
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;

                this.groupDropDown = null; 
                this.groupListBox = null;
                this.nameField = null;
                this.priorityDropDown = null;
                this.patternDropDown = null;

                this.sceneInteraction = sceneInteraction;
                this.simulation = null;

                this.updatePatternList();
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

                this.updatePatternDropDown();
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

            getPatternName: function(){
                return this.nameField.get('displayedValue');
            },

            getPatternPriority: function () {
                return this.priorityDropDown.get('value'); 
            },

            addGroup: function(groupName){
    
                var group = this.sceneInteraction.groupSet.nameToGroup[groupName];
                
                if(!!group && !this.groupList.contains(group)){

                    this.groupList.add(group);
                    this.updateGroupListBox(this.groupList);
                }
            },

            removeGroups: function (groups) {

                var thisObj = this;
                groups.forEach(function (group) {
                    thisObj.sceneInteraction.groupSet.deselectGroup(group.name); 
                    thisObj.groupList.remove(group);                       
                });   

                this.updateGroupListBox(this.getGroups());
            },

            getSelectedGroups: function () {
                var selected = [];
                array.forEach(this.groupListBox.getSelected(), function (group) {
                    selected.push(group);
                });
                return selected;
            },

            getGroups: function(){
                return this.groupList;
            },

            getGroupNames: function () {

                var nameArray = [];
                this.groupList.forEach(function (group) {

                    nameArray.push(group.name);
                });

                return nameArray; 
            },

            updateGroupListBox: function(groupList)
            {
                html.removeDomChildren(this.groupListBox);

                var thisObj = this;
                this.groupList.forEach(function (group) {
                    var option = html.createOption(group.name);
                    dojo.connect(option, "onclick", thisObj.sceneInteraction.groupSet.selectGroup(group.name));
                    thisObj.groupListBox.domNode.appendChild(option);
                });
            },

            updateGroupDropDown: function()
            {
                this.groupDropDown.dropDown.destroyDescendants();
                this.groupDropDown.set('label', "Select Group");
                
                var ledGroupList = this.sceneInteraction.groupSet.getGroups();
                var thisObj = this;

                array.forEach(ledGroupList, function (groupName) {
                    var label = groupName;

                    var menuItem = new MenuItem({
                        label: label,
                        onClick: dojo.hitch(thisObj, function (label) {

                            thisObj.groupDropDown.set('label', label);
                        }, label)
                    });
                    thisObj.groupDropDown.dropDown.addChild(menuItem);
                }); 
            },

            updatePatternDropDown: function(){ 

                this.patternDropDown.dropDown.destroyDescendants();
                this.patternDropDown.set('label', "Select Pattern");    

                if (!!this.sceneInteraction.composerModel.patternListBox) {
                    this.sceneInteraction.composerModel.updatePatternListBox();
                }   
                 
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

            getSelectedPattern: function(){

                this.updatePatternList(); 

                var pattern = null;
                var thisObj = this;
                this.patternList.forEach(function (patternDef) {
                    if (thisObj.patternDropDown.get('value') === patternDef.name) {
                        pattern = patternDef;
                    }                   
                });

                return pattern; 
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
    