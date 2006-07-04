using System;
using System.Collections;
using LibBeline.Gui;

namespace LibBeline {
  /// <summary>Třída poskytující práci s knihovnou vizuálním prvkům</summary>
  public sealed class BMasterServiceManager {

    #region attributes
    /// <summary> Check all transactions and show if some data wait in queue </summary>
    public bool NewData
    {
      get
      {
        if (newData) return true;
        
        CheckTransactions();
        newData = (messageQueue.Count > 0);
        return newData;
      }
    }
    private bool newData;
    
    /// <summary>Minimální úroveň zprávy, která se bude logovat (včetně)</summary>
    public BEnumLogLevel MinLogLevel  
    {
      set { minLogLevel  = value; }
      get { return minLogLevel  ; }
    }
    private BEnumLogLevel minLogLevel = BEnumLogLevel.WarningEvent;
    #endregion
    
    /// 
    private static BMasterServiceManager singletonInstance;
    
    /// <summary>Seznam objektů, které očekávají, že budou v případě příchozí zpráv</summary>
    private Queue messageQueue;

    public static BMasterServiceManager GetInstance ()
    {
      try
      {
        // slave shouldn't make instance of BMasterServiceManager
        if (LibBeline.GetInstance().System != BEnumSystem.master) return null;
        
        if (singletonInstance == null)
        {
          singletonInstance = new BMasterServiceManager();
        }
        
        return singletonInstance;
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    /// <summary>
    /// Return array of all currently loaded modules.
    /// </summary>
    /// <returns></returns>
    public BModuleItem[] GetInstalledModules ()
    {
      try
      {
        return BModuleManager.GetInstance().GetAllModules();
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

    /// Start new transaction of module
    /// <param name="aOID">Module's OID</param>
    /// <return>BTransactionItem object describing created transaction</return>
    public string StartModule (string aOID)
    {
      try
      {
        BTransactionItem transaction = BModuleManager.GetInstance().Start(aOID);
        return transaction.OID;
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
	  /// Run procedure with given parameters on a transaction with given OID
	  /// <param name="aOID">Transaction's OID</param>
	  /// <param name="aProcedure">Name of runned procedure</param>
	  /// <param name="aParameters">Array of parameters to run (array of a BValueType values)</param>
    public void RunModule (string aOID, string aProcedure, ArrayList aParameters)
    {
      try
      {
        BModuleManager.GetInstance().Run(aOID, aProcedure, aParameters);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="OID"></param>
    public void StopModule (string OID)
    {
      try
      {
        BModuleManager.GetInstance().Stop(OID);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    // 
    public void EndModule (string aOID)
    {
      try
      {
        BModuleManager.GetInstance().End(aOID);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

    public void AwakeModule (string aOID)
    {
      try
      {
        BModuleManager.GetInstance().Awake(aOID);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

    public void SleepModule (string aOID, int aTime)
    {
      try
      {
        BModuleManager.GetInstance().Sleep(aOID, aTime);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

    /// Send complete message to transaction
    /// <param name="aOID">ID of a transaction</param>
    /// <param name="aMessage">Message to send</param>
    public void SendMessage (string aOID, BMessage aMessage)
    {
      BTransactionItem transaction = BModuleManager.GetInstance().GetTransaction(aOID);
      if (transaction == null) throw new Exception("Transaction with OID " + aOID + " does not exists.");
      
      transaction.SendMessage(aMessage);
    }
    
    /// If some message arrived and stored in queue, read and remove it from the queue
    /// <param name="blocking">If no message in queue, wait for it</param>
    public BMessage ReceiveMessage(bool blocking)
    {
      try
      {
        // if no data waiting in queue, nothing to return
        int ticks = 0;
        while (!NewData)
        {
          ticks++;
          if (blocking && ticks < 32)
            System.Threading.Thread.Sleep(200);
          else
          {
            newData = false;
            return null;
          }
        }
        
        BMessage retval = (BMessage)messageQueue.Dequeue();
        if (messageQueue.Count == 0)
          newData = false;
          
        return retval;
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }  

    /// Return a configuration for given module
    /// <param name="OID">A module's ID </param>
    public BConfigItem GetConfig (string OID)
    {
      BModuleItem module = BModuleManager.GetInstance().GetModule(OID);
      if (module == null) return null;
      
      return module.GetConfig();
    }

    public void SetConfig (string aOID, BConfigItem aConfiguration)
    {
      throw new Exception("Not implemented yet!");
    }

    private BMasterServiceManager ()
    {    
      try
      {
        newData = false;
        messageQueue = new Queue();
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// signal from BusManager that some message arrived
    /// <param name="transactionOID">Who catch the message</param>
    /// <param name="aMessage">Arrived message</param>
    public void ArrivedMessage(string transactionOID, BMessage aMessage)
    {
      newData = true;
      
      try
      {
        switch (aMessage.Template)
        {
          case "status.msg":
          case "return.msg":
            // give it to the user (store to the queue for later use)
            messageQueue.Enqueue(aMessage);
            break;
          case "question.msg":
            BMessage returnMessage = BDialogFactory.ShowDialog(aMessage);
            BModuleManager.GetInstance().GetTransaction(transactionOID).SendMessage(returnMessage);
            break;
          default:
            // ignore unknown messages
            break;
        }
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

    // check all transactions if some message arrived
    private void CheckTransactions()
    {
      foreach (BTransactionItem transaction in BModuleManager.GetInstance().GetAllTransactions())
      {
        BMessage message = transaction.Receive();
        if (message != null) 
        {
          newData = true;
          messageQueue.Enqueue(message);
        }
      }
    }
  } // class LibBeline
} // namespace 

