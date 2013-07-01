define([
    "dojo/_base/declare", "dojo/dom-construct","dojo/dom-style", "dojo/parser", "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dijit/form/HorizontalSlider",
    "dijit/form/TextBox",
    "dijit/form/Button",
    "dojo/_base/xhr"
], function (declare, domConstruct,domStyle, parser, ready, _WidgetBase, html, CommonForm, HorizontalSlider, TextBox,Button, xhr) {
    return declare("SimPane", [_WidgetBase], {

        enabled: false,
        slider: null,
        simulation: null,

        buildRendering: function () {
            // create the DOM for this widget
            this.domNode = html.createDiv();

            if (this.simulation === null)
                throw new Error("Invalid SimPane, Simulation object must be passed into the constructor");

            var table = html.createTable(html.tableStyle);
            domStyle.set(table, 'width', '80%');

            domConstruct.place(this.buildSliderRow(), table);
            domConstruct.place(this.buildTextBoxRow(), table);

            domConstruct.place(table, this.domNode);
        },
        
        buildSliderRow: function() {
            this.slider = new HorizontalSlider({
                name: "timeSlider",
                value: 0,
                minimum: 0,
                maximum: 0,
                intermediateChanges: true,
                onChange: function(value){
                    dom.byId("sliderValue").value = value;
                }
            }, "timeSlider");

            this.button = new Button({
                name: "playButton",
                label: "play"
            });
            
            var sliderRow = html.createRow();
            var sliderCell = html.createCell();
            var buttonCell = html.createCell("width:10%;");
            var endCell = html.createCell("width:10%");
            domConstruct.place(buttonCell, sliderRow);
            domConstruct.place(sliderCell, sliderRow);
            domConstruct.place(endCell, sliderRow);
            this.slider.placeAt(sliderCell);
            this.button.placeAt(buttonCell);

            return sliderRow;
        },
        
        buildTextBoxRow: function () {
            this.textBox = new TextBox({
                name: "timeBox",
                value: 0,
                readOnly:true
            });

            var boxRow = html.createRow();
            var boxCell = html.createCell();
            domConstruct.place(html.createCell(), boxRow);
            domConstruct.place(boxCell, boxRow);
            this.textBox.placeAt(boxCell);

            return boxRow;
        },
        
        _setEnabled: function(isEnabled) {
            domStyle.set(this.domNode, "visibility", isEnabled?"visible":"hidden");
        },
        
        _sliderChanged: function(newValue) {
            this.textBox.set('value', newValue);
            this.simulation.setTime(newValue);
        },
        
        _playPressed: function() {
            if (this.button.get('label') === "play") {
                this.button.set('label', 'pause');
                this.simulation.pause();
            } else {
                this.button.set('label', 'play');
                this.simulation.play();
            }
        },
        
        _timeUpdated: function(time) {
            this.slider.set('value', time);
            this.textBox.set('value', time);
        },
        
        _endTimeUpdated: function(endTime) {
            this.slider.set('maximum', endTime);
        },
        
        _playUpdated: function(isPlaying) {
            this.button.set('label', isPlaying ? "pause" : "play");
        },

        postCreate: function () {
            //// First handle changes to UI Elements
            //handle the slider being moved
            dojo.connect(this.slider, "onChange", dojo.hitch(this, this._sliderChanged));
            //handle the play button being pressed
            dojo.connect(this.button, "onClick", dojo.hitch(this, this._playPressed));
            
            ////Then update in response to Simulation state changes
            //handle updated time
            dojo.connect(this.simulation, "onTimeChange", dojo.hitch(this, this._timeUpdated));
            //handle updated end time
            dojo.connect(this.simulation, "onEndTimeChange", dojo.hitch(this, this._endTimeUpdated));
            //handle change in play state
            dojo.connect(this.simulation, "onPlayChange", dojo.hitch(this, this._playUpdated));
            //handle change in simulation mode
            dojo.connect(this.simulation, "onSimMode", dojo.hitch(this, this._setEnabled));
        },

    });

});