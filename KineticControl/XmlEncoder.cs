using System.Collections.Generic;
using System.Net;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace KineticControl
{
    public class XmlEncoder
    {
        public static List<PDS> decodeXml(XmlDocument xmlDoc)
        {
            XmlNode Attr = xmlDoc.SelectSingleNode("//Attribute");
            foreach(XmlNode node in Attr.ChildNodes )
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(node["IPAddress"].InnerText), 6038 );
     
                PDS60ca pds = new PDS60ca(Network.GetInstance(), endPoint);
                 
            }
            return null;
        }

        public static XElement encodeXml(IDictionary<string, Matrix> imgToDev)
        {
            XElement cam = new XElement("CameraTransforms");
            foreach(string serial in imgToDev.Keys)
            {
                Matrix mtx = imgToDev[serial];
                XElement camTrans = new XElement("CameraTransform");
                camTrans.Add(new XElement("Serial", serial));
                camTrans.Add(new XElement("M11", mtx.M11 ));
                camTrans.Add(new XElement("M12", mtx.M12));
                camTrans.Add(new XElement("M21", mtx.M21));
                camTrans.Add(new XElement("M22", mtx.M22));
                camTrans.Add(new XElement("OffsetX", mtx.OffsetX));
                camTrans.Add(new XElement("OffsetY", mtx.OffsetY));
                cam.Add(camTrans);
            }
            return cam;
        }

        public static XElement encodeXml(IEnumerable<PDS60ca> pdss)
        {
            XElement pdssNode = new XElement("PDSs");
            foreach (PDS60ca pds in pdss)
            {
                XElement pdsNode = new XElement("PDS");
                pdsNode.Add(new XElement("Address", pds.EndPoint.Address.ToString()));
                pdsNode.Add(new XElement("Port", pds.EndPoint.Port));
                pdsNode.Add(new XElement("Data1", pds.Data1.ToXml()));
                pdsNode.Add(new XElement("Data2", pds.Data2.ToXml()));
                pdssNode.Add(pdsNode);  
            }
            return pdssNode;
        }
    }
}
