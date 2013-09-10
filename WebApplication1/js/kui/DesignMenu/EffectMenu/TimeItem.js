define([
    "dojo/_base/declare",
    "dojo/dom-construct",
    "kui/DesignMenu/EffectMenu/EffectItem",
    "kui/util/CommonHTML",
    "kui/ajax/Effects",
    "kui/util/CommonFormItems"
], function (declare, domConstruct, EffectItem, html, Effects, CommonForm) {
    return declare("TimeItem", [EffectItem], {

        timeBox: null,
        
        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createRow();
            var timeTitle = html.createCell(html.titleCellStyle);

            timeTitle.innerHTML = this.key;

            var timeValue = html.createCell("text-align:left;" +
                "width:60%;");

            this.timeBox = CommonForm.createTableNumberTextBox("width:100%;");
            this.timeBox.set('value', this.value);

            domConstruct.place(timeTitle, this.domNode);
            domConstruct.place(timeValue, this.domNode);
            this.timeBox.placeAt(timeValue); 
        },

        postCreate: function () {
            this.connect(this.timeBox, "onChange", dojo.hitch(this, function(newValue) {
                this.value = newValue;
                this.onUpdate(this.key, newValue);
            }));
        },

    });

});