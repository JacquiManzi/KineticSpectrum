
/*
*   @author: Jacqui Manzi
*   August 7th, 2013
*   jacqui@revkitt.com
*
*   Model view controller for Patterns.
*/
define([
    "dojo/_base/declare",
    "dojox/collections/ArrayList",
    "dijit/MenuItem",
    "dojo/_base/array",
    "kui/ajax/Scenes",
    "kui/ajax/SimState"
],
    function (declare, ArrayList, MenuItem, array, Scenes, SimState) {

        return declare("kui.DesignMenu.PatternMenu.PatternModel", null, {

            constructor: function (sceneModel, simulation) {

                this.patternList = new ArrayList();
                this.patternsUpdatedListeners = new ArrayList();
                this.patternMenuElementListeners = new ArrayList();

                this.patternDef = {};
                this.effectsDef = {}; 
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;
                this.nameCount = 0;

                this._patternMenuElements = {
                    groupDropDown: null,
                    groupListBox: null,
                    nameField: null,
                    priorityDropDown: null,
                    patternDropDown: null
                };


                this.sceneModel = sceneModel;
                this.simulation = simulation;

                /*Add pattern menu element listeners*/
                this.addPatternMenuElementListener(dojo.hitch(this, this._updatePatternDropDown));

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

                patternDef.name = this.getPatternNameValue();

                /*Give default name to pattern if none is provided then increment the pattern name counter for unique pattern naming.*/
                if (!patternDef.name || patternDef.name === "") {

                    var isAvailable = false;
                    while(!isAvailable){

                        if(this.patternList.contains("_Pattern" + this.nameCount)){
                            this.nameCount++;
                        }
                        else{
                            patternDef.name = "_Pattern" + this.nameCount++;
                            isAvailable = true;
                        }
                    }            
                }

                patternDef.groups = this.sceneModel.getSelectedGroupNames();
                patternDef.priority = 1.0;

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


            _updatePatternDropDown: function(){ 

                var patternDropDown = this._patternMenuElements.patternDropDown;
                patternDropDown.dropDown.destroyDescendants();
                patternDropDown.set('label', "Select Pattern");
                                  
                var thisObj = this;
                SimState.getPatternNames(function (patterns) {
                    array.forEach(patterns, function (patternName) {

                        var menuItem = new MenuItem({
                            label: patternName,
                            onClick: function () {

                                patternDropDown.set('label', patternName);
                                patternDropDown.set('value', patternName); 
                            }
                        });
                        patternDropDown.dropDown.addChild(menuItem);
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
    