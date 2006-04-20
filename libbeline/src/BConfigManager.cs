using System;
using System.Xml;
using System.IO;
using System.Collections;

namespace LibBeline {
  /// Globální konfigurační manager
  public sealed class BConfigManager {

    public static string GlobalModulesPath
    {
      get {return globalModulesPath;}
    }
    private static string globalModulesPath = "/etc/libbeline/modules";
    public static string LocalModulesPath
    {
      get {return localModulesPath; }
    }
    private static string localModulesPath = "~/.libbeline/modules";
    
    // Attributes
    /// Globalni konfigurace (inicializovaná v metodě Initialize třídy LibBeline)
    public BConfigItem GlobalConf 
    { 
      get { return globalConf  ; }
    }
    private BConfigItem globalConf;
    /// Pole indexované OIDéčky; udržuje se kvůli bezpečnému promazání
    private Hashtable moduleConf;
    
    /// Maximum count of configuration items (+1 for global configuration)
    public int Capacity
    {
      get {return capacity;}
    }
    int capacity;
    /// 
    private static BConfigManager singletonInstance;
    
    /// constructor
    private BConfigManager()
    {
      // create global configuration
      globalConf = new BConfigItem("/etc/libbeline/global.conf");
      
      try
      {
        BValueType str = (BValueType)globalConf["/beline/conf/general/limit[maxmodulescount]"];
        capacity = Convert.ToInt32(str.ToString());
      }
      catch (Exception e)
      {
        capacity = 32;
      }
      
      moduleConf = new Hashtable(capacity);
    }

    /// Load new module's configuration from global and then from local config file
    public BConfigItem LoadModuleConfig (string aFileName)
    {
      if (moduleConf.Count == capacity) throw new Exception("Maximum count of configuration items reached.");
      
      BConfigItem retval = new BConfigItem(Path.Combine("/etc/libbeline/modules",aFileName));
      retval.LoadConfig(Path.Combine("~/.libbeline/modules", aFileName));
      moduleConf.Add(retval.OID, retval);
      
      return retval;
    }

    ///<summary>Return configuration of module</summary>
    ///<param aOID="Configuration item's OID. Should be obtained by BModuleItem.ConfigOID property." />
    public BConfigItem GetModuleConfig (string aOID)
    {
      return (BConfigItem)moduleConf[aOID];
    }

    public void FreeModuleConfig (string aOID)
    {
      moduleConf.Remove(aOID);
    }

    public static string GenerateXPath (string aPath, string aLanguage)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    // 
    public static BConfigManager GetInstance ()
    {
    	// no BConfigManager in slave instance of libBeline
	    if (LibBeline.GetInstance().System == BEnumSystem.slave) return null;
	
      if (singletonInstance == null)
      {
        singletonInstance = new BConfigManager();
      }
      
      return singletonInstance;
    }
  } // class BConfigManager
} // namespace

