using System;

namespace LibBeline {
  /// <summary>Class covering all logging managers.</summary>
  public abstract class BLogManager {
    /// <summary>
    /// Type of log manager for a simle identification.
    /// </summary>
    protected BEnumLogManagerType logManagerType;
    ///
    public BEnumLogLevel minimalLogLevel;
    /// <summary>
    /// Override. Overloaded. Write message to log.
    /// </summary>
    /// <param name="message">Message to write.</param>
    /// <param name="level">Level of message.</param>
    public abstract void Log (string message, BEnumLogLevel level);
    
    /// <summary>
    /// Override. Overloaded. Write message to log as error event.
    /// </summary>
    /// <param name="aException">Exception to write.</param>
    public abstract void Log (Exception aException);

    /// <summary>
    /// Protected constructor for derived classes
    /// </summary>
    protected BLogManager () {}
  } // class BLogManager
} // namespace
