using System;

namespace LibBeline {
  /// <summary>Enumeration of all levels of logging messages.</summary>
  public enum BEnumLogLevel {
    /// <summary>
    /// Nothing will be logged.
    /// </summary>
    Nothing = 10,
    /// <summary>
    /// Serious error causes crash of application.
    /// </summary>
    ErrorEvent = 9,
    /// <summary>
    /// Warning events in applicaton.
    /// </summary>
    WarningEvent = 8,
    /// <summary>
    /// Some other events in application.
    /// </summary>
    InfoEvent = 7,
    /// <summary>
    /// Messages transported via bus.
    /// </summary>
    Messages = 6,
    /// <summary>
    /// All actions except debug messages.
    /// </summary>
    AllActions = 5,
    /// <summary>
    /// Including debug messages.
    /// </summary>
    Debug = 4
  } // class BEnumLogLevel
} // namespace

