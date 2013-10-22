define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three"
],
    function (declare, html, dom, domStyle, domConstruct, three) {
        "use strict";
        return declare("kui.ModelView.Axis", null, {
            /*
             *   
             *
             */

            material: new three.LineBasicMaterial({color: 0xFFFFFF}),
            xAxis: null,
            yAxis: null,
            zAxis: null,

            constructor: function (scene) {
                this._scene = scene;
                this.constructAxis();

            },

            constructAxis: function() {
                var geometry = new three.Geometry();
                geometry.vertices.push(new three.Vector3(10, 0, 0));
                geometry.vertices.push(new three.Vector3(0, 0, 0));
                geometry.vertices.push(new three.Vector3(-10, 0, 0));
                this.xAxis = new three.Line(geometry, new three.LineBasicMaterial({color: 0xFF0000}));

                geometry = new three.Geometry();
                geometry.vertices.push(new three.Vector3(0, 10, 0));
                geometry.vertices.push(new three.Vector3(0, 0, 0));
                geometry.vertices.push(new three.Vector3(0, -10, 0));
                this.yAxis = new three.Line(geometry, new three.LineBasicMaterial({color: 0x00FF00}));

                geometry = new three.Geometry();
                geometry.vertices.push(new three.Vector3(0, 0, 10));
                geometry.vertices.push(new three.Vector3(0, 0, 0));
                geometry.vertices.push(new three.Vector3(0, 0, -10));
                this.zAxis = new three.Line(geometry, new three.LineBasicMaterial({color: 0x0000FF}));

                this._scene.add(this.xAxis);
                this._scene.add(this.yAxis);
                this._scene.add(this.zAxis);
            }




        });

    });
