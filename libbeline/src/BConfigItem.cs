using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace LibBeline {
  /// Udržuje konfiguraci právě jednoho modulu, nebo globální konfiguraci
  public sealed class BConfigItem : BItem {
  
    // Attributes
    /// Obsah rozparsovaného dokumentu
    private XmlDocument innerValue;
    /// Way to the first XML document with a configuration
    public string File
    {
      get {return fileName; }
    }
    private string fileName;
    
    // Vytvori konfiguraci podle Xml v zadanem souboru
	  // <exception> XmlException, FileNotFoundException </exception>
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
      
      fileName = aPath;
    }
    
    /// gets/sets the first found value according to a XPath value
    /// if user want to set BNull value, selected items will be deleted
    public BValueType this[string aXPath]
    {
      get
      {       
        string wayToElement, wayToAttribute;
        if (aXPath.IndexOf('[') == -1)
        { // it has only way to element (ie. /way/to/element)
          wayToElement = aXPath;
          wayToAttribute = "";
        }
        else
        { // it has way to specified attribute in element (ie. /way/to/element[attribute])
          wayToElement = aXPath.Substring(0, aXPath.IndexOf('['));
          wayToAttribute= aXPath.Substring(aXPath.IndexOf('[') + 1, aXPath.Length - aXPath.IndexOf('[') - 2);
        }
        
        if (wayToAttribute == "")
        { // he wan't to return element's CData
          XmlNodeList nodeList = innerValue.SelectNodes(wayToElement);
          if (nodeList.Count == 0)
            return new BNull(aXPath); // nothing found => return BNull value
          else
            return new BString(aXPath, nodeList[0].InnerText.Trim());  // return first CData element
        }
        
        foreach (XmlNode node in innerValue.SelectNodes(wayToElement))
        { // go throught all found elements and find the one with wayToAttribute attribute
          if (node.NodeType != XmlNodeType.Element) continue;
          XmlElement element = (XmlElement)node;
          
          if (element.HasAttribute(wayToAttribute))
          {
            string attributeValue = element.GetAttribute(wayToAttribute);
            BString retval = new BString(wayToAttribute, attributeValue);
            return (BValueType)retval;
          }
        }
        // none of given XPath were found
        return new BNull(aXPath);
      }
      
	  	set
	  	{
	  	  string wayToElement, wayToAttribute;
        if (aXPath.IndexOf('[') == -1)
        { // it has only way to element (ie. /way/to/element)
          wayToElement = aXPath; 
          wayToAttribute = "";
        }
        else
        { // it has way to specified attribute in element (ie. /way/to/element[attribute])
          wayToElement = aXPath.Substring(0, aXPath.IndexOf('[') - 1);
          wayToAttribute= aXPath.Substring(aXPath.IndexOf('['), aXPath.Length);
        }
        
        //// inner value temporary converted to BObject (for simpler work)
        //BObject tmpBObject = BObject.Deserialize(innerValue);
        // way splitted to array of elements
        string[] tmpElements = wayToElement.Split('/');
        // pointer to inner value's elements
        XmlElement tmpInnerValue = null, tmpInnerValue2 = innerValue.DocumentElement;
        
        foreach (string elementName in tmpElements)
        {
          tmpInnerValue = tmpInnerValue2; // jump to next level
          if ((tmpInnerValue2 = tmpInnerValue[elementName]) == null)
          { // element with given name not found
            XmlDocument tmpDocument = new XmlDocument();
            tmpDocument.CreateElement(elementName);
            tmpInnerValue2 = (XmlElement)tmpInnerValue.InsertBefore(tmpDocument, null);
          }
        } // now in tmpInnerValue2 is the last element in XPath
        
        if (wayToAttribute != "")
        { // set last element's CData
          if (value.GetBType() == BEnumType.BNull)
          { // remove element
            if (tmpInnerValue == null) return; // can't set there
            tmpInnerValue.RemoveChild(tmpInnerValue);
          }
          else 
          { // set string value as an element's CData
            tmpInnerValue2.InnerText = value.ToString();
          }
        }
        else
        { // set attribute value
          if (value.GetBType() != BEnumType.BNull)
          {
            tmpInnerValue2.SetAttribute(wayToAttribute, value.ToString());
          }
          else
          { 
            tmpInnerValue2.RemoveAttribute(wayToAttribute);
          }
        }
	  	} // setter 
    }
    
    /// execute XPath query and return first result as XML
    public XmlNode GetXmlNode (string aXPath)
    {
      string wayToElement, wayToAttribute;
      if (aXPath.IndexOf('[') == -1)
      { // it has only way to element (ie. /way/to/element)
        wayToElement = aXPath;
        wayToAttribute = "";
      }
      else
      { // it has way to specified attribute in element (ie. /way/to/element[attribute])
        wayToElement = aXPath.Substring(0, aXPath.IndexOf('[') - 1);
        wayToAttribute= aXPath.Substring(aXPath.IndexOf('['), aXPath.Length);
      }

      // Find proper element
      string lastElementName = wayToElement.Substring(wayToElement.LastIndexOf('/'));

      if (wayToAttribute == "")
      { // he wan't to return element's CData
        XmlNodeList nodeList = innerValue.SelectNodes(wayToElement);
        if (nodeList.Count == 0)
          return null;
        else
          return nodeList[0];  // return whole first element
      }

      foreach (XmlNode element in innerValue.SelectNodes(wayToElement))
      { // go throught all found elements and find the one with wayToAttribute attribute
        if (element[lastElementName].HasAttribute(wayToAttribute))
        {
          return element[lastElementName].GetAttributeNode(wayToAttribute);
        }
      }
      // none of given XPath were found
      return null;
    }
    
    // execute XPath query and return all found elements
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
    
    // return URI to help to first selected item 
    public string GetHtmlHelp (string aXPath)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    
    // return hint to first selected item 
    public string GetHint (string aXPath)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    
    // 
    public void LoadConfig (string aPath)
    {
      // check if the specified file exists
      FileInfo lfi = new FileInfo(aPath);
      if (!lfi.Exists) return;  // file not found => can't import another info
      
      // temporary xml document
      XmlDocument tmpXml = new XmlDocument();
      tmpXml.Load(aPath);
      
      // go through all elements (and its attributes and set it to inner value)
      MergeConfig("", tmpXml);
    }
    
    /// help method used in LoadConfig method in recurent read of a loaded configuration file
    /// <attrib aXPath="Way to the parrent of this node"></attrib>
    /// <attrib aNode="XML element which should be processed"></attrib>
    public void MergeConfig (string aXPath, XmlNode aNode)
    {
      aXPath += "/" + aNode.LocalName;
      
      // precess element's CDATA value
      if (aNode.InnerText != "")
      {
        // set new value to inner document
        this[aXPath] = new BString("", aNode.InnerText); 
      }
      
      // process element's attributes
      foreach (XmlAttribute attribute in aNode.Attributes)
      {
        // copy the value of an element to the inner XML object
        this[aXPath + "[" + attribute.LocalName + "]"] = new BString("", attribute.Value);
      }
      
      // process children of this element
      foreach (XmlNode element in aNode)
      {
        MergeConfig(aXPath, element);
      }
    }
    
    /// checks if selected items exist (has this element (attribute))
    /// return the count of elements returned by a XPath query
    public int ExistsElement (string aXPath)
    {
      XmlNodeList nodeList = innerValue.SelectNodes(aXPath);
      return nodeList.Count;
    }
    
    ///<summary>Save configuration file stored in this instance to given file</summary>
    ///<param aPath="Full path to the destination file. If it is null so save to the default local configuration file (ie. ~/.libbeline/modules/projectname.conf)"></param>
    public void SaveConfigToFile (string aPath)
    {
      if (aPath == null) aPath = Path.Combine(BConfigManager.LocalModulesPath, fileName);
      innerValue.Save(aPath);
    }
  } // class BConfigItem
} // namespace  
