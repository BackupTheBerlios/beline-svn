using System;

namespace LibBeline {
  /// 
  public class BBool : BValueType {

    // Attributes
    /// 
    private bool innerValue;
  
    public BBool (string aName, bool aValue) : base(aName, BEnumType.BBool)
    {
      innerValue=aValue;
    }
    
    public BBool (string aName, string aValue) : base(aName, BEnumType.BBool)
    {
      innerValue=(aValue=="true" ? true : false);
    }
  
    public virtual bool ToBool ()
    {
      return innerValue;
    }
    
    // 
    public override string ToString ()
    {
      if (innerValue) 
      {
        return "true";
      } else
      {
        return "false";
      }
    }
    
    // 
    public override BEnumType GetBType ()
    {
      return type;
    }
  
  } // class BBool
}  // namespace

