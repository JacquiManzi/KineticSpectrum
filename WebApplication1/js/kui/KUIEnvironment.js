define([
      "dojo/_base/declare",
      "dijit/registry",
      "dijit/layout/BorderContainer",
      "dijit/layout/ContentPane",
      "dojox/layout/ContentPane",
      "dojo/window",
      "kui/DesignMenu/DesignMenu",
      "kui/ModelView/ModelView",
      "dojo/on",
      "kui/util/CommonHTML",
      "dojo/dom-construct",
      "kui/Simulation/Simulation",
      "kui/Simulation/SimPane"
],
          function (declare, registry, BorderContainer, ContentPane, ContentPaneX, window, DesignMenu,
               ModelView, on, html, domConstruct, Simulation, SimPane) {

              return declare("kui.PatternMenu.KUIEnvironment", null, {
              
                  
                  constructor: function()
                  {
                       
                  },

                  createKUILayout: function()
                  {
                      var windowHeight = window.getBox().h - 20;

                      var simulation = new Simulation();

                      /*The main border container for the entire KUI*/
                      var mainContainer = new BorderContainer({
                          gutters: false,
                          design: "sidebar",
                          style: "height:" + windowHeight + "px;" +
                              "width:100%;" +
                              "border:none;"
                      }, "mainContainer");


                      ///setup for the center container
                      var centerContainer = new ContentPaneX({
                          region: "center",
                          id: "centerContainer",
                          style: "width:75%;" +
                                 "height:95%;" +
                                 "background-color:black;" +
                                 "overflow: hidden;" +
                                 "border:none;",
                          executeScripts: true
                      });

                      /*The center content pane to the bottom content pane*/
                      mainContainer.addChild(centerContainer);
                      

                      /*Create and place the model view content pane into center container*/
                      var modelView = new ModelView({ simulation: simulation });
                      centerContainer.set('content', modelView.domNode);


                      /*setup for the left container*/
                      var designMenu = new DesignMenu({ simulation: simulation }, null, modelView);
                      
                      /*Create the left design menu*/ 
                      designMenu.createMenu();            

                      dojo.connect(modelView, 'onShow', function(){                     
                          dojo.hitch(modelView, modelView.displayModel, "/js/kui/3DModels/GeodesicDome.js")();                                       
                      });
                        
                      /*Left container*/
                      var leftContainer = new ContentPane({
                          isLayoutContainer: true,
                          region: "left",
                          id: "leftContainer",
                          minSize: ".1",
                          style: "width:25%;" +
                                 "height:100%;" 
                      });

                      mainContainer.addChild(leftContainer);
                      
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
                      domConstruct.place(designMenu.domNode, leftDiv);
                      domConstruct.place(innerBottomDiv, leftDiv);

                      leftContainer.set('content', leftDiv);
                      
                      /*Setup for bottom Container*/ 
                      var bottomContainer = new ContentPane({
                          isLayoutContainer: true,
                          region: "bottom",
                          id: "bottomContatiner",
                          style: "height: 90px;"
                      });

                      var simPane = new SimPane(
                          {
                              simulation: simulation
                          });

                      bottomContainer.set('content', simPane);
                      mainContainer.addChild(bottomContainer);

                      mainContainer.startup();
                  }
              });

          });
