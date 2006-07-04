using System;
using System.Xml;

namespace LibBeline {
  /// <summary>Abstract class covering the whole hierarchy of B-types.</summary>
  public class BValueType {

    /// <summary>Type of the object.</summary>
    protected BEnumType type;
    /// 
    protected string name;
    /// <summary>
    /// Name of the object (use when looking for or storing in XML configuration)
    /// </summary>
    public string Name 
    {
      get { return name; }
    }
  
    /// <summary>
    /// Return type of object (overrided byl childs)
    /// </summary>
    /// <returns></returns>
    public virtual BEnumType GetBType ()
    {
      return type;
    }
  
    // derived from Object
    // public abstract string ToString ();
  
    /// <summary>
    /// Create new instance used by children.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    protected BValueType (string name, BEnumType type)
    {
      this.name = name;
      this.type = type;
    }
    
    /// <summary>
    /// Actual deep in object hierarchy (in BObject)
    /// </summary>
    public static int Deep = 0;
    
    /// <summary>Convert value of BValueType object (some of its children) to xml document.</summary>
    /// <param name="aValue">Value to serialize.</param>
    /// <exception>XmlException</exception>
    public static XmlNode Serialize(BValueType aValue)
    {
      // pokud je hloubka zanoreni prilis velika ukonci zanorovani, aby nedoslo k preteceni
      if (Deep >= 100) return null;
      Deep++;
      
      XmlDocument retval = new XmlDocument();
      switch (aValue.GetBType())
      {
        case BEnumType.BBool:         
          retval.LoadXml("<bool name=\"" + aValue.Name + "\" value=\"" + aValue.ToString() + "\" />");
          break;
        case BEnumType.BFloat:
          retval.LoadXml("<float name=\"" + aValue.Name + "\" value=\"" + aValue.ToString() + "\" />");
          break;
        case BEnumType.BInteger:
          retval.LoadXml("<int name=\"" + aValue.Name + "\" value=\"" + aValue.ToString() + "\" />");
          break;
        case BEnumType.BString:
          string tmpStr = String.Format(@"<bool name=""{1}""><text lang=""en""><![CDATA[{2}]]></text></string>",
                                        aValue.Name, aValue.ToString());
          retval.LoadXml(tmpStr);
          break;
        case BEnumType.BVersion:
          retval.LoadXml("<version name=\"" + aValue.Name + "\" value=\"" + aValue.ToString() + "\" />");
          break;
        case BEnumType.BObject:
          // create this element
          retval.LoadXml("<object/>");
          XmlAttribute attribute = retval.CreateAttribute("name");
          attribute.Value = aValue.Name;
          retval.DocumentElement.Attributes.Append(attribute);
      
          foreach (BValueType innerValue in ((BObject)aValue).GetValue())
          {
            XmlNode node = Serialize(innerValue);
            if (node != null) retval.ImportNode(node, true);
          }
          break;
        default:
          // BNull
          retval = null;
          break;
      }
      
      Deep--;
      return retval.DocumentElement;
    }
    
    /// <summary>Convert value serialized in XML to BValueType object (some of its children)</summary>
    /// <param name="aValue">Value to deserialize.</param>
    /// <exception>XmlException</exception>
    public static BValueType Deserialize(XmlNode aValue)
    {
      // pokud je hloubka zanoreni prilis velika ukonci zanorovani, aby nedoslo k preteceni
      if (Deep >= 100) return null;
      Deep++;
      
      BValueType retval;
      XmlAttribute name = aValue.Attributes["name"];
      XmlAttribute hodn;
      switch (aValue.LocalName)
      {
        case "bool":
          hodn = aValue.Attributes["value"];
          retval = new BBool(name.Value, hodn.Value);
          break;
        case "float":
          hodn = aValue.Attributes["value"];
          try
          {
            retval = new BFloat(name.Value, hodn.Value);
          }
          catch (Exception e)
          {
            throw new XmlException("Error in XML document in tag \"" + aValue.Name + "\" : \n" + e.Message);
          }
          break;
        case "int":
          hodn = aValue.Attributes["value"];
          try
          {
            retval = new BInteger(name.Value, hodn.Value);
          }
          catch (Exception e)
          {
            throw new XmlException("Error in XML document in tag \"" + aValue.Name + "\" : \n" + e.Message);
          }
          break;
        case "string":
          XmlNode text = aValue.FirstChild;
          if (text == null || text.LocalName.ToLower() != "text" || text.NodeType != XmlNodeType.Element)
            throw new XmlException("Error in XML document in tag \"" + aValue.Name + 
                                  "\" : \nElement string does not contain element text.");
                                  
          XmlNode cdata = text.FirstChild;
          if (cdata == null || cdata.NodeType != XmlNodeType.CDATA)
            throw new XmlException("Error in XML document in tag \"" + aValue.Name + 
                                  "\" : \nElement string does not contain CDATA value.");
                                  
          retval = new BString(name.Value, cdata.Value);
          break;
        case "version":
          hodn = aValue.Attributes["value"];
          try
          {
            retval = new BVersion(name.Value, hodn.Value);
          }
          catch (Exception e)
          {
            throw new XmlException("Error in XML document in tag \"" + aValue.Name + "\" : \n" + e.Message);
          }
          break;
        case "object":
          System.Collections.ArrayList innerRetVal = new System.Collections.ArrayList(aValue.ChildNodes.Count);
          foreach (XmlNode node in aValue.ChildNodes)
          {
            BValueType objekt = Deserialize(node);
            if (objekt != null) innerRetVal.Add(objekt);
          }
          
          retval = new BObject(name.Value, innerRetVal);
          break;
        default:
          // something wrong
          retval=null;
          break;
      }
      
      Deep--;
      return retval;
    }
  } // class BValueType
} // namespace

