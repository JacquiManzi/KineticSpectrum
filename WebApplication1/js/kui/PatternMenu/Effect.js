

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three"
],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, VertexSphere) {
        "use strict";
        return declare("kui.PatternMenu.Effect", null, {

            /*
             *  Effect
             *
             */

            constructor: function () {

                this.colorEffect = null;
                this.runTime = 0.0;
                this.repeatCount = false;
                this.repeatMethod = null;
                this.duration = 0.0;
                this.ordering = null,
                this.width = 0;
                this.tween = null;
            }










        });

    });
