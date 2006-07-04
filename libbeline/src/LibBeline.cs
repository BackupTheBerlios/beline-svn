using System;
using System.IO;

namespace LibBeline {
  /// Class used for communication with programmer - first created instance
  public class LibBeline {

    // Attributes
    public BConfigManager ConfigManager
    {
      get { return configManager; }
    }
    private BConfigManager configManager;
    
    public BModuleManager ModuleManager
    {
      get { return moduleManager; }
    }
    private BModuleManager moduleManager;
    
    // name of the project using this instance of libBeline
    public string ProjectName
    {
      get { return projectName; }
      set { projectName = value;}
    }
    private string projectName;
    
    public BEnumSystem System
    {
      get {return system; }
    }
    private BEnumSystem system;
    
    public BLogManager LogManager
    {
      get {return logManager;}
    }
    private BLogManager logManager;
    
    /// <summary> Path to the global configuration and template files </summary>
    public static string GlobalPath
    {
      get {
        if (Environment.GetEnvironmentVariable("LIBBELINE_GLOBALPATH") != null)
          return Environment.GetEnvironmentVariable("LIBBELINE_GLOBALPATH");
          
        return "/etc/libbeline";
      }
    }
    // TODO only for demo CD !!!
    
    /// <summary> Path to local configuration files </summary>
    public static string LocalPath
    {
      get 
      {
        string homeDir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
        return Path.Combine(homeDir, ".libbeline"); 
      }
    }
    
    /// 
    private static LibBeline singletonInstance;
    
    /// <summary>Create instance of LibBeline</summary>
    public static LibBeline GetInstance()
    {
      return singletonInstance;
    }
    
    // 
    public static LibBeline InitializeInstance (BEnumSystem aSystem, string aProjectName)
    {
      try
      {
        if (singletonInstance == null)
        {
          singletonInstance = new LibBeline(aSystem, aProjectName);

          if (aSystem == BEnumSystem.master)
          { // only master module manager can make instances of module manager and config manager
            singletonInstance.configManager = BConfigManager.GetInstance();
            singletonInstance.moduleManager = BModuleManager.GetInstance();
            BMasterServiceManager.GetInstance();
          }
          else
          { // only slave can make slave's service manager instance
            singletonInstance.configManager = BConfigManager.GetInstance();
            BSlaveServiceManager.InitializeInstance(aProjectName);
          }
        }
      }
      catch (Exception e)
      {
        if (singletonInstance != null)
          singletonInstance.LogManager.Log(e);
          
        throw;
      }
      
      return singletonInstance;
    }
    
    // 
    private LibBeline (BEnumSystem aSystem, string aProjectName)
    {
      projectName = aProjectName;
      system = aSystem;
      logManager = BLogManagerFactory.CreateLogManager();
    }
    
  } // class LibBeline
} // namespace

