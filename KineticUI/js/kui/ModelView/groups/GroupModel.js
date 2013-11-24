/*
* @author: Jacqui Manzi
* August, 7th 2013
* jacqui@revkitt.com
*
* GroupModel.js - Model view controller for groups.
*/

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "dojo/_base/lang",
    "kui/ModelView/groups/Group",
    "kui/ajax/Scenes"
],
    function (declare, html, three, ArrayList, array, lang, Group, Scenes) {
        "use strict";
        return declare("kui.ModelView.groups.GroupSet", null, {

            /*
             *   
             */
            constructor: function (ledSet) {
                this.groupsChangedListeners = new ArrayList();
                this.selectedGroupNames = new ArrayList();
                this.nameToGroup = [];
                this.ledSet = ledSet;
            },

            /**
             * Should be called whenever 
             */
            _dispatchGroups: function() {
                var thisObj = this;
                this.groupsChangedListeners.forEach(function (changeListener) {
                    changeListener(thisObj.getGroups(), thisObj.getSelectedGroupNames());
                });
            },

            /**
             * Registers a function to be called whenever the group membership changes. The provided function will be called
             * whenever a group is added or removed with the full list of group names
             */
            addGroupsUpdatedListener: function (groupsUpdated /*void groupsUpdated(Array<String> groupNames, Array<String> groupsSelected)*/) {
                this.groupsChangedListeners.add(groupsUpdated);
            },

            generateGroupName: function () {
                var largest = 0;
                var pattern = /group\s(\d+)/;
                array.forEach(Object.keys(this.nameToGroup), function (groupName) {
                    var match = pattern.exec(groupName);
                    if (!!match) {
                        largest = Math.max(largest, match[1]);
                    }
                });
                return "group " + (largest + 1);
            },

            getGroups: function () {
                return Object.keys(this.nameToGroup).slice(0);
            },

            getSelectedGroupNames: function() {
                return this.selectedGroupNames.toArray();
            },

            deleteSelectedGroups: function () {

                var thisObj = this;
                this.selectedGroupNames.forEach(function(groupName) {

                    thisObj.deselectGroup(groupName);
                    delete thisObj.nameToGroup[groupName];
                    /*Delete group from server*/
                    Scenes.deleteGroup(groupName);
                });
               
                this.selectedGroupNames.clear();
                this._dispatchGroups();
            },

            getGroupFromName: function(groupName) {
                return this.nameToGroup[groupName];
            },

            createGroupFromSelected: function (groupName) {
                groupName = groupName || this.generateGroupName();
                var selectedNodes = this.ledSet.getSelectedNodes();

                var group = new Group(groupName, selectedNodes);

                this.nameToGroup[groupName] = group;
                group.applyGroup();
                this.selectedGroupNames.add(groupName);
                this._dispatchGroups();

                return groupName;
            },

            addGroups: function (serialGroups) {
                var thisObj = this;
                serialGroups.forEach(function (serialGroup) {
                    var selectedNodes = new ArrayList();
                    var name = serialGroup.name;
                    serialGroup.lights.forEach(function (lightAddress) {
                        var node = thisObj.ledSet.getLEDNode(lightAddress);
                        selectedNodes.add(node);
                    });
                    thisObj.nameToGroup[name] = new Group(name, selectedNodes);
                });
                this._dispatchGroups();
            },
          
            /*Select all associated nodes with the groups selected in the group list box*/
            selectGroups: function (groupNames) {
                if (!this._arraysEqual(groupNames, this.selectedGroupNames.toArray())) {
                    this.ledSet.deselectAllVertexs();
                    this.ledSet.deselectAllLEDs();
                    this.selectedGroupNames.clear();
                    array.forEach(groupNames, dojo.hitch(this, this._selectGroup));
                    this._dispatchGroups();
                }
            },

            _arraysEqual: function (array1, array2) {
                if (array1.length === array2.length) {
                    return array.every(array1, function (item) {
                        return array.indexOf(array2, item) >= 0;
                    });
                }
                return false;
            },

            /*Select all associated nodes with the group selected in the group list box*/
            _selectGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                this.ledSet.selectNodes(group.selectedNodes);

                if (!this.selectedGroupNames.contains(groupName)) {
                    this.selectedGroupNames.add(groupName);
                }
            },

            /*Select all associated nodes with the group selected in the group list box*/
            selectGroup: function (groupName) {
                this._selectGroup(groupName);
                this._dispatchGroups();
            },

            /*Deselect all associated nodes with the group selected in the group list box*/
            deselectGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                group.deselectAll();
                this.ledSet.deselectNodes(group.selectedNodes);
                this.selectedGroupNames.remove(groupName);
                this._dispatchGroups();
            },

            /*Remove all associated nodes with the group selected in the group list box and remove the group from the list box*/
            removeGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                this.deselectGroup(groupName);
                group.remove();
                delete this.nameToGroup[groupName];
                this._dispatchGroups();

                //Update the group list box in the pattern model
                this.patternModel.updateGroupDropDown();
                var thisObj = this;

                this.patternModel.updateGroupListBox(this.patternModel.getGroups());
            },

            showSelectedVertexGroups: function () {

                this.ledSet.deselectAllVertexs();
                this.ledSet.deselectAllLEDs();

                var selectedGroupOptions = this.getGroupOptions();

                array.forEach(selectedGroupOptions, function (option) {

                        var group = this.nameToGroup[option];
                        group.selectAll();                    
                    });             
            }

        });

    });
