using System;

namespace LibBeline {
  /// Komunikační třída pro komunikaci přes D-BUS
  public class BDBusManager : BBusManager {

    // Attributes
    /// 
    private string name;
    public string Name
    {
      get { return name; }
    }
    
    // 
    public BDBusManager (string aName)
    {
    }
    // 
    public override BEnumBusManagerType GetManagerType ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public override void Send (BMessage aMessage)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public override BMessage Receive ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    ~BDBusManager ()
    {
    }

  } // class BDBusManager
} // namespace

