using System;

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
    
    /// 
    private static LibBeline singletonInstance;
    
    /// return instance of LibBeline
    public static LibBeline GetInstance()
    {
      return singletonInstance;
    }
    
    // 
    public static LibBeline InitializeInstance (BEnumSystem aSystem, string aProjectName)
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
          BSlaveServiceManager.GetInstance();
        }
      }
      return singletonInstance;
    }
    
    // 
    private LibBeline (BEnumSystem aSystem, string aProjectName)
    {
      projectName = aProjectName;
      system = aSystem;
    }
    
  } // class LibBeline
} // namespace

