define([
    "dojo/_base/declare",
    "kui/ModelView/ModelEnvironment/Vector3",
    "kui/ModelView/ModelEnvironment/Mesh",
    "kui/ModelView/ModelEnvironment/MeshBasicMaterial",
    "kui/ModelView/ModelEnvironment/MeshNormalMaterial",
    "kui/ModelView/ModelEnvironment/SphereGeometry",
     "kui/ModelView/ModelEnvironment/Color"
],
    function (declare, Vector3, Mesh, MeshBasicMaterial, MeshNormalMaterial, SphereGeometry, Color) {
        "use strict";
        return declare("kui.ModelMenu.Node.Node", Mesh, {

            segmentWidth: 3,
            segmentHeight: 2,

            constructor: function () {
              
                this.setMaterial(new MeshNormalMaterial());
                //his.setSphereGeometry(radius, this.segmentWidth, this.segmentHeight);
                
                this.radius = 2;
                this.isVertex = true;
                this.addModeOn = true;

                this._createMeshSphere(0, 0 ,0);

                this.overdraw = true; //what is this?
                this.isSelected = false;

                this.address = ""; //necessary?
            },

            _createMeshSphere: function( x, y, z){

                this.setX(x);
                this.setY(y);
                this.setZ(z);

                var vector = new Vector3();
                vector.setCoords(x, y, z);

                this.coords = vector;
                this.geometry.computeFaceNormals(); //this about moving this to container class
            },

            setColor: function (color) {
                if (color == 0x000000) {
                    this.unselect();
                    return;
                }
                if (!this.material.color) {
                    var material = new MeshBasicMaterial();
                    material.setColor(color);
                    this.setMaterial(material);
                } else if(this.material.color.getHex() !== color) {
                    this.material.setColor(color);
                }
            },

            select: function () {
                var color = new Color(0xff0000);
                this.setColor(color);
                this.isSelected = true;
            },

            unselect: function () {
                if (this.material.color) {
                    var material = new MeshNormalMaterial();
                    this.setMaterial(material);
                    this.isSelected = false;
                    this.hasColor = false;
                }
            },
           
            setRadius: function (radius) {
                this.radius = radius;
                this.setSphereGeometry(radius, this.segmentWidth, this.segmentHeight);
            },

            setSegmentWidth: function (segmentWidth) {
                this.segmentWidth = segmentWidth;
                this.setSphereGeometry(this.radius, segmentWidth, this.segmentHeight);
            },

            setSegmentHeight: function (segmentHeight) {
                this.segmentHeight = segmentHeight;
                this.setSphereGeometry(this.radius, this.segmentWidth, segmentHeight);
            },
         
            setCoords: function (x, y, z) {
                this.setX(x);
                this.setY(y);
                this.setZ(z);
            }
             

        });

    });
