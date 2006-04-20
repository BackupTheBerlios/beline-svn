using System;

namespace LibBeline {
  /// 
  public sealed class BLogManagerMail : BLogManager {

    // Attributes
    /// Emailová adresa, která je upozorněna (lze ji měnit)
    public string EmailAddress  
    {
      get { return emailAddress  ; }
    }
    private string emailAddress;
    // 
    public BLogManagerMail (string aEmail)
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
    ~BLogManagerMail ()
    {
      throw new System.Exception ("Not implemented yet!");
    }

  } // class BLogManagerMail
} // namespace 

