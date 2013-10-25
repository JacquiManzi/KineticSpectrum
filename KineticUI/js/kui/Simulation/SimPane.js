define([
    "dojo/_base/declare",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/parser",
    "dojo/ready",
    "dijit/_WidgetBase",
    "kui/util/CommonHTML",
    "kui/util/CommonFormItems",
    "dijit/form/HorizontalSlider",
    "dijit/form/TextBox",
    "dijit/form/Button"
], function (declare, domConstruct,domStyle, parser, ready, _WidgetBase, html, CommonForm, HorizontalSlider, TextBox, Button) {

    return declare("SimPane", [_WidgetBase], {

        slider: null,
        simulation: null,

        buildRendering: function () {
           
            this.domNode = html.createDiv("height: 90px;");
            
            if (this.simulation === null)
                throw new Error("Invalid SimPane, Simulation object must be passed into the constructor");

            var div = html.createDiv(html.tableStyle + "padding-top:0px;" +
                "width:80%;" +
                "height:100%;");

            var buttonDiv = html.createDiv("float:left;" +
                "width:10%;max-width:100px;" +
                "height:100%;");
            var tableDiv = html.createDiv();

            this.button = new Button({
                name: "playButton",
                label: this.simulation.isPlaying ? "Pause" : "Play"
            });

            domStyle.set(this.button.domNode, "height", "100%");
            domStyle.set(this.button.domNode, "width", "92%");

            var table = html.createTable();
           
            domConstruct.place(this.buildSliderRow(), table);
            domConstruct.place(this.buildTextBoxRow(), table);                
            this.button.placeAt(buttonDiv);
            domConstruct.place(buttonDiv, div);
            domConstruct.place(table, tableDiv);
            domConstruct.place(tableDiv, div);

            domConstruct.place(div, this.domNode);
        },

        resize: function(){ 

        },
        
        buildSliderRow: function() {
            this.slider = new HorizontalSlider({
                name: "timeSlider", 
                value: this.simulation.getTime(),
                minimum: 0,
                maximum: this.simulation.endTime,
                intermediateChanges: true,
                style: "width: 100%"
            }, "timeSlider");

                      
            var sliderRow = html.createRow();
            var sliderCell = html.createCell("width:100%;");
            var endCell = html.createCell("width:10%");
            domConstruct.place(sliderCell, sliderRow);
            domConstruct.place(endCell, sliderRow);
            this.slider.placeAt(sliderCell);

            return sliderRow;
        },
        
        buildTextBoxRow: function () {
            this.textBox = new TextBox({
                name: "timeBox",
                value: 0,
                readOnly: true,
                style: "width:10%;" +
                    "background-color:#0a0a0a;" +
                    "border-radius:7px;"
            });

            var boxRow = html.createRow();
            var boxCell = html.createCell("width:100%;" +
                "padding-left:10px;");
            domConstruct.place(boxCell, boxRow);
            this.textBox.placeAt(boxCell);

            return boxRow;
        },
        
        _setEnabled: function(isEnabled) {
            
            //this timeout is pretty gross, but it is required because resizing causes the accordian panes to 
            //resize in the middle of animation. So, we wait until the animation is complete to do our resize.
            setTimeout(dojo.hitch(this, function () {
                domStyle.set(this.domNode.parentNode, 'height', isEnabled ? '6%' : '0px');
                domStyle.set(this.domNode, "display", isEnabled ? "inline" : "none");
                this.getParent().getParent().resize();                
            }), 300);
        },
        
        _sliderChanged: function(newValue) {
            this.textBox.set('value', newValue);
            this.simulation.setTime(newValue);
        },
        
        _playPressed: function() {
            if (this.button.get('label') === "play") {
                this.simulation.play();
            } else {
                this.simulation.pause();
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
            //dojo.connect(this.slider, "onChange", dojo.hitch(this, this._sliderChanged));
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

        startup: function () {
            this._setEnabled(false); 
        }

    });

});