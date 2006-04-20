using System;

namespace LibBeline {
  /// Number with floating decimal point
  public class BFloat : BValueType {

    // Attributes
    /// 
    private double innerValue;
    
    public BFloat (string aName, float aValue) : base(aName, BEnumType.BFloat)
    {
      innerValue=aValue;
    }
    
    ///<exception>ArgumentException, FormatException, OverflowException</exception>
    public BFloat (string aName, string aValue) : base(aName, BEnumType.BFloat)
    {
      innerValue=System.Convert.ToDouble(aValue);
    }    

    public virtual double ToFloat ()
    {
      return innerValue;
    }

    public override string ToString ()
    {
      return innerValue.ToString();
    }
   
    public override BEnumType GetBType ()
    {
      return type;
    }

  }
}

