using System;
using System.Xml;
using System.IO;
using System.Collections;

namespace LibBeline {
  /// Manažer udržující infomrace o modulech
  public sealed class BModules {
  
    // Attributes
    /// Konejner s instalovanými moduly (indexovaný IDčkem)
    private Hashtable modules;
    private int modulesCapacity;
  
    public BModuleItem[] GetModuleList ( )
    {
      BModuleItem[] retval = new BModuleItem[modules.Count];
      int i=0;
      foreach (DictionaryEntry entry in modules)
      {
        retval[i++] = (BModuleItem)entry.Value;
      }
      
      return retval;
    }
  
    /// <exception>NotFoundException</exception>
    public BModuleItem this [string aOID]
    {
      get
      {        
        return (BModuleItem)modules[aOID];
      }
      set
      {
        modules[aOID] = value;
      }
    }
  
    // <exception>FileNotFoundException, XmlException, Exception</exception>
    public string ImportModule (string aFileName)
    {
      if (modules.Count >= modulesCapacity) throw new Exception("Maximum number of modules imported.");
      BModuleItem tmpItem = new BModuleItem(aFileName);
      modules.Add(tmpItem.OID, tmpItem);
        
      return tmpItem.OID;
    }
  
    /// Unload module from memory
    /// <param name="aOID">Module's OID</param>
    public void DisableModule (string aOID)
    {
      // Stop all transactions
      foreach (BTransactionItem transaction in BModuleManager.GetInstance().GetAllTransactions())
      {
        if (transaction.ModuleOID == aOID)
          // it is transaction of this module, so stop it
          transaction.Destroy();
      }
      
      modules.Remove(aOID);
    }
  
    public BModules ()
    { 
      int maxModulesCount;
      try
      { // read maximum count of modules from global configuration
        maxModulesCount = Convert.ToInt32(BConfigManager.GetInstance().GlobalConf["/beline/conf/global/limit[@maxmodulescount]"]);
      }
      catch (Exception)
      {
        maxModulesCount = 32;
      }
      modules = new Hashtable(maxModulesCount);
      modulesCapacity = maxModulesCount;
    }
  
  } // class BModules
} // namespace
