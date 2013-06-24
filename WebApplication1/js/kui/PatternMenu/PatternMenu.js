

define([
    "dojo/_base/declare",
    "kui/util/CommonHTML",
    "dojo/dom",
    "dijit/layout/ContentPane",
    "dojo/dom-style",
    "dojo/dom-construct",
    "threejs/three",
    "kui/util/CommonFormItems",
    "kui/PatternMenu/KUIPatterns"],
    function (declare, html, dom, ContentPane, domStyle, domConstruct, three, CommonForm, KUIPatterns) {
        "use strict";
        return declare("kui.PatternMenu.PatternMenu", null, {

            /*
             *   
             *
             */

            constructor: function (obj, obj1, modelView) {

                this.style = "background-color:transparent;";
                this.modelView = modelView;
                

                this.patterns = new KUIPatterns();


            },

            createPatternMenu: function (container) {
                var contentPane = new ContentPane(
                  {
                      title: "Pattern Menu",
                      style: "background:linear-gradient(27deg, #151515 5px, transparent 5px) 0 5px," +
                          "linear-gradient(207deg, #151515 5px, transparent 5px) 10px 0px," +
                          "linear-gradient(27deg, #222 5px, transparent 5px) 0px 10px," +
                          "linear-gradient(207deg, #222 5px, transparent 5px) 10px 5px," +
                          "linear-gradient(90deg, #1b1b1b 10px, transparent 10px)," +
                          "linear-gradient(#1d1d1d 25%, #1a1a1a 25%, #1a1a1a 50%, transparent 50%, transparent 75%, #242424 75%, #242424);" +
                "background-color: #131313;" +
                "background-size: 20px 20px;width:100%;"

                  });

                container.addChild(contentPane);

                var patternDiv = html.createDiv();
                this.createPatternCreateSection(patternDiv);

                domConstruct.place(patternDiv, contentPane.domNode);

                var applyDiv = html.createDiv();
                this.createApplyPattern(applyDiv);

                domConstruct.place(applyDiv, contentPane.domNode);

            },

            createPatternCreateSection: function (div) {

                var patternCreateDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#383838;" +
                    "border-radisu:7px;");
                patternCreateDiv.innerHTML = "Create Pattern";

                var innerDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;"+
                    "padding-bottom: 10px;");


                var table = html.createTable("margin-left:auto;" +
                    "margin-right:auto;" +
                    "padding-top:10px;" +
                    "padding-bottom:10px;");
                var timeRow = html.createDiv();
                var timeTitle = html.createCell();
                
                timeTitle.innerHTML = "Run Time (ms)";

                var timeValue = html.createCell();
                var timeBox = CommonForm.createTableNumberTextBox();

                domConstruct.place(patternCreateDiv, div);
                domConstruct.place(innerDiv, div);
                domConstruct.place(table, innerDiv);
                domConstruct.place(timeRow, table);
                domConstruct.place(timeTitle, timeRow);
                domConstruct.place(timeValue, timeRow);
                domConstruct.place(timeBox.domNode, timeValue);

              
                var obj = this;
                var createButton = CommonForm.createButton('Create Pattern', function () {

                   
                });

                domConstruct.place(createButton.domNode, innerDiv); 


            },

            createApplyPattern: function (div) {


                var patternApplyDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;" +
                    "background-color:#383838;" +
                    "border-radisu:7px;");
                patternApplyDiv.innerHTML = "Apply Pattern";

                var innerDiv = html.createDiv("text-align:center;" +
                    "color:#3d8dd5;");

                domConstruct.place(patternApplyDiv, div);
                domConstruct.place(innerDiv, div);

                var patternDropDown = CommonForm.createDropDown("Select Pattern", "");

                var patternDropDownDiv = html.createDiv("padding-top:10px;");
            
                domConstruct.place(patternDropDown.domNode, patternDropDownDiv);
                domConstruct.place(patternDropDownDiv, innerDiv);

                var obj = this;
                var applyButton = CommonForm.createButton('Apply Pattern', function () {


                });

                domConstruct.place(applyButton.domNode, innerDiv);


            },

            populatePatternDropDown: function () {

                var patterns = this.patterns.getPatterns();
                for (var i = 0; i < patterns.count; i++) {



                }


            }




        });

    });
