
/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacqui@revkitt.com
*
*   PatternComposerMenu.js - The sub accordian item for the pattern composer. Located in Design Menu.
*/

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/dom-geometry",
    "dojo/dnd/Moveable",
    "kui/ajax/SimState",
    "dojo/_base/array",
    "kui/DesignMenu/AccordianItem",
    "dojo/dom-class",
    "dojo/dom-style"
],
    function (declare, html, CommonForm, domConstruct, on, domGeom, Moveable,
        SimSate, array, AccordianItem, domClass, domStyle) {

        return declare("kui.DesignMenu.GenerativeMenu.GenerativeMenu", AccordianItem, {

            constructor: function () {
                this.title = "Generative Controller";
            },

            createComposerMenu: function (container) {

                domConstruct.place(this.domNode, container.domNode);

                this.onShow = function () {
                    this.simulation.setGenerativeMode();
                };

                domClass.add(this.domNode, "designMenu");
            },
        });

    });
