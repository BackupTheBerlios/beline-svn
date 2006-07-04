using System;

namespace LibBeline {
  /// <summary>Store messages to syslog. <c>This class is not implemented yet.</c></summary>
  public sealed class BLogManagerSyslog : BLogManager {

    /// <summary>
    /// 
    /// </summary>
    public BLogManagerSyslog ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// Override. Overloaded. Write message to log.
    /// </summary>
    /// <param name="message">Message to write.</param>
    /// <param name="level">Level of message.</param>
    public override void Log (string message, BEnumLogLevel level)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// Override. Overloaded. Write message to log as error event.
    /// </summary>
    /// <param name="exception">Exception to write.</param>
    public override void Log (Exception exception)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// 
    /// </summary>
    ~BLogManagerSyslog ()
    {
      throw new System.Exception ("Not implemented yet!");
    }

  } // class BLogManagerSyslog
} // namespace

