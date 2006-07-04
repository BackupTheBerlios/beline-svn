using System;

namespace LibBeline {
  /// <summary>Enumeration messages managers types.</summary>
  public enum BEnumLogManagerType {
    /// <summary>
    /// Store messages to a file.
    /// </summary>
    File = 1,
    /// <summary>
    /// Send messages via e-mail.
    /// </summary>
    Mail = 2,
    /// <summary>
    /// Store messages to syslog.
    /// </summary>
    Syslog = 3
  } // class BEnumLogManagerType
} // namespace

