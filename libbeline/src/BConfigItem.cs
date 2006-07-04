using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace LibBeline {
  /// <summary>
  /// This class store the configuration of exactly one module or the global configuration
  /// </summary>
  public sealed class BConfigItem : BItem {
  
    #region attributes
    /// Obsah rozparsovaného dokumentu
    private XmlDocument innerValue;
    /// <summary>Way to the first XML document with a configuration</summary>
    public string File
    {
      get {return fileName; }
    }
    private string fileName;
    #endregion
    
    /// <summary>
    /// Create configuration item based on a XML configuration file.
    /// </summary>
    /// <param name="aPath">The whole path to a XML configuration file.</param>
    /// <exception> XmlException, FileNotFoundException </exception>
    public BConfigItem (string aPath)
    {
      oid = BItem.GenerateID();
      // overeni existence souboru
      FileInfo lfi = new FileInfo(aPath);
      if (!lfi.Exists) throw new FileNotFoundException("File " + aPath + " not found.");
      
      // read the xml configuration from file
      innerValue = new XmlDocument();
      innerValue.Load(aPath);
      
      // TODO check if it is DTD valid
      fileName = Path.GetFileName(aPath);
    }
    
    /// <summary>gets/sets the first found value according to a XPath value
    /// if user want to set BNull value, selected items will be deleted</summary>
    /// <param name="XPath">The XPath expression</param> 
    /// <return>The BString object with value if the node has found or the BNull if the node hasn't found.</return>
    public BValueType this[string XPath]
    {
      get
      {       
        string wayToElement, wayToAttribute;
        if (XPath.Substring(XPath.LastIndexOf('/')).LastIndexOf('[') == -1)
        { // it has only way to element (ie. /way/to/element)
          wayToElement = XPath;
          wayToAttribute = "";
        }
        else
        { // it has way to specified attribute in element (ie. /way/to[@attr='10']/element[attribute])
          wayToElement = XPath.Substring(0, XPath.LastIndexOf('['));
          wayToAttribute= XPath.Substring(XPath.LastIndexOf('[') + 2, XPath.Length - XPath.LastIndexOf('[') - 3);
        }
        
        if (wayToAttribute == "")
        { // he wan't to return element's CData
          XmlNodeList nodeList = innerValue.SelectNodes(wayToElement);
          if (nodeList.Count == 0)
            return new BNull(XPath); // nothing found => return BNull value
          else
            return new BString(XPath, nodeList[0].InnerText.Trim());  // return first CData element
        }
        
        XmlElement node = (XmlElement)this.GetXmlNode(wayToElement);
        if (node == null)
            return new BNull(XPath); // nothing found => return BNull value
          else
            return new BString(XPath, node.GetAttribute(wayToAttribute));  // return value of attribute

//        foreach (XmlNode node in innerValue.SelectNodes(wayToElement))
//        { // go throught all found elements and find the one with wayToAttribute attribute
//          if (node.NodeType != XmlNodeType.Element) continue;
//          XmlElement element = (XmlElement)node;
//          
//          if (element.HasAttribute(wayToAttribute))
//          {
//            string attributeValue = element.GetAttribute(wayToAttribute);
//            BString retval = new BString(wayToAttribute, attributeValue);
//            return (BValueType)retval;
//          }
//        }
        // none of given XPath were found
//        return new BNull(aXPath);
      }
      
//	  	set
//	  	{
////	  	  string wayToElement, wayToAttribute;
////        if (aXPath.IndexOf('[') == -1)
////        { // it has only way to element (ie. /way/to/element)
////          wayToElement = aXPath; 
////          wayToAttribute = "";
////        }
////        else
////        { // it has way to specified attribute in element (ie. /way/to/element[@attribute])
////          wayToElement = aXPath.Substring(0, aXPath.IndexOf('[') - 1);
////          wayToAttribute= aXPath.Substring(aXPath.IndexOf('['), aXPath.Length);
////        }
//        
//        // way splitted to array of elements
//        string[] tmpElements = wayToElement.Split('/');
//        // pointer to inner value's elements
//        XmlElement tmpInnerValue = null, tmpInnerValue2 = innerValue.DocumentElement;
//        string attributeName = string.Empty; string attributeValue = string.Empty; 
//        
//        foreach (string elementName in tmpElements)
//        {
//          if (elementName.IndexOf('[') != -1)
//          { // contains attribute's specification
//            string attributeSpecification = elementName.Substring(elementName.IndexOf('['), elementName.Length);
//            if (attributeSpecification != null && attributeSpecification != string.Empty && attributeSpecification.StartsWith("@"))
//            { // if attribute specification is present parse it
//              attributeName = attributeSpecification.Substring(1,attributeSpecification.IndexOf("=") - 2);
//              attributeValue = attributeSpecification.Substring(attributeSpecification.IndexOf("\""), attributeSpecification.Length - attributeSpecification.IndexOf("\""));
//              elementName = elementName.Substring(0, elementName.IndexOf('[') - 1);
//            }
//          }
//          
//          tmpInnerValue = tmpInnerValue2; // jump to next level
//          if ((tmpInnerValue2 = tmpInnerValue[elementName]) != null)
//          { // exists at least one subelement with this name
//            foreach (XmlNode subNode in tmpInnerValue.ChildNodes)
//            {
//              if (subNode.NodeType != XmlNodeType.Element || subNode.LocalName != elementName) continue;
//              XmlElement subNodeElement = (XmlElement)subNode;
//              
//              string attribute = subNodeElement.GetAttribute(attributeName);
//              if (attribute == null || attribute != attributeValue) continue; // another attribute
//              // searched node found
//              tmpInnerValue2 = subNodeElement;
//            }
//          }
//          
//          
//          // nevytvarej nove elementy
//          if (tmpInnerValue2 == null) break;
////          if (tmpInnerValue2 == null)
////          { // suitable element not found
////            XmlDocument tmpDocument = new XmlDocument();
////            tmpDocument.CreateElement(elementName);
////            tmpInnerValue2 = (XmlElement)tmpInnerValue.InsertBefore(tmpDocument, null);
////            if (attributeName != string.Empty)
////            {
////              tmpInnerValue2.SetAttribute(attributeName, attributeValue);
////            }
////          }
//          
//          attributeName = string.Empty;
//        } 
//        
//        // now in tmpInnerValue2 is the proper last element in XPath or null
//        
////        if (wayToAttribute != "")
////        { // set last element's CData
////          if (value.GetBType() == BEnumType.BNull)
////          { // remove element
////            if (tmpInnerValue == null) return; // can't set there
////            tmpInnerValue.RemoveChild(tmpInnerValue);
////          }
////          else 
////          { // set string value as an element's CData
////            tmpInnerValue2.InnerText = value.ToString();
////          }
////        }
////        else
////        { // set attribute value
////          if (value.GetBType() != BEnumType.BNull)
////          {
////            tmpInnerValue2.SetAttribute(wayToAttribute, value.ToString());
////          }
////          else
////          { 
////            tmpInnerValue2.RemoveAttribute(wayToAttribute);
////          }
////        }
//	  	} // setter 
    }
    
    /// execute XPath query and return first result as XML
    /// <param name="XPath">The XPath expression</param>
    public XmlNode GetXmlNode(string XPath)
    {
// TODO dodelat na opravdovy XPath
//      XPathNavigator navigator = innerValue.CreateNavigator();
//      XPathExpression expression = navigator.Compile(aXPath);
//      
      // select XPath result
      return innerValue.SelectSingleNode(XPath); 
      
//      string wayToElement, wayToAttribute;
//      if (aXPath.IndexOf('[') == -1)
//      { // it has only way to element (ie. /way/to/element)
//        wayToElement = aXPath;
//        wayToAttribute = "";
//      }
//      else
//      { // it has way to specified attribute in element (ie. /way/to/element[@attribute])
//        wayToElement = aXPath.Substring(0, aXPath.IndexOf('[') - 1);
//        wayToAttribute= aXPath.Substring(aXPath.IndexOf('['), aXPath.Length);
//      }
//
//      // Find proper element
//      string lastElementName = wayToElement.Substring(wayToElement.LastIndexOf('/'));
//
//      if (wayToAttribute == "")
//      { // he wan't to return element's CData
//        XmlNodeList nodeList = innerValue.SelectNodes(wayToElement);
//        if (nodeList.Count == 0)
//          return null;
//        else
//          return nodeList[0];  // return whole first element
//      }
//
//      foreach (XmlNode element in innerValue.SelectNodes(wayToElement))
//      { // go throught all found elements and find the one with wayToAttribute attribute
//        if (element[lastElementName].HasAttribute(wayToAttribute))
//        {
//          return element[lastElementName].GetAttributeNode(wayToAttribute);
//        }
//      }
//      // none of given XPath were found
//      return null;
    }
    
    // TODO dodelat opravdovy XPath, nebo dopsat omezeni na XPath (napriklad pomoci BNF)
    /// look for first element given with XPath and set him to the attribute new value
    /// if XPath don't return element nothing will be done
    /// <param name="aXPath">XPath expression</param>
    /// <param name="aAttributeName">Name of the attribute which will be changed</param>
    /// <param name="aAttributeValue">New value for the attribute</param>
    public void SetAttribute(string aXPath, string aAttributeName, string aAttributeValue)
    {   
      // remove heading and end slashes and first tag "beline"
      aXPath = aXPath.Trim('/');
      aXPath = aXPath.Substring(aXPath.IndexOf("/") + 1, aXPath.Length - aXPath.IndexOf("/") - 1);
      
      // way splitted to array of elements
      string[] tmpElements = aXPath.Split('/');
      // pointer to inner value's elements
      XmlElement tmpInnerValue = null, tmpInnerValue2 = innerValue.DocumentElement;
      string attributeName = string.Empty; string attributeValue = string.Empty; 

      foreach (string elementName in tmpElements)
      {
        string innerElementName = elementName; // must be because elementName can't be changed
        if (elementName.IndexOf('[') != -1)
        { // contains attribute's specification
          string attributeSpecification = elementName.Substring(elementName.IndexOf('[') + 1, elementName.Length - elementName.IndexOf('[') - 2);
          if (attributeSpecification != null && attributeSpecification != string.Empty && attributeSpecification.StartsWith("@"))
          { // if attribute specification is present parse it
            attributeName = attributeSpecification.Substring(1,attributeSpecification.IndexOf("=") - 1);
            attributeValue = attributeSpecification.Substring(attributeSpecification.IndexOf("'") + 1, attributeSpecification.Length - attributeSpecification.IndexOf("'") - 2);
            innerElementName = elementName.Substring(0, elementName.IndexOf('['));
          }
        }

        tmpInnerValue = tmpInnerValue2; // jump to next level
        if ((tmpInnerValue2 = tmpInnerValue[innerElementName]) != null)
        { // exists at least one subelement with this name
          foreach (XmlNode subNode in tmpInnerValue.ChildNodes)
          {
            if (subNode.NodeType != XmlNodeType.Element || subNode.LocalName != innerElementName) continue;
            XmlElement subNodeElement = (XmlElement)subNode;
            
            string attribute = subNodeElement.GetAttribute(attributeName);
            if (attribute == null || attribute != attributeValue) continue; // another attribute
            // searched node found
            tmpInnerValue2 = subNodeElement;
          }
        }

        // nevytvarej nove elementy
        if (tmpInnerValue2 == null) 
        {
          Console.WriteLine("break!!!!!!!!!!!!!");
          break;
        }

        attributeName = string.Empty;
      } 

      // now in tmpInnerValue2 is the proper last element in XPath or null
      if (tmpInnerValue2 != null)
      {
        tmpInnerValue2.SetAttribute(aAttributeName, aAttributeValue);
      }
    }
    
    /// <summary>
    /// Execute XPath query and return all found elements.
    /// </summary>
    /// <param name="aXPath">A XPath to searched elements.</param>
    /// <returns>All found elemets stored in BObject</returns>
    public BObject GetBObject (string aXPath)
    {
      XmlNodeList nodeList = innerValue.SelectNodes(aXPath);
      ArrayList retval = new ArrayList(nodeList.Count);
      
      foreach (XmlNode node in nodeList)
      { // go throught all found elements and set them to the return value
        retval.Add(BObject.Deserialize(node));
      }
      
      return new BObject(aXPath, retval);
    }
    
    /// <summary>
    /// Return URI to help to first selected item.
    /// </summary>
    /// <param name="aXPath">A XPath to searched elements.</param>
    /// <returns>Return help to the first fond elment of empty string if element not found.</returns>
    public string GetHtmlHelp (string aXPath)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    
    /// <summary>
    /// Return hint to first selected item.
    /// </summary>
    /// <param name="aXPath">A XPath to searched elements.</param>
    /// <returns>Return help to the first fond elment of empty string if element not found.</returns>
    public string GetHint (string aXPath)
    {
      throw new System.Exception ("Not implemented yet!");
    }

    /// <summary>
    /// Read Xml configuration from given file and merge it to a configuration stored in this
    /// object. This is specialized to master's configuration
    /// </summary>
    /// <param name="aPath">The whole path to a master's configuration file.</param>    
    public void ImportGlobalConfig (string aPath)
    {
      // check if the specified file exists
      FileInfo lfi = new FileInfo(aPath);
      if (!lfi.Exists) return;  // file not found => can't import another info
      
      // temporary xml document
      XmlDocument tmpXml = new XmlDocument();
      tmpXml.Load(aPath);
      
      try
      {
        string attribute;
        XmlElement configNode = (XmlElement)(tmpXml.DocumentElement["conf"]["global"]);
        // process limit node
        XmlElement limitNode = (XmlElement)configNode["limit"];
        if ((attribute = limitNode.GetAttribute("maxmodulescount")) != string.Empty) this.SetAttribute("/beline/conf/global/limit", "maxmodulescount", attribute);
        if ((attribute = limitNode.GetAttribute("maxtransactionscount")) != string.Empty) this.SetAttribute("/beline/conf/global/limit", "maxtransactionscount", attribute);
        if ((attribute = limitNode.GetAttribute("defaulttimeout")) != string.Empty) this.SetAttribute("/beline/conf/global/limit", "defaulttimeout", attribute);
        // process paths node
        XmlElement pathsNode = (XmlElement)configNode["paths"];
        if ((attribute = pathsNode.GetAttribute("templates")) != string.Empty) this.SetAttribute("/beline/conf/global/paths", "templates", attribute);
        if ((attribute = pathsNode.GetAttribute("fifos")) != string.Empty) this.SetAttribute("/beline/conf/global/paths", "fifos", attribute);
        // process bus node
        XmlElement busNode = (XmlElement)configNode["bus"];
        if ((attribute = busNode.GetAttribute("bustype")) != string.Empty) this.SetAttribute("/beline/conf/global/bus", "bustype", attribute);
        
        // process user's private configuration
        XmlElement privateNode = (XmlElement)configNode["configuration"];
        ImportConfigurationElement(privateNode);
      }
      catch (Exception e)
      { // probably bad XML format
        Console.WriteLine("Error: " + e.Message);
        return;
      }
    }
    
    /// <summary>
    /// Read Xml configuration from given file and merge it to a configuration stored in this
    /// object. This is specialized to module's configuration
    /// </summary>
    /// <param name="aPath">The whole path to a module's configuration file.</param>
    public void ImportConfig (string aPath)
    { 
      // check if the specified file exists
      FileInfo lfi = new FileInfo(aPath);
      if (!lfi.Exists) return;  // file not found => can't import another info
      
      // temporary xml document
      XmlDocument tmpXml = new XmlDocument();
      tmpXml.Load(aPath);

      try
      {
        string attribute;
        XmlElement configNode = (XmlElement)(tmpXml.DocumentElement["conf"]["module"]);
        if ((attribute = configNode.GetAttribute("runcommand")) != string.Empty) this.SetAttribute("/beline/conf/module", "runcommand", attribute);

        XmlElement runNode = (XmlElement)configNode["run"];
        if ((attribute = runNode.GetAttribute("timeout")) != string.Empty) this.SetAttribute("/beline/conf/module/run", "timeout", attribute);
        if ((attribute = runNode.GetAttribute("exclusive")) != string.Empty) this.SetAttribute("/beline/conf/module/run", "exclusive", attribute);

        // process user's private configuration
        XmlElement privateNode = (XmlElement)configNode["configuration"];
        ImportConfigurationElement(privateNode);
      }
      catch (Exception e)
      { // probably bad XML format
        Console.WriteLine("Error: " + e.Message);
        return;
      }
//      // go through all elements (and its attributes and set it to inner value)
//      MergeConfig("", tmpXml);
    }
    
    /// help method used in LoadConfig method in recurent read of a loaded configuration file
    /// <attrib aXPath="Way to the parrent of this node"></attrib>
    /// <attrib aNode="XML element which should be processed"></attrib>
//    public void MergeConfig (string aXPath, XmlNode aNode)
//    {
//      aXPath += "/" + aNode.LocalName;
//      
//      // precess element's CDATA value
//      if (aNode.InnerText != "")
//      {
//        // set new value to inner document
//        this[aXPath] = new BString("", aNode.InnerText); 
//      }
//      
//      // process element's attributes
//      foreach (XmlAttribute attribute in aNode.Attributes)
//      {
//        // copy the value of an element to the inner XML object
//        this[aXPath + "[" + attribute.LocalName + "]"] = new BString("", attribute.Value);
//      }
//      
//      // process children of this element
//      foreach (XmlNode element in aNode)
//      {
//        MergeConfig(aXPath, element);
//      }
//    }
    
    /// <summary>
    /// checks if selected items exist (has this element (attribute))
    /// return the count of elements returned by a XPath query
    /// </summary>
    /// <param name="aXPath">A XPath to searched elements.</param>
    public int ExistsElement (string aXPath)
    {
      XmlNodeList nodeList = innerValue.SelectNodes(aXPath);
      return nodeList.Count;
    }
    
    ///<summary>Save configuration file stored in this instance to given file</summary>
    ///<param name="aPath">Full path to the destination file. If it is null so save to the default local configuration file (ie. ~/.libbeline/modules/projectname.conf)</param>
    public void SaveConfigToFile (string aPath)
    {
      if (aPath == null) aPath = Path.Combine(BConfigManager.LocalModulesPath, fileName);
      
      DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(aPath));
      if (!directory.Exists)
      { 
        directory.Create();
      }
      
      innerValue.Save(aPath);
    }
    
    /// <summary>Import configuration given by XmlElement (and it's subtree) to a user defined configuration.</summary>
    /// <param name="configuration">The "Configuration" element of XmlConfiguration</param>
    private void ImportConfigurationElement(XmlElement configuration)
    {
      string attribute;
      if (configuration == null) return;  // if an element doesn't exits nothing to do
      
      try
      {
        foreach (XmlNode foldNode in configuration.ChildNodes)
        {
          if (foldNode.NodeType != XmlNodeType.Element || foldNode.LocalName.ToLower() != "fold") continue;
          string foldLabel = ((XmlElement)foldNode).GetAttribute("label");
          foreach (XmlNode headingNode in foldNode.ChildNodes)
          {
            if (headingNode.NodeType != XmlNodeType.Element || headingNode.LocalName.ToLower() != "heading") continue;
            string headingText = ((XmlElement)headingNode).GetAttribute("text");
            foreach (XmlNode bcfgNode in headingNode.ChildNodes)
            {
              if (bcfgNode.NodeType != XmlNodeType.Element || bcfgNode.LocalName.ToLower() != "bcfgitem") continue;
              string bcfgLabel = ((XmlElement)bcfgNode).GetAttribute("label");

              XmlNode itemNode = bcfgNode.FirstChild;
              if (itemNode.NodeType != XmlNodeType.Element) continue;

              XmlElement itemElement = (XmlElement)itemNode;
              string bcfgType = itemElement.LocalName;
              string wayToItem = "/beline/conf/module/configuration/fold[@label='"+foldLabel+"']/heading[@text='"+headingText+"']/bcfgitem[@label='"+bcfgLabel+"']/"+bcfgType;
              switch (bcfgType)
              {
                case "bool":
                  if ((attribute = itemElement.GetAttribute("value")) != string.Empty) this.SetAttribute(wayToItem, "value", attribute);
                  break;
                case "int":
                  if ((attribute = itemElement.GetAttribute("value")) != string.Empty) this.SetAttribute(wayToItem, "value", attribute);
                  break;
                case "select":
                  if ((attribute = itemElement.GetAttribute("selected")) != string.Empty) this.SetAttribute(wayToItem, "selected", attribute);
                  break;
                case "string":
                  if ((attribute = itemElement.GetAttribute("value")) != string.Empty) this.SetAttribute(wayToItem, "value", attribute);
                  break;
              }
            } // foreach bcfg items
          } // foreach headings
        } // foreach folds
      }
      catch (Exception e)
      { // probably bad XML format
        Console.WriteLine("Error in private configuration: " + e.Message);
        LibBeline.GetInstance().LogManager.Log("Error in private configuration: " + e.Message, BEnumLogLevel.Debug);
        return;
      }
    }
  } // class BConfigItem
} // namespace  
