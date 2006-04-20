using System;
using System.Collections;

namespace LibBeline {
  /// Class communicating with modules (slave part)
  public class BSlaveServiceManager {

    // Attributes
    /// 
    private static BSlaveServiceManager singletonInstance;
    
    /// Collection of observable classes (the should by announced when message arrive)
    ArrayList observers;
    
    BBusManager busManager;
     
    // 
    public static BSlaveServiceManager GetInstance ()
    {
      // only slave can make instance of BSlaveServiceManager
      if (LibBeline.GetInstance().System != BEnumSystem.slave) return null;
      
      if (singletonInstance == null)
      {
        singletonInstance = new BSlaveServiceManager();
      }
      return singletonInstance;
    }
    // 
    public void AttachObserver (BObservable aClass)
    {
      observers.Add(aClass);
    }
    // 
    public void DetachObserver (int aClass)
    {
      if (aClass < 0 || aClass >= observers.Count) return;  // bad number
      
      observers.Remove(aClass);
    }
    // 
    public void CleanObservers ()
    {
      observers.Clear();
    }
    // 
    public void SendCommand (BMessage message)
    {
      busManager.Send(message);
    }
    // 
    public void SendMessage (int aComplete, string aMessage, string destinationPID)
    {
      BMessage msg = BMessage.CreateStatus(aComplete, aMessage);
      
      busManager.Send(msg);
    }
    
    /// signal from BusManager that some message arrived
    public void ArrivedMessage(BMessage aMessage)
    {
      // tell everybody that the message arrived
      foreach (BObservable observer in observers)
      {
        observer.MessageArrived(aMessage);
      }
    }
    
    //
    private BSlaveServiceManager ()
    {
      observers = new ArrayList();
      busManager = BBusManagerFactory.CreateBusManager(null);
    }

  } // class BSlaveServiceManager
} // namespace
