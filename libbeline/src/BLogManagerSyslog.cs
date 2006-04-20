using System;

namespace LibBeline {
  /// Logovací manažer, který umí logovat do syslogu
  public sealed class BLogManagerSyslog : BLogManager {

    // 
    public BLogManagerSyslog ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public override void Log (string aMessage, BEnumLogLevel aLevel)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public override void Log (Exception aException)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public void Send ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    ~BLogManagerSyslog ()
    {
      throw new System.Exception ("Not implemented yet!");
    }

  } // class BLogManagerSyslog
} // namespace

