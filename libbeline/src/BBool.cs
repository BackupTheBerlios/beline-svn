using System;

namespace LibBeline {
  /// <summary>
  /// Part of BValueType hierarchy. This class represents boolean value and offer functions around this.
  /// </summary>
  /// <remarks>Instances of this type have values of either true or false.</remarks>
  public class BBool : BValueType {
    // Attributes
    /// 
    private bool innerValue;
  
    /// <summary>
    /// Create instance of class from classic .Net bool value
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BBool (string name, bool aValue) : base(name, BEnumType.BBool)
    {
      innerValue=aValue;
    }
    
    /// <summary>
    /// Create instance of class from string
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BBool (string name, string aValue) : base(name, BEnumType.BBool)
    {
      innerValue=(aValue=="true" ? true : false);
    }
  
    /// <summary>
    /// Convert the inner value to the ekvivalent .Net representation.
    /// </summary>
    /// <returns>Ekvivalent .Net representation</returns>
    public virtual bool ToBool ()
    {
      return innerValue;
    }
    
    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>The "True" or "False" value</returns>
    public override string ToString ()
    {
      if (innerValue) 
      {
        return "True";
      } else
      {
        return "False";
      }
    }
    
    /// <summary>
    /// Overrided. Return BEnumType.BBool.
    /// </summary>
    /// <returns></returns>
    public override BEnumType GetBType ()
    {
      return type;
    }
  
  } // class BBool
}  // namespace

