using System;
using System.Xml;

namespace LibBeline {
  /// Abstraktní třída zastřešující typy
  public class BValueType {
  
    // Attributes
    /// type of the object
    protected BEnumType type;
    /// 
    protected string name;
    public string Name 
    {
      get { return name; }
    }
  
    public virtual BEnumType GetBType ()
    {
      return type;
    }
  
    // derived from Object
    // public abstract string ToString ();
  
    protected BValueType (string aName, BEnumType aType)
    {
      name = aName;
      type = aType;
    }
    
    public static int Deep = 0;
    
    /// convert value of BValueType object (some of its children) to xml document
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
    
    /// convert value serialized in XML to BValueType object (some of its children)
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

