using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace LibBeline {
	/// Manžer udržující stav transakcí
	public sealed class BModuleManager {

	  // Attributes
	  /// 
	  private static BModuleManager singletonInstance;
	  
	  /// Při spouštění transakce, pro níž ještě nemám transakci potřebuju info
	  private BModules modManager;
	  /// Seznam běžících transakcí
	  private Hashtable transactions;
	  private int transactionsCapacity;
	  // 
	  public static BModuleManager GetInstance ()
	  {
	    // no BModuleManager in slave instance of libBeline
	    if (LibBeline.GetInstance().System == BEnumSystem.slave) return null;
	
	    if (singletonInstance == null)
	    {
	     singletonInstance = new BModuleManager();
	    }
	    
	    return singletonInstance;
	  }
	  
	  ///Start new transaction of module if it is possible
	  ///<param name="aOID">Module's OID</param>
	  public BTransactionItem Start (string aOID)
	  {
	    if (transactions.Count == transactionsCapacity) throw new Exception ("Maximum running transactions reached!");
	   
	    // get instance of module
	    BModuleItem module = modManager[aOID];
	    if (module == null) throw new Exception("Module with OID " + aOID + " not found.");
	    
	    // read configuration from module
	    XmlNode config;
	    try
	    {
	      config = module.GetConfig().GetXmlNode("/beline/conf/module/configuration");
	      // if this is not Xml element, configuration file is bad
	      if (config.NodeType != XmlNodeType.Element) throw new Exception();
	    }
	    catch (Exception)
	    {
	      throw new Exception("Error in configuration file. It does't contain \"/beline/conf/module/configuration\" branch");
	    }
	    
	    ArrayList configItems = new ArrayList(config.ChildNodes.Count);
	    foreach (XmlNode element in config.ChildNodes)
	    {
	      // if another child than element "bcfgitem" do not parse him
        if (element.NodeType != XmlNodeType.Element || element.LocalName != "bcfgitem") continue;
        
        BValueType hodnota = BValueType.Deserialize(element.FirstChild);
        if (hodnota != null) configItems.Add(hodnota);
	    }
	    
	    BValueType[] configArray = new BValueType[configItems.Count];
	    configItems.CopyTo(configArray);
	    
	    // create transaction
	    BTransactionItem transaction = new BTransactionItem(module, configArray);
	    
	    // save transaction to inner ArrayList
	    transactions.Add(transaction.OID, transaction);
	    
	    return transaction;
	  }
	  
	  // 
	  public void Stop (string aOID)
	  {
      // find instance of transaction
      BTransactionItem transaction = transactions[aOID] as BTransactionItem;
      
      if (transaction == null) return;  // no transaction found => nothing to do
      
      transaction.Destroy();
      transactions.Remove(aOID);
	  }
	  
	  ///
	  public void Sleep (string aOID, int aTime)
	  {
	    if (aTime < 0) aTime = 0;
      // find instance of transaction
      BTransactionItem transaction = transactions[aOID] as BTransactionItem;
      
      if (transaction == null) return;  // no transaction found => nothing to do
      
      transaction.Sleep(aTime);
	  }
	  
	  ///
	  public void Sleep (string aOID)
	  {
	    this.Sleep(aOID, 0);
	  }
	  
	  // 
	  public void Awake (string aOID)
	  {
      // find instance of transaction
      BTransactionItem transaction = transactions[aOID] as BTransactionItem;
      
      if (transaction == null) return;  // no transaction found => nothing to do
      
      transaction.Awake();
	  }
	  
	  /// Run procedure with given parameters on a transaction with given OID
	  /// <param name="aOID">Transaction's OID</param>
	  /// <param name="aProcedure">Name of runned procedure</param>
	  /// <param name="aParameters">Array of parameters to run</param>
	  public void Run (string aOID, string aProcedure, ArrayList aParameters)
	  {
      // find instance of transaction
      BTransactionItem transaction = transactions[aOID] as BTransactionItem;
      if (transaction == null) return;  // no transaction found => nothing to do
      
      transaction.Restart(aProcedure, aParameters);
	  }
	  
	  //
	  public void End(string aOID)
	  {
	    // stop all running transactions
	    foreach (BTransactionItem transaction in transactions)
	    {
	      if (transaction.ModuleOID == aOID)
	      {
	        transactions.Remove(transaction.OID);
	      }
	    }
	    
	    // unload module itself
	    modManager.DisableModule(aOID);
	  }
	  
	  // 
	  public void SendMessage (string aMessage)
	  {
	    throw new System.Exception ("Not implemented yet!");
	  }
	  
	  // 
	  public BModuleItem[] GetAllModules ()
	  {
	    return modManager.GetModuleList();
	  }

    // Return array of all transactions
    public BTransactionItem[] GetAllTransactions()
    {
      BTransactionItem[] retval = new BTransactionItem[transactions.Count];
      int i=0;
      foreach (DictionaryEntry transaction in transactions)
      {
        retval[i++] = (BTransactionItem)transaction.Value;
      }
      //transactions.CopyTo(retval, 0);
      
      return retval; 
    }
    
	  // 
	  public BMessage GetResult ()
	  {
	    throw new System.Exception ("Not implemented yet!");
	  }
	  
	  /// Return transaction with given OID
	  /// <param aOID="OID of running transaction to be returned" />
	  public BTransactionItem GetTransaction (string aOID)
	  {
			if (transactions[aOID] == null)
				throw new ArgumentException("Transaction with OID " + aOID + " not exists");
			
			return (BTransactionItem)transactions[aOID];
	  }
	  
	  /// Return module with given OID
	  /// <param aOID="OID of module" />
	  public BModuleItem GetModule(string aOID)
	  {
	    if (modManager[aOID] == null)
	      throw new ArgumentException("Module with OID " + aOID + " not exists");
	      
	    return (BModuleItem)modManager[aOID];
	  }
	  
	  // 
	  public void CheckTransactions ()
	  {
	   throw new System.Exception ("Not implemented yet!");
	  }
	  
	  // 
	  private BModuleManager ()
	  {
	    modManager = new BModules();
	    
	    BConfigItem globalConfig = BConfigManager.GetInstance().GlobalConf;
	    int maxTransactionsCount;
      try
      { // read maximum count of modules from global configuration
        maxTransactionsCount = Convert.ToInt32(globalConfig["/beline/conf/global/limit[@maxtransactionscount]"].ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine("chyba pri zjistovani poctu instanci:" + e.Message);
        maxTransactionsCount = 32;
      }
      transactions = new Hashtable(maxTransactionsCount);
      transactionsCapacity = maxTransactionsCount;
      
      // importing all modules
      try
      {
//        XmlNode paths = globalConfig.GetXmlNode("/beline/conf/global/paths");
        DirectoryInfo wayToModules = new DirectoryInfo(BConfigManager.GlobalModulesPath);
        
        // import all module's through its configuration files
        foreach (FileInfo module in wayToModules.GetFiles("*.conf"))
        {
          modManager.ImportModule(module.Name);
        }
      }
      catch (Exception e)
      {
        throw new Exception("Error during importing of modules:\n "+ e.Message);
      }
	  }

	} // class BModuleManager
} // namespace

