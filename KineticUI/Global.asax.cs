using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.Interact;

namespace KineticUI
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(HttpRuntime.AppDomainAppPath);
            LightSystemProvider.Init(Config.HostInterface);
            Images.GetImages(Config.ImageDirectory);
            Saver.LocalLoad();
            bool enabled = false;
            bool.TryParse(Config.KinectEnabled, out enabled); //false if unsuccessfull
            KinectPlugin.Instance.Enabled = enabled;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}