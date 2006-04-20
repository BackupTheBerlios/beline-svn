using System;

namespace LibBeline {
  /// Určuje jestli se jedná o modul řídící (zobrazovací), nebo jen výkonný
  public enum BEnumSystem {

    // Attributes
    /// Pro řídící moduly
    master = 1,
    /// Pro výkonné moduly
    slave = 2
  } // enum BEnumSystem
} // namespace

