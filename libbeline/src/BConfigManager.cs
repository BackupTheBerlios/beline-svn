using System;
using System.Xml;
using System.IO;
using System.Collections;

namespace LibBeline {
  /// Globální konfigurační manager
  
  /// <summary>
  /// This manager stores configuration files in memory during running of libbeline. In fact this is a
  /// container for BConfigItem elements that offer some extra functions over them.
  /// </summary>
  public sealed class BConfigManager {

    #region Attributes
    /// <summary> Path to the global module configuration files </summary>
    public static string GlobalModulesPath
    {
      get {return Path.Combine(LibBeline.GlobalPath, "modules");}
    }
    /// <summary> Path to the local module configuration files </summary>
    public static string LocalModulesPath
    {
      get {return Path.Combine(LibBeline.LocalPath, "modules");}
    }
    
    /// <summary>A global libbeline configuration (it is initialized in the method LibBeline.Initialize)</summary>
    public BConfigItem GlobalConf 
    { 
      get { return globalConf; }
    }
    private BConfigItem globalConf;
    /// <summary>Pole indexované OIDéčky; udržuje se kvůli bezpečnému promazání</summary>
    private Hashtable moduleConf;
    
    /// <summary>Maximum count of configuration items (+1 for global configuration)</summary>
    public int Capacity
    {
      get {return capacity;}
    }
    int capacity;
    #endregion

    /// 
    private static BConfigManager singletonInstance;
    
    private BConfigManager()
    {
      // create global configuration
      globalConf = new BConfigItem(Path.Combine(LibBeline.GlobalPath, "global.conf"));
      globalConf.ImportGlobalConfig(Path.Combine(LibBeline.LocalPath, "global.conf"));
      
      try
      {
        BValueType str = (BValueType)globalConf["/beline/conf/global/limit[@maxmodulescount]"];
        capacity = Convert.ToInt32(str.ToString());
      }
      catch
      {
        capacity = 32;
      }
      
      moduleConf = new Hashtable(capacity);
    }

    /// <summary>Load new module's configuration from global and then from local config file</summary>
    public BConfigItem LoadModuleConfig (string aFileName)
    {
      if (moduleConf.Count == capacity) throw new Exception("Maximum count of configuration items reached.");
      
      BConfigItem retval = new BConfigItem(Path.Combine(BConfigManager.GlobalModulesPath,aFileName));
      retval.ImportConfig(Path.Combine(BConfigManager.LocalModulesPath, aFileName));
      moduleConf.Add(retval.OID, retval);
      
      return retval;
    }

    ///<summary>Return configuration of module</summary>
    ///<param name="aOID">Configuration item's OID. Should be obtained by BModuleItem.ConfigOID property</param>
    public BConfigItem GetModuleConfig (string aOID)
    {
      return (BConfigItem)moduleConf[aOID];
    }

    /// <summary>
    /// Remove module's configuration from memory.
    /// </summary>
    /// <param name="aOID">Identification of configuration item.</param>
    public void FreeModuleConfig (string aOID)
    {
      moduleConf.Remove(aOID);
    }

    /// <summary>
    /// Part of a singleton design pattern. Return an instance of the BConfigManager or if this instance doesn't
    /// exist create it (within initialize global configuration item)
    /// </summary>
    /// <returns></returns>
    public static BConfigManager GetInstance ()
    {
      if (singletonInstance == null)
      {
        singletonInstance = new BConfigManager();
      }
      
      return singletonInstance;
    }
  } // class BConfigManager
} // namespace

