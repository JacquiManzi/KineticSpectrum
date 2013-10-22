/// <reference path="../LEDSet.js" />
define([
    "dojo/_base/declare",
    "kui/ModelView/Node/Node",
    "kui/ModelView/ModelEnvironment/Vector3",
    "kui/ModelView/ModelEnvironment/MeshBasicMaterial"
],
    function (declare, Node, Vector3, MeshBasicMaterial) {
        "use strict";
        return declare("kui.Node.Vertex", [Node], {

            RADIUS: 1,
            SEGMENT_WIDTH: 3,
            SEGMENT_HEIGHT: 2,
            RED: 0,
            GREEN: 0,
            BLUE: 0,
            COLOR: 0x00,
            ADDRESS: "",
            IS_VERTEX: true,

            constructor: function () {

                this.red = this.RED;
                this.green = this.GREEN;
                this.blue = this.BLUE;

                this.setRadius(this.RADIUS);
                this.setSegmentWidth(this.SEGMENT_WIDTH);
                this.setSegmentHeight(this.SEGMENT_HEIGHT);

                this.address = this.ADDRESS;
                this.position = new Vector3();
                this.color = this.COLOR;

                this.isVertex = this.IS_VERTEX;
            },

            updatePosition: function (position) {

                this.setX(position.x);
                this.setY(position.y);
                this.setZ(position.z);
            }

        });

    });
