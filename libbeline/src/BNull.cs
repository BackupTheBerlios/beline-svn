using System;

namespace LibBeline {
  /// <summary>Class representing Null value.</summary>
  public class BNull : BValueType {

    public BNull (string aName) : base(aName, BEnumType.BNull)
    {}

    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>Empty string.</returns>
    public override string ToString ()
    {
      return "";
    }

    /// <summary>
    /// Overrided. Return BEnumType.BNull.
    /// </summary>
    /// <returns></returns>
    public override BEnumType GetBType ()
    {
      return BEnumType.BNull;
    }
  } // class BNull
} // namespace

