using System;

namespace LibBeline {
  /// Třída zastřešující string
  public class BString : BValueType {

    // Attributes
    /// 
    private string innerValue;

    public BString (string aName, string aValue) : base(aName, BEnumType.BString)
    {
      innerValue=aValue;
    }

    public override string ToString ()
    {
      return innerValue;
    }
  } // class BString
} // namespace

