
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
    "kui/ajax/Scenes"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList, MenuItem, array, Scenes) {
        "use strict";
        return declare("kui.PatternMenu.patterns.PatternModel", null, {

            /*
             *   
             *
             */

            constructor: function (sceneInteraction) {

                this.name = "";
                this.groupList = new ArrayList();
                this.priority = 0;
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;

                this.groupDropDown = null;
                this.groupListBox = null;

                this.sceneInteraction = sceneInteraction;
            },

            addGroup: function(groupName){


            },

            removeGroups: function (groupList) {

                var thisObj = this;
                array.forEach(groupList, function (group) {
                    thisObj.getGroups().remove(group);                    
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

            updateGroupListBox: function(groupList)
            {
                this.groupListBox.destroyDescendants();

                var thisObj = this;
                groupList.forEach(function (group) {
                    var option = html.createOption(group.groupName);
                    thisObj.groupListBox.domNode.appendChild(option);
                });
            },

            updateGroupDropDown: function()
            {
                this.groupDropDown.destroyDescendants();
                this.groupDropDown.set('label', "Select Group");
                
                var groupList = this.sceneInteraction.groupSet.getGroups();
                var thisObj = this;

                array.forEach(groupList, function (groupName) {
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
