using System;
using System.IO;

namespace LibBeline {
  /// Informace o jednom modulu
  public sealed class BModuleItem : BItem {

    // Attributes
    /// path to the memory stored configuration of module
    private string configOID;
    public string ConfigOID
    {
      get {return configOID;}
    }
    
    /// 
    private string name;
    public string Name
    {
      get { return name; }
    }
    
    /// Version of the file
    private BVersion version;
    public BVersion Version
    {
      get { return version; }
    }
    
    /// 
    private string author;
    public string Author
    {
      get { return author; }
    }
    
    /// 
    private string description;
    public string Description
    {
      get { return description; }
    }
    
    /// <exception>FileNotFoundException, XmlException</exception>
    public BModuleItem (string aFileName)
    {
      oid = BItem.GenerateID();
      
      // create instance of configuration item
      try
      {
        BConfigManager configManager = BConfigManager.GetInstance();
        BConfigItem configItem = configManager.LoadModuleConfig(aFileName);
        // store some frequently used properties
        configOID = configItem.OID;
        name = configItem["/beline/conf/module[name]"].ToString();
        version = new BVersion(configItem["/beline/conf/module[version]"].ToString());
        author = configItem["/beline/conf/module/author[name]"].ToString() + " <" + 
                 configItem["/beline/conf/module/author[email]"].ToString() + ">";
        description = configItem["/beline/conf/module[description]"].ToString();
      }
      catch (System.Xml.XmlException e)
      {
        throw new System.Xml.XmlException("Bad XML file " + aFileName + ": " + e.Message);
      }
    }
    // 
    public BConfigItem GetConfig ()
    {
      BConfigManager configManager = BConfigManager.GetInstance();
      return configManager.GetModuleConfig(configOID);
    }
  } // class BModuleItem
} // namespace

