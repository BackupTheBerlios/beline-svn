using System;
using System.Collections;

namespace LibBeline {
  /// Třída poskytující práci s knihovnou vizuálním prvkům
  public sealed class BMasterServiceManager {

    // Attributes
    /// Hodnota pouze pro čtení; true, pokud jsou v nějaké frontě (u nějakého mo
    public bool NewData  
    {
      set { newData  = value; }
    }
    private bool newData;
    
    /// Minimální úroveň zprávy, která se bude logovat (včetně)
    public BEnumLogLevel MinLogLevel  
    {
      set { minLogLevel  = value; }
      get { return minLogLevel  ; }
    }
    private BEnumLogLevel minLogLevel = BEnumLogLevel.WarningEvent;
    
    /// 
    private static BMasterServiceManager singletonInstance;
    
    /// Seznam objektů, které očekávají, že budou v případě příchozí zpráv
    private ArrayList observers;

    public static BMasterServiceManager GetInstance ()
    {
      // slave shouldn't make instance of BMasterServiceManager
      if (LibBeline.GetInstance().System != BEnumSystem.master) return null;
      
      if (singletonInstance == null)
      {
        singletonInstance = new BMasterServiceManager();
      }
      
      return singletonInstance;
    }

    public void AttachObserver (BObservable aClass)
    {
      observers.Add(aClass);
    }

    public void DetachObserver (BObservable aClass)
    {
      int i=0; // iterator
      
      foreach (BObservable observer in observers)
      {
        if (((Object)observer).Equals(aClass))
        {
          observers.RemoveAt(i);
          i--; // repare iterator's position
        }
        
        i++;
      } 
    }

    public void CleanObservers ()
    {
      observers.Clear();
    }
    
    public BModuleItem[] GetInstalledModules ()
    {
      return BModuleManager.GetInstance().GetAllModules();
    }

    public void StartModule (string aOID)
    {
      BModuleManager.GetInstance().Awake(aOID);
    }
    
    // 
    public void RunModule (string aOID, string aProcedure, ArrayList aParameters)
    {
      BModuleManager.GetInstance().Run(aOID, aProcedure, aParameters);
    }
    
    // 
    public void StopModule (string aOID)
    {
      BModuleManager.GetInstance().Stop(aOID);
    }
    
    // 
    public void EndModule (string aOID)
    {
      BModuleManager.GetInstance().End(aOID);
    }

    public void AwakeModule (string aOID)
    {
      BModuleManager.GetInstance().Awake(aOID);
    }

    public void SleepModule (string aOID, int aTime)
    {
      BModuleManager.GetInstance().Sleep(aOID, aTime);
    }

    public void SendMessage (string aOID, string aResult)
    {
      throw new Exception("Not implemented yet!");
    }

    public BConfigItem[] GetConfig (string aOID, int aLevel)
    {
      throw new System.Exception ("Not implemented yet!");
    }

    public void SetConfig (string aOID, BConfigItem aConfiguration)
    {
    }

    private BMasterServiceManager ()
    {
      observers = new ArrayList();
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

  } // class LibBeline
} // namespace 

