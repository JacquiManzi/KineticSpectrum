/*
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
      "dojo/dom-style",
      "dojo/dom-class",
      "dojo/on"
],
          function (declare, registry, BorderContainer, ContentPane, DesignMenu,
               ModelView, html, domConstruct, Simulation, SimPane, domStyle, domClass, on) {

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
                          dojo.hitch(modelView, modelView.displayModel, "/js/kui/3DModels/bag.js")();                                       
                      });
                        
                      /*Left container*/
                      var leftContainer = new ContentPane({
                          isHidden: false,
                          isFullScreen: false,
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
                          "width:100%;" +
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

                      var modeDiv = html.createDiv("width: 100px;" +
                          "height:30px;"+
                          "position: absolute;"+
                          "top:0;"+
                          "right:0;"+
                          "background-color:#141414;"+
                          "display: table;"+
                          "vertical-align: middle;"+
                          "text-align: center;"+
                          "margin: 10px;"+
                          "border: 2px solid #222222;");

                      var liveDiv = html.createDiv(
                          "background-color:#8e0100;");

                      domClass.add(liveDiv, "modeItem"); 

                      liveDiv.innerHTML = "live";

                      var vertexDiv = html.createDiv(
                          "background-color:#141414;" +
                          "color:#e0e0e0;"+
                          "background-image:url('../js/kui/images/vertex.png');");

                      domClass.add(vertexDiv, "modeItem");

                      on(vertexDiv, "click", function () {

                          if (!modelView.sceneInteraction.verticesHidden) {
                              modelView.sceneInteraction.nodeModel.hideAllVertices();
                              modelView.sceneInteraction.verticesHidden = true;

                              domStyle.set(vertexDiv, "background-color", "#0072c4");
                          }
                          else {
                              modelView.sceneInteraction.nodeModel.showAllVertices();
                              modelView.sceneInteraction.verticesHidden = false;

                              domStyle.set(vertexDiv, "background-color", "transparent"); 
                          }
                      });

                      domConstruct.place(liveDiv, modeDiv); 
                      domConstruct.place(vertexDiv, modeDiv);
                      domConstruct.place(modeDiv, modelView.domNode);
                     
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
