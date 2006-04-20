using System;
using System.Text;
using System.Xml;
using System.Collections;


namespace LibBeline {
  /// Public object
  public class BObject : BValueType {
  
    // Attributes
    /// Array with inner representation of objects
    private ArrayList innerValue;
  
    public BObject (string aName, ArrayList aValue) : base(aName, BEnumType.BObject)
    {
      innerValue=aValue;
    }
  
    public virtual ArrayList GetValue()
    {
      return innerValue;
    }
    
    /// Find first value of a stored object with given name 
    public virtual BValueType GetValue(string aName)
    {
      BValueType retval = null;
      
      foreach (BValueType hodnota in innerValue)
      {
        if (hodnota.Name == aName) 
        { // store found value to return value
          retval = hodnota;
          break;  // found, so finish for cyclus
        }
        
        if (hodnota.GetBType() == BEnumType.BObject)
        { // search recursive (deep first searching)
          retval=((BObject)hodnota).GetValue(aName);
        }
          
        // if searched value has found stop cycling
        if (retval != null)
          break;
      }

      return retval;
    }
    
    public override string ToString ()
    {
      StringBuilder tmpSb = new StringBuilder();
      // return all inner values concatenated together
      foreach (BValueType objekt in innerValue)
        tmpSb.Append(objekt.ToString() + '\n');
        
      return tmpSb.ToString();
    }
  
    public override BEnumType GetBType ()
    {
      return type;
    }
  } // class BObject
} // namespace   
