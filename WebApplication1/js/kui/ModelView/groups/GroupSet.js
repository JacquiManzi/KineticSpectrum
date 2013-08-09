/*
* @author: Jacqui Manzi
* August, 7th 2013
*/

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dojo/_base/array",
    "kui/ModelView/groups/Group"
],
    function (declare, html, three, ArrayList, array, Group) {
        "use strict";
        return declare("kui.ModelView.groups.GroupSet", null, {

            /*
             *   
             */

            constructor: function (ledSet, patternModel) {

                this.nameToGroup = [];
                this.groupList = new ArrayList();
                this.ledSet = ledSet;
                this.patternModel = patternModel;
                this.ledGroupListBox = null;
              
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
                return Object.keys(this.nameToGroup);
            },
            deleteGroup: function (groupName) {
                delete this.nameToGroup[groupName];
            },

            createGroupFromSelected: function (groupName) {
                groupName = groupName ? groupName : this.generateGroupName();
                var selectedNodes = this.ledSet.getSelectedNodes();

                var group = new Group(groupName, selectedNodes);

                this.nameToGroup[groupName] = group;
                group.applyGroup();

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
            },
          
            /*Select all associated nodes with the groups selected in the group list box*/
            selectGroups: function (groupNames) {
                this.ledSet.deselectAllVertexs();
                this.ledSet.deselectAllLEDs();
                array.forEach(groupNames, dojo.hitch(this, this.selectGroup));
            },

            /*Select all associated nodes with the group selected in the group list box*/
            selectGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                this.ledSet.selectNodes(group.selectedNodes);
            },

            /*Deselect all associated nodes with the group selected in the group list box*/
            deselectGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                this.ledSet.deselectNodes(group.selectedNodes);
            },

            /*Remove all associated nodes with the group selected in the group list box and remove the group from the list box*/
            removeGroup: function (groupName) {
                var group = this.nameToGroup[groupName];
                group.deselectAll();
                this.ledSet.deselectNodes(group.selectedNodes);
                group.remove();
                delete this.nameToGroup[groupName];

                //Update the group list box in the pattern model
                this.patternModel.updateGroupDropDown();
                var thisObj = this;
                this.patternModel.getGroups().forEach(function (group) {

                    if (group.groupName === groupName) {
                        thisObj.groupList.remove(group);
                    }
                });

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
            },
           
            getSelectedGroupOptions: function () {
                return this.ledGroupListBox.getSelected();
            }

        });

    });
