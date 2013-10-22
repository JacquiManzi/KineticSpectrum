define(
    ["dojo/dom-construct",
        "dojox/gfx"
    ], function (domConstruct, GFX) {
        "use strict";

        var selectionOff = "-webkit-touch-callout: none;" +
            "-webkit-user-select: none;" +
            "-khtml-user-select: none;" +
            "-moz-user-select: none;" +
            "-ms-user-select: none;" +
            "user-select: none;";

        /*Create a DIV DOM element*/
        var createDiv = function (style) {
            return domConstruct.create("div",
                {
                    style: style
                });
        },

        /*Create a DOM table element*/
            createTable = function (style) {
                return domConstruct.create("table",
                    {
                        style: style
                    });
            },

        /*Create DOM table row element*/
            createRow = function (style) {
                return domConstruct.create("tr",
                    {
                        style: style
                    });
            },

        /*Create a DOM table cell element*/
            createCell = function (style) {
                return domConstruct.create("td",
                    {
                        style: style

                    });
            },

        /*Create Image*/
            createImage = function (style, src) {
                return domConstruct.create("img",
                    {
                        style: style,
                        src: src
                    });
            },

        /*Create a DOM UL*/
            createUL = function (style) {
                return domConstruct.create("ul",
                    {
                        style: style
                    });
            },

        /*Create DOM List item (li)*/

            createLI = function (style) {
                return domConstruct.create("li",
                    {
                        style: style
                    });
            },

            /*Create DOM Ordered List (ol)*/

            createOL = function (style) {
                return domConstruct.create("ol",
                    {
                        style: style
                    });
            },

        /*Create HTML Link*/
            createLink = function (href, title, innerHTML, rel, type, media) {
                return domConstruct.create("a",
                    {
                        href: href,
                        title: title,
                        innerHTML: innerHTML,
                        rel: rel,
                        type: type,
                        media: media
                    });

            },

            createLabel = function (style, node) {
                return domConstruct.create("label",
                        {
                            style: style,
                            'for': node
                        });
            },


            createStyleSheetLink = function (href, rel, type, media) {
                return domConstruct.create("link",
                    {
                        href: href,
                        rel: rel,
                        type: type,
                        media: media
                    });
            },

            createInput = function (type, size, name, style) {
                return domConstruct.create("input",
                    {
                        size: size,
                        type: type,
                        name: name,
                        style: style
                    });
            },

            createOutput = function () {

                return domConstruct.create("output",
                    {

                    });

            },

            createOption = function (innerHTML) {

                return domConstruct.create("option",
                    {
                        innerHTML: innerHTML

                    });

            },


        /*Create a dojox GFX SVG canvas element inside of a passed DOM DIV*/
            createSVGCanvas = function (div, width, height) {

                if (!height) {
                    height = '100%';
                }

                if (!width) {
                    width = '100%';
                }

                var surface = new GFX.createSurface(div, width, height);

                return surface.createGroup();
            },

            setContentFunction = function (func) {
                this.contentFunc = func;
            },

            asDomNode = function (contentPaneOrNode) {
                if (contentPaneOrNode.domNode) {
                    return contentPaneOrNode.domNode;
                }
                if (contentPaneOrNode.rawNode) {
                    return contentPaneOrNode.rawNode;
                }
                return contentPaneOrNode;
            },

            resizeTextBelow = function (contentPane, width, height) {
                //                defaultSize = defaultSize || 2;

                var domNode = asDomNode(contentPane);

                var paneWidth = dojo.position(domNode).w;
                var paneHeight = dojo.position(domNode).h;
                var minSize = 0.5;

                var relativeHeight = 1, relativeWidth = 1;
                if (paneWidth < width) {
                    relativeWidth = paneWidth / width;
                }
                if (paneHeight < height) {
                    relativeHeight = paneHeight / height;
                }

                var scale = Math.max(Math.min(relativeWidth, relativeHeight) * 2, minSize);
                dojo.setStyle(domNode, 'font-size', scale + 'em');
            },

            /*Dynamically resize the text based on the content pane size*/
            resizeText = function (contentPaneOrDomNode, axis) {
                var domNode = asDomNode(contentPaneOrDomNode);

                var scaleSource,
                    scaleFactor = 0.002,
                    maxScale = 1,
                    minScale = 0.5; //Tweak these values to taste

                if (axis === 'h') {
                    scaleSource = dojo.position(domNode).h;
                }
                else if (axis === 'w') {
                    scaleSource = dojo.position(domNode).w;
                }
                else {
                    scaleSource = Math.sqrt(dojo.position(domNode).w * dojo.position(domNode).h);
                }

                var fontSize = scaleSource * scaleFactor; //Multiply the width of the body by the scaling factor:

                if (fontSize > maxScale) {
                    fontSize = maxScale;
                }
                if (fontSize < minScale) {
                    fontSize = minScale;
                } //Enforce the minimum and maximums

                dojo.setStyle(domNode, 'font-size', fontSize + 'em');

            },

            resizeDialogText = function (dialog) {
                var scaleSource = Math.sqrt(dojo.position(dialog.containerNode).w * dojo.position(dialog.containerNode).h),
                    scaleFactor = 0.002,
                    maxScale = 1,
                    minScale = 0.9; //Tweak these values to taste

                var fontSize = scaleSource * scaleFactor; //Multiply the width of the body by the scaling factor:

                if (fontSize > maxScale) {
                    fontSize = maxScale;
                }
                if (fontSize < minScale) {
                    fontSize = minScale;
                } //Enforce the minimum and maximums

                dojo.setStyle(dialog.containerNode, 'font-size', fontSize + 'em');
            },

        /*Convert CSS hex values to css RGBA values*/
            convertHex = function (hex, opacity) {
                var hexVal = hex.replace('#', '');
                var r = parseInt(hexVal.substring(0, 2), 16);
                var g = parseInt(hexVal.substring(2, 4), 16);
                var b = parseInt(hexVal.substring(4, 6), 16);

                return 'rgba(' + r + ',' + g + ',' + b + ',' + opacity / 100 + ')';
            },

        /*Resize SVG images*/
            resizeSVGImage = function (canvas, domNode, imageHeight, imageWidth) {
                var width = dojo.contentBox(domNode).w;
                var height = dojo.contentBox(domNode).h;

                var scale = Math.min(width / imageWidth, height / imageHeight);

                if (width !== 0) {
                    canvas.setTransform(dojox.gfx.matrix.scale(scale, scale));
                    canvas.applyTransform(dojox.gfx.matrix.translate((width / 2 - scale * imageWidth / 2) / scale, (height / 2 - scale * imageHeight / 2) / scale));
                }
            };

        var backgroundColor = "#141414";

        var textColor = "#3d8dd5";

        var tableStyle = "margin-left:auto;" +
                "margin-right:auto;" +
                "padding-top:5px;" +
                "padding-bottom:5px;" +
                "background-color:" + backgroundColor + ";" +
                "border-radius: 7px;" +                
                "width:99%;" +
                "color:" + textColor + ";";

        var titleCellStyle = "background-color: #232323;" +
                "border-radius:5px;" +
                "width: 40%;";

        var mainBackgroundColor = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                      "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                      "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                      "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                      "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                      "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
            "background-color: #131313;" +
            "background-size: 20px 20px;";
           

        /*Remove dom elements*/
           var removeDomChildren = function (element) {
                element = asDomNode(element);

                if (element != null) {
                    while (element.hasChildNodes()) {
                        element.removeChild(element.firstChild);
                    }
                }
            };
        return {
            createDiv: createDiv,
            createTable: createTable,
            createRow: createRow,
            createCell: createCell,
            createImage: createImage,
            createLabel: createLabel, 
            createUL: createUL,
            createLI: createLI,
            createOL: createOL, 
            createLink: createLink,
            createStyleSheetLink: createStyleSheetLink,
            createSVGCanvas: createSVGCanvas,
            setContentFunction: setContentFunction,
            removeDomChildren: removeDomChildren,
            convertHex: convertHex,
            resizeText: resizeText,
            resizeDialogText: resizeDialogText,
            resizeSVGImage: resizeSVGImage,
            selectionOff: selectionOff,
            resizeTextBelow: resizeTextBelow,
            createInput: createInput,
            createOutput: createOutput,
            createOption: createOption,
            backgroundColor: backgroundColor,
            textColor: textColor,
            tableStyle: tableStyle,
            titleCellStyle: titleCellStyle,
            mainBackgroundColor: mainBackgroundColor
        };
    });
