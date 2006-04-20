using System;


namespace LibBeline {
  /// 
  public class BInteger : BValueType {

    // Attributes
    /// 
    private int innerValue;

    public BInteger (string aName, int aValue) : base(aName, BEnumType.BInteger)
    {
      innerValue=aValue;
    }
    
    public BInteger (string aName, string aValue) : base(aName, BEnumType.BInteger)
    {
      innerValue=Convert.ToInt32(aValue);
    }
    
    public virtual int ToInt ()
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

  } // class BInteger
} // namespace

