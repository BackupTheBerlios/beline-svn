using System;

namespace LibBeline {
  /// Komunikační třída pro komunikaci přes D-BUS
  public class BCorbaManager : BBusManager {

    // 
    public BCorbaManager (string aName)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public override BEnumBusManagerType GetManagerType ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    
    public override void Send(BMessage aMessage)
    {
      throw new System.Exception("Not implemented yet!");
    }
    
    public override BMessage Receive()
    {
      throw new System.Exception("Not implemented yet!");
    }

  } // class BCorbaManager
} // namespace

