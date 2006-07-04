using System;

namespace LibBeline {
  /// <summary>Send messages via mail. <c>This class is not implemented yet.</c></summary>
  public sealed class BLogManagerMail : BLogManager {

    #region Attributes
    /// <summary>Emailová adresa, která je upozorněna (lze ji měnit)</summary>
    public string EmailAddress  
    {
      get { return emailAddress  ; }
    }
    private string emailAddress;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aEmail"></param>
    public BLogManagerMail (string aEmail)
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
    /// Send now the message (not implemented yet).
    /// </summary>
    public void Send ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// 
    /// </summary>
    ~BLogManagerMail ()
    {
      throw new System.Exception ("Not implemented yet!");
    }

  } // class BLogManagerMail
} // namespace 

