

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "dojox/collections/ArrayList",
    "dijit/MenuItem"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, ArrayList, MenuItem) {
        "use strict";
        return declare("kui.PatternMenu.patterns.PatternModel", null, {

            /*
             *   
             *
             */

            constructor: function (sceneInteraction) {



                this.name = "";
                this.groupList = null;
                this.priority = 0;
                this.effectName = "";
                this.effectProperties = {};
                this.id = 0;

                this.groupDropDown = null;
                this.groupDropDownMenu = null;

                this.sceneInteraction = sceneInteraction;

               
            },

            updateGroupList: function(listBox)
            {

               


            },

            updateGroupDropDown: function()
            {
                this.groupDropDownMenu.destroyDescendants();
                this.groupDropDown.set('label', "Select Group");
                

                var groupList = this.sceneInteraction.getGroupOptions();                
                for (var i = 0; i < groupList.count; i++) {
                    var label = groupList.item(i).label;
                    var menuItem = new MenuItem({

                        label: label,
                        onClick: dojo.hitch(this, function (label, selectedGroupOption) {
                            
                            var newList = new ArrayList();
                            newList.add(selectedGroupOption);
                            
                            this.groupDropDown.set('label', label);
                            this.sceneInteraction.showSelectedVertexGroups(newList);
                        }, label, groupList.item(i))

                    });   


                    this.groupDropDownMenu.addChild(menuItem);
                }

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
