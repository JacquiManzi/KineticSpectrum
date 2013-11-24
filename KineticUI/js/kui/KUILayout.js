﻿/*
*   @Author: Jacqui Manzi
*    August 15th, 2013
*    jacqui@revkitt.com
*
*   KUILayout.js - Initial setup of layout for Kinetic Spectrum. ModelView and DesignMenu are created and initialized here.
*/

define([
      "dojo/_base/declare",
      "dijit/registry",
      "dijit/layout/BorderContainer",
      "dijit/layout/ContentPane",
      "kui/DesignMenu/DesignMenu",
      "kui/ModelView/ModelView",
      "kui/util/CommonHTML",
      "dojo/dom-construct",
      "kui/Simulation/Simulation",
      "kui/Simulation/SimPane",
      "dojo/dom-style"
],
          function (declare, registry, BorderContainer, ContentPane, DesignMenu,
               ModelView, html, domConstruct, Simulation, SimPane, domStyle) {

              return declare("kui.PatternMenu.KUILayout", BorderContainer, {

                  buildRendering: function () {

                      this.inherited(arguments);                     
                      this._createKUILayout();
                  },
                                 
                  _createKUILayout: function()
                  {
                      var simulation = new Simulation();

                      /*Create and place the model view content pane into center container*/
                      var modelView = new ModelView(
                          {
                              simulation: simulation,
                              region: "center",
                              id: "centerContainer",
                              style: "width:100%;" +
                                     "height:100%;" +
                                     "background-color:black;" +
                                     "overflow: hidden;" +
                                     "border:none;" +
                                     "padding: 0px;"
                          });

                      /*The center content pane to the bottom content pane*/
                      this.addChild(modelView);

                      /*setup for the left container*/
                      var designMenu = new DesignMenu(
                          {
                              simulation: simulation,
                              sceneModel: modelView.sceneModel,
                              id: "designMenu"
                          });
                      
                      /*Create the left design menu*/ 
                      designMenu.createMenu();             

                      dojo.connect(modelView, 'onShow', function(){                     
                          dojo.hitch(modelView, modelView.displayModel, "/js/kui/3DModels/GeodesicDome.js")();                                       
                      });
                        
                      /*Left container*/
                      var leftContainer = new ContentPane({
                          isHidden: false,
                          isLayoutContainer: true,
                          region: "left",
                          id: "leftContainer",
                          minSize: ".1",
                          style: "width:26%;" +
                                 "height:100%;" +
                                 "max-width:400px;" +
                                 "padding: 0px"
                      });

                      this.addChild(leftContainer);
                      
                      var leftDiv = html.createDiv("height:100%;" +
                          "width:99%;" +
                          "background-color:black;" +
                          "overflow:hidden;");

                      var innerTopDiv = html.createDiv("height:5%;" +
                          "width:100%;" +
                          "background-color:black;");

                      var innerBottomDiv = html.createDiv("height:5%;" +
                          "width:100%;" +
                          "background-color:black;");

                      domConstruct.place(innerTopDiv, leftDiv);
                      domConstruct.place(designMenu.containerDiv, leftDiv); 
                      domConstruct.place(innerBottomDiv, leftDiv);
                   
                      leftContainer.set('content', leftDiv); 
                                          
                      /*Setup for bottom Container*/ 
                      var bottomContainer = new ContentPane({
                          isLayoutContainer: true,
                          region: "bottom",
                          id: "bottomContatiner",
                          style: "height:0;max-height: 45px;"
                      });

                      this.simPane = new SimPane(
                          {
                              simulation: simulation                              
                          });

                      bottomContainer.set('content', this.simPane);
                      this.addChild(bottomContainer);
                  },

                  startup: function () {
                      this.inherited(arguments);
                      this.simPane.startup();

                      /*Apply custom styling after dom is rendered*/
                      dojo.query(".claro .dijitAccordionInnerContainerSelected .dijitAccordionTitle").style("background-image", "none");
                      dojo.query(".claro .dijitAccordionInnerContainerSelected .dijitAccordionTitle").style("background-color", "#2d2d2d");
                      dojo.query(".claro .dijitTitlePaneTitle").style("background-image", "none");
                      dojo.query(".claro .dijitTitlePaneTitle").style("background-color", "#2d2d2d");
                      dojo.query(".claro .dijitAccordionTitle").style("background-image", "none");
                      dojo.query(".claro .dijitAccordionTitle").style("background-color", "#2d2d2d");
                     
                  }
              });

          });
