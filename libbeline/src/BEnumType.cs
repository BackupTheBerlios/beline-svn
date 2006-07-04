using System;

namespace LibBeline {
  /// <summary>Enumeration of BValueType subtypes.</summary>
  public enum BEnumType {
    /// <summary>
    /// Null value.
    /// </summary>
    BNull = 1,
    /// <summary>
    /// String value.
    /// </summary>
    BString = 2,
    /// <summary>
    /// Integer value.
    /// </summary>
    BInteger = 3,
    /// <summary>
    /// Floating number value.
    /// </summary>
    BFloat = 4,
    /// <summary>
    /// Boolean value.
    /// </summary>
    BBool = 5,
    /// <summary>
    /// Structured object. Can represent XML configuration.
    /// </summary>
    BObject = 6,
    /// <summary>
    /// Version in format x.y.z.w
    /// </summary>
    BVersion = 7
  } // enum BEnumType
} // namespace

