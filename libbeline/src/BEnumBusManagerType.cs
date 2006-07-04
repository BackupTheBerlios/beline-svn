using System;

namespace LibBeline {
  /// <summary>Enumeration of all existing BUS managers.</summary>
  public enum BEnumBusManagerType {

    /// <summary>
    /// BUS manager for communication using D-BUS.
    /// </summary>
    DBUS = 1,
    /// <summary>
    /// BUS manager for communication using Corba.
    /// </summary>
    Corba = 2,
    /// <summary>
    /// BUS manager for communication using unix Fifos.
    /// </summary>
    Fifo = 3,
    /// <summary>
    /// Not specified type of BUS manager.
    /// </summary>
    Other = 99
  } // class BEnumBusManagerType
} // namespace 

