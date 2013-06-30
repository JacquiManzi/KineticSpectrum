define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems"
], function (declare, domConstruct, parser, ready, _WidgetBase, html, Effects, CommonForm) {
    return declare("EffectItem", [_WidgetBase], {


        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();

        },

        rebuildProperties: function (effectDefinition) {
            domConstruct.empty(this.domNode);

            if (effectDefinition.length > 0) {
                var table = html.createTable(html.tableStyle);
                domConstruct.place(table, this.domNode);
            }
        },


        postCreate: function () {
            // every time the user clicks the button, increment the counter
            //this.connect(this.domNode, "onclick", "increment");
        },

    });

});