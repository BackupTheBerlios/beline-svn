using System;

namespace LibBeline {
  /// Třída zajišťující ukládání do logu
  public abstract class BLogManager {

    // Attributes
    /// 
    protected BEnumLogManagerType logManagerType;
    // 
    public abstract void Log (string aMessage, BEnumLogLevel aLevel);
    
    // 
    public abstract void Log (Exception aException);
    
    /// Protected constructor for derived classes
    protected BLogManager () {}
  } // class BLogManager
} // namespace
