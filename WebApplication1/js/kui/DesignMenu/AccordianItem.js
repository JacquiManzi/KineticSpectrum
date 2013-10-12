/*
*   @Author: Jacqui Manzi
*    October 8th, 2013
*    jacquimanzi@gmail.com
*/

define([
    "dojo/_base/declare",
    "dijit/layout/ContentPane",
    "dijit/TitlePane",
    "kui/util/CommonHTML",
    "dojo/dom-construct",
    "dojo/_base/array",
     "dojo/dom-style"
],
    function (declare, ContentPane, TitlePane, html, domConstruct, array, domStyle) {

        return declare("kui.DesignMenu.AccordianItem", ContentPane, {

            constructor: function (title) {

                this.title = title;

                this.domNode = html.createDiv("background-color:transparent;");

                this.backgroundColor = "#141414";
                this.textColor = "#3d8dd5";
                this.tableStyle = "margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:5px;" +
                    "padding-bottom:5px;" +
                    "background-color:" + this.backgroundColor + ";" +
                    "border-radius: 7px;" +
                    //"width:99%;" +
                    "color:" + this.textColor + ";" +
                    "border-spacing:0;";

                this.style = "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;";

            },

            createTitlePane: function (title) {

                var spacerDivStyle = "width:100%;height:7px;";
                var div = html.createDiv("text-align:center;");
               
                var titlePane = new TitlePane({
 
                    title: title,
                    content: div
                });
                
                titlePane.containerNode.setAttribute('style', 'padding:0;background-color:black;');

                return {paneDiv: titlePane.domNode, contentDiv: div};
            },

            addTableItem: function (items, div) {

               var table = html.createTable(this.tableStyle);
               this._createRowItem(items, table);

               domConstruct.place(table, div);
            }, 

            addDivItem: function(divItem, div){

                var itemDiv = html.createDiv("text-align:center;");
                domConstruct.place(divItem.valueContent, itemDiv); 
                domConstruct.place(itemDiv, div); 
            },  

            _createRowItem: function (paneItems, table) {

                array.forEach(paneItems, function (itemMap) {

                    var row = html.createRow();
                    for (var key in itemMap) {

                        var cell = html.createCell("text-align:center;");
                        if (typeof itemMap[key] === 'string') {

                            cell.innerHTML = itemMap[key];
                            domStyle.set(cell, 'width', '10%');
                            domStyle.set(cell, 'padding', '7px'); 
                        }          
                        else {
                            domConstruct.place(itemMap[key], cell);   
                        }
                        domConstruct.place(cell, row);   
                    }
                    domConstruct.place(row, table); 
                });
            }
        });
    });

