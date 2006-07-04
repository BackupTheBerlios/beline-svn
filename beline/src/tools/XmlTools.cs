using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using LibBeline;

namespace Beline.Tools
{
	/// <summary>
	/// Xsl Transforamtions from Xml message to Html output
	/// </summary>
	public class XslTransform
	{
	  private XslTransform()
	  {}
	  
	  /// Transform input Xml document to output HTML document using beline.html.xslt stylesheet
	  /// <param name="inputDocument">Transformed document</param>
	  /// <param name="stylesheet">A name of a styleseet file (this file is in temlate's path)</param>
	  public static string Transform(XmlDocument inputDocument, string stylesheet)
	  {
      BConfigItem global = LibBeline.LibBeline.GetInstance().ConfigManager.GlobalConf;
      string templatePath = global["/beline/conf/global/paths[@templates]"].ToString();
      string stylesheetPath = Path.Combine(templatePath, stylesheet);
      // open stylesheet and xml document
      System.Xml.Xsl.XslTransform xslt = new System.Xml.Xsl.XslTransform();
      xslt.Load(stylesheetPath);

      // open output (if not defined send to standard output)
      StringWriter retval = new StringWriter();

      // do the transformation
      xslt.Transform(inputDocument, null, retval, null);
      
      return retval.ToString();    
		}
		
    /// Take a "procedure" element and using a XSL transformation create HTML that is shown on the main page
    /// <param name="oid">Module's oid (the one that should be shown)</param>
    public static string CreateProcedureHtml(string oid)
    {
      // get root of configuration
  	  BConfigItem configuration = BMasterServiceManager.GetInstance().GetConfig(oid);
   	  XmlNode rootNode = configuration.GetXmlNode("/"); 

      XmlDocument document = new XmlDocument();
      document.LoadXml(rootNode.OuterXml);
      return Transform(document, "procedure.html.xslt");
    }
	}
}
