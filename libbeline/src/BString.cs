using System;

namespace LibBeline {
  /// Třída zastřešující string
  public class BString : BValueType {

    // Attributes
    /// 
    private string innerValue;

    /// <summary>
    /// Create instance of class from string value
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BString (string name, string aValue) : base(name, BEnumType.BString)
    {
      innerValue=aValue;
    }

    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>The string representation of inner value</returns>
    public override string ToString ()
    {
      return innerValue;
    }
  } // class BString
} // namespace

