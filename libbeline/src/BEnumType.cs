using System;

namespace LibBeline {
  /// Výčtový typ s použitými typy v XML zprávách
  public enum BEnumType {

    // Attributes
    BNull = 1,
    /// Řetězce
    BString = 2,
    /// Celé číslo
    BInteger = 3,
    /// 
    BFloat = 4,
    /// 
    BBool = 5,
    /// Can represent XML document
    BObject = 6,
    BVersion = 7
  } // enum BEnumType
} // namespace

