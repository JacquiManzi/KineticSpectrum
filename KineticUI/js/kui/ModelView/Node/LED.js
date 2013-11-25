define([
    "dojo/_base/declare",    
    "kui/ModelView/Node/Node",
    "kui/ModelView/ModelEnvironment/Vector3",
    "kui/ModelView/Node/LightAddress",
    "kui/ModelView/ModelEnvironment/MeshBasicMaterial"
],
    function (declare, Node, Vector3, LightAddress, MeshBasicMaterial) {
        "use strict";
        return declare("kui.ModelView.Node.LED", [Node], {

            RADIUS: 0.02,
            SEGMENT_WIDTH: 8,
            SEGMENT_HEIGHT: 6,
            RED: 0,
            GREEN: 0,
            BLUE: 0,
            COLOR: 0x00,
            ADDRESS: "",
            IS_VERTEX: false,

            constructor: function () {

                this.red = this.RED;
                this.green = this.GREEN;
                this.blue = this.BLUE;

                this.setRadius(this.RADIUS);
                this.setSegmentWidth(this.SEGMENT_WIDTH);
                this.setSegmentHeight(this.SEGMENT_HEIGHT);

                this.address = new LightAddress();
                this.position = new Vector3();
                this.color = this.COLOR;

                this.isVertex = false;
            },

            //TODO: See if color is being passed in any where
            changeColor: function (color) {

                var material = new MeshBasicMaterial();
                material.setColor(0xff0000);

                this.sphere.setMaterial(material);
            },

            updatePosition: function (position) {

                this.position = position;
                this.x = this.position.x;
                this.y = this.position.y;
                this.z = this.position.z;
            }
             
  
        });

    });
