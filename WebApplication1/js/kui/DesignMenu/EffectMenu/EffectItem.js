define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
        "dojo/on"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, CommonForm, on) {
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