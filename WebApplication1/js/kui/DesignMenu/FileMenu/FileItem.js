/*
* @Author: Jacqui Manzi
* jacqui.manzi@gmail.com
*
* September 6th, 2013
*
* FileItem.js
*
*/


define([
    "dojo/_base/declare",
    "dijit/_WidgetBase"

], function (declare, _WidgetBase) {
    return declare("FileItem", [_WidgetBase], {


        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();
        },

        onUpdate: function (key, value) {
        },


        postCreate: function () {
        },

    });

});