using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebApplication1
{
    public partial class FileUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = Request.Files[0].FileName; //name of file I pass in
            byte[] rawFile;


            if (Request.Browser.Browser == "IE")
            {
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    rawFile = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }
            }
            else
            {
                using (var binaryReader = new BinaryReader(Request.InputStream))
                {
                    rawFile = binaryReader.ReadBytes((int)Request.InputStream.Length);

                }
            }

            StreamWriter writer = new StreamWriter("C:\\kui\\files\\" + fileName);
            writer.Write(rawFile);
            writer.Close();

            Response.Write("{\"success\": true}");
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