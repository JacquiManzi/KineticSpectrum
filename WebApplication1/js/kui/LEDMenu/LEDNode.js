

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/ModelView/VertexSphere"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, VertexSphere) {
        "use strict";
        return declare("kui.LEDMenu.LEDNode", [VertexSphere], {

            /*
             *   LED Node Object
             *
             */

            constructor: function () {

                this.red = 0;
                this.green = 0;
                this.blue = 0;

                this.radius = 0.009;
                this.segmentsWidth = 8;
                this.segmentsHeight = 6;

                this.isVertex = false;
                
            },


            changeColor: function (color) {

                var material = new three.MeshBasicMaterial({

                    color: 0xff0000
                });

                this.sphere.setMaterial(material);

            }

  


         




        });

    });
