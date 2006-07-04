using System;
using System.Text;
using System.Xml;
using System.Collections;


namespace LibBeline {
  /// <summary>
  /// Structured object. Can represent XML configuration, public object or array.
  /// </summary>
  public class BObject : BValueType {
  
    // Attributes
    /// Array with inner representation of objects
    private ArrayList innerValue;
  
    /// <summary>
    /// Create instance of class from classic .Net ArrayList value
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BObject (string name, ArrayList aValue) : base(name, BEnumType.BObject)
    {
      innerValue=aValue;
    }
  
    public virtual ArrayList GetValue()
    {
      return innerValue;
    }
    
    /// <summary>
    /// Find first value of a stored object with given name 
    /// </summary>
    /// <param name="aName">Name of item stored in this object</param>
    /// <returns>Found value or BNull object.</returns>
    public virtual BValueType GetValue(string aName)
    {
      BValueType retval = new BNull("");
      
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
    
    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString ()
    {
      StringBuilder tmpSb = new StringBuilder();
      // return all inner values concatenated together
      foreach (BValueType objekt in innerValue)
        tmpSb.Append(objekt.ToString() + '\n');
        
      return tmpSb.ToString();
    }
  
    /// <summary>
    /// Overrided. Return BEnumType.BObject.
    /// </summary>
    /// <returns></returns>
    public override BEnumType GetBType ()
    {
      return type;
    }
  } // class BObject
} // namespace   
