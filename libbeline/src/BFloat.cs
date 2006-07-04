using System;

namespace LibBeline {
  /// Number with floating decimal point
  public class BFloat : BValueType {

    // Attributes
    /// 
    private double innerValue;
    
    /// <summary>
    /// Create instance of class from classic .Net float value
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BFloat (string name, float aValue) : base(name, BEnumType.BFloat)
    {
      innerValue=aValue;
    }
    
    ///<summary>Create instance from string value.</summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    ///<exception>ArgumentException, FormatException, OverflowException</exception>
    public BFloat (string name, string aValue) : base(name, BEnumType.BFloat)
    {
      innerValue=System.Convert.ToDouble(aValue);
    }    

    /// <summary>
    /// Convert the inner value to the ekvivalent .Net representation.
    /// </summary>
    /// <returns>Ekvivalent .Net representation</returns>
    public virtual double ToFloat ()
    {
      return innerValue;
    }

    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>The string representation of float value</returns>
    public override string ToString ()
    {
      return innerValue.ToString();
    }
   
    /// <summary>
    /// Overrided. Return BEnumType.BFloat.
    /// </summary>
    /// <returns></returns>
    public override BEnumType GetBType ()
    {
      return type;
    }

  }
}

