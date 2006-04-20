using System;

namespace LibBeline {
  /// Faktory metoda tvořící požadovaný manažer
  public static class BLogManagerFactory {

    // 
    public static BLogManager CreateLogManager (BEnumLogManagerType aLogManagerType, string aParameter)
    {
      switch (aLogManagerType)
      {
        case BEnumLogManagerType.File:
          return new BLogManagerFile(aParameter);
        case BEnumLogManagerType.Mail:
          return new BLogManagerMail(aParameter);
        default: // BEnumLogManagerType.Syslog:
          return new BLogManagerSyslog();
      }
    }

  } // class BLogManagerFactory
} // namespace

