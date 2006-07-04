using System;

namespace LibBeline {
  /// <summary>Status of run of a module</summary>
  public enum BEnumStatus {
    /// <summary>
    /// Module is sleeping.
    /// </summary>
    Sleep = 1,
    /// <summary>
    /// Module is waiting to run.
    /// </summary>
    Wait = 2,
    /// <summary>
    /// Module is now running.
    /// </summary>
    Running = 3,
    /// <summary>
    /// Module is idle.
    /// </summary>
    Idle = 4
  } // enum BEnumType
} // namespace
