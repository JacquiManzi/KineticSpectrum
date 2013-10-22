﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Scenes;
using RevKitt.KS.KineticEnvironment.Sim;
using KineticUI.JSConverters;

namespace KineticUI
{
    public partial class FileUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Stream stream = Request.Browser.Browser == "IE"
                                ? Request.Files[0].InputStream
                                : Request.InputStream;

            IDictionary<string, string> nameToSection;
            LightSystemProvider.ParseProps(stream, out nameToSection);
            State.Scene = new Scene();
            State.PatternSim = new Simulation(State.Scene);
            State.Simulation = new Simulation(State.Scene);

            if (nameToSection.ContainsKey("Groups"))
            {
                string section = nameToSection["Groups"];
                foreach (var group in Serializer.FromString<IEnumerable<Group>>(section))
                {
                    State.Scene.SetGroup(group);
                }
            }

            if (nameToSection.ContainsKey("Patterns"))
            {
                string section = nameToSection["Patterns"];
                foreach (var pattern in Serializer.FromString<IEnumerable<Pattern>>(section))
                {
                    State.Scene.SetPattern(pattern);
                }
            }
            
            Response.Write(JsonConvert.SerializeObject(LightSystemProvider.Lights));
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET <http://ASP.NET> Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
}