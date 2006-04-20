using System;

namespace LibBeline {
  /// Class represents Null value
  public class BNull : BValueType {

    public BNull (string aName) : base(aName, BEnumType.BNull)
    {}

    public override string ToString ()
    {
      return "";
    }
  } // class BNull
} // namespace

