/*
*   @Author: Jacqui Manzi
*    September 13th, 2013
*    jacqui@revkitt.com
*
*   EffectIten.js - Effect items are placed inside the Effect sub menu of Pattern Menu.
*/

define([
    "dojo/_base/declare",
    "dijit/_WidgetBase"

], function (declare, _WidgetBase) {
    return declare("EffectItem", [_WidgetBase], {


        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();
        },
        
        onUpdate: function(key, value) {
        },


        postCreate: function () {
            // every time the user clicks the button, increment the counter
            //this.connect(this.domNode, "onclick", "increment");
        },

    });

});