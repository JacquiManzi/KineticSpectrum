define([
    "dojo/_base/declare", "dojo/dom-construct", "dojo/parser", "dojo/ready",
    "kui/DesignMenu/EffectMenu/EffectItem",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems"
], function (declare, domConstruct, parser, ready, EffectItem, html, Effects, CommonForm) {
    return declare("TimeItem", [EffectItem], {

        timeBox: null,

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createRow();
            var timeTitle = html.createCell(this.titleCellStyle);

            timeTitle.innerHTML = this.key;

            var timeValue = html.createCell("text-align:left;width:100%;");
            this.timeBox = CommonForm.createIntNumberTextBox("width:100%;");
            this.timeBox.set('value', this.value);

            domConstruct.place(timeTitle, this.domNode);
            domConstruct.place(timeValue, this.domNode);
            this.timeBox.placeAt(timeValue);
        },

        postCreate: function () {
            this.connect(this.timeBox, "onChange", dojo.hitch(this, function (newValue) {
                this.value = newValue;
                this.onUpdate(this.key, newValue);
            }));
        },

    });

});