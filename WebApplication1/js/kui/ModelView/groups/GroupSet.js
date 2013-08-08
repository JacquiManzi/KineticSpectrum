/*
* Jacqui Manzi
*
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

            constructor: function (ledSet) {

                this.nameToGroup = [];
                this.selectedGroupNames = [];
                this.selectedGroupOptions = new ArrayList();
                this.groupList = new ArrayList();
                this.ledSet = ledSet;
              
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

            addSelectedGroup: function (listBox, groupName) {

                var selectedNodes = this.ledSet.getSelectedNodes();

                if (selectedNodes.count > 0) {

                    var countAmount = this.groupOptionList.count + 1;
                    var group = new Group(countAmount, selectedNodes, groupName);

                    listBox.domNode.appendChild(group.groupOption);
                    this.groupOptionList.add(group.groupOption);

                    group.applyGroup();

                    on(group.groupOption, "click", dojo.hitch(this, function (listBox) {

                        this.selectedGroupOptions.clear();
                        for (var i = 0; i < listBox.getSelected().length; i++) {
                            this.selectedGroupOptions.add(listBox.getSelected()[i]);
                        }
                        this.showSelectedVertexGroups(this.selectedGroupOptions);

                    }, listBox));

                }

                this.patternModel.updateGroupDropDown();
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
            },

            showSelectedVertexGroups: function (selectedGroupOptions) {

                this.ledSet.deselectAllVertexs();
                this.ledSet.deselectAllLEDs();
                for (var i = 0; i < selectedGroupOptions.count; i++) {

                    var option = selectedGroupOptions.item(i);

                    for (var j = 0; j < option.list.count; j++) {
                        option.list.item(j).isSelected = true;

                        var selectionMaterial = new three.MeshBasicMaterial({

                            color: 0xff0000
                        });

                        option.list.item(j).setMaterial(selectionMaterial);
                    }

                }

            },

            getGroupOptions: function () {

                var groupOptions = new ArrayList();

                for (var i = 0; i < this.groupOptionList.count; i++) {

                    groupOptions.add(this.groupOptionList.item(i));
                }
                return groupOptions;

            }

        });

    });
