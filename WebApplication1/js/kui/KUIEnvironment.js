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
      "dojo/dom-construct"
],
          function (declare, registry, BorderContainer, ContentPane, ContentPaneX, window, DesignMenu,
               ModelView, on, html, domConstruct) {

              return declare("kui.PatternMenu.KUIEnvironment", null, {
              
                  
                  constructor: function()
                  {

                  },

                  createKUILayout: function()
                  {
                      var windowHeight = window.getBox().h - 20;

                      /*The main border container for the entire KUI*/
                      var mainContainer = new BorderContainer({
                          gutters: false,
                          design: "headline",
                          style: "height:" + windowHeight + "px;width:100%;border:none;"
                      }, "mainContainer");



                      var centerContainer = new ContentPaneX({
                          region: "center",
                          id: "centerContainer",
                          style: "width:90%;" +
                                 "height:95%;" +
                                 "background-color:black;" +
                                 "overflow: hidden;" +
                                 "border:none;",
                          executeScripts: true
                      });

                      /*The center content pane to the bottom content pane*/
                      mainContainer.addChild(centerContainer);

                      /*Create and place the model view content pane into center container*/
                      var modelView = new ModelView();
                      centerContainer.set('content', modelView.domNode);

                      //Create the left design menu
                      var designMenu = new DesignMenu(null, null, modelView);  
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
                          style: "width:15%;" +
                                 "height:100%;"
                      });

                      mainContainer.addChild(leftContainer);

                      var leftDiv = html.createDiv("height:100%;width:100%;background-color:black;overflow:hidden;");
                      var innerTopDiv = html.createDiv("height:10%;width:100%;background-color:black;");
                      var innerBottomDiv = html.createDiv("height:10%;width:100%;background-color:black;");

                     

                      domConstruct.place(innerTopDiv, leftDiv);
                      domConstruct.place(designMenu.domNode, leftDiv);
                      domConstruct.place(innerBottomDiv, leftDiv);

                      leftContainer.set('content', leftDiv);

                      mainContainer.startup();
                  }

             

              });

          });
