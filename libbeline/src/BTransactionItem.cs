using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
//using Mono.Unix;

namespace LibBeline {
  /// Keep informations about a status of transactions
  public sealed class BTransactionItem : BItem {

    // Attributes
    /// Timeout of not responding transation in seconds
    public int Timeout
    {  
      set { timeout  = value; }
      get { return timeout  ; }
    }
    private int timeout;
    
    /// Aktuální průběh
    private BEnumStatus status;
    public BEnumStatus Status
    {
      get { return status; }
    }
    
    /// Čas, kdy přišla poslední zpráva (využívá se při timeoutu)
    public DateTime LastMsgTime
    {
      get {return lastMsgTime;}
    }
    private DateTime lastMsgTime;
    
    /// Remember message for awake
    private BMessage lastRunMessage;
    /// Physical layer used for sending and receiving messages from and to modules
    private BBusManager busManager;
    
    /// OID of module which transaction it is
    public string ModuleOID
    {
      get { return moduleOID; }
    }
    private string moduleOID;
    
    /// Object representing a process of module on the far end
    public Process ModuleProcess
    {
      get { return moduleProcess; }
    }
    private Process moduleProcess;
    
    ///<param aConfiguration="Configuration parameters for transaction"></param>
    public BTransactionItem(BModuleItem aModule, BValueType[] aConfiguration)
    {
      oid = BItem.GenerateID();
      moduleOID = aModule.OID;
      
      status = BEnumStatus.Idle;
      lastRunMessage = null;  // nothing was run

      // get module configuration
      BConfigItem configuration = BConfigManager.GetInstance().GetModuleConfig(aModule.ConfigOID);
      
      // set timeout
      try
      {
        timeout = Convert.ToInt32(configuration["/beline/conf/module/run[@timeout]"].ToString());
      }
      catch (Exception)
      {
        try
        {
          timeout = Convert.ToInt32(BConfigManager.GetInstance().
                                    GlobalConf["/beline/conf/global/limit[@defaulttimeout]"].
                                    ToString());
        }
        catch (Exception)
        { // timeout is 10 minutes
          timeout = 600;
        }
      }
      
      // create instance of module
      string module = configuration["/beline/conf/module[@runcommand]"].ToString();
      FileInfo fi = new FileInfo(module);
      if (!fi.Exists)
        throw new FileNotFoundException("File " + module + " not found.");    
      
      moduleProcess = Process.Start(module);
      
      try
      {
        // set instance of BusManager
        busManager = BBusManagerFactory.CreateBusManager(this);
      }
      catch (Exception)
      { // can't continue => kill process
        if (!moduleProcess.HasExited)
          moduleProcess.Kill();
        // notice user
        throw;
      }
      
      // alive module
      BMessage message = BMessage.CreateAlive(aConfiguration);
      busManager.Send(message);
      // wait until confirmation message arrive 
      message = busManager.Receive(true);
      if (message.Template != "status.msg") throw new Exception("Error in a communication. Module don't send acknowledgement");
    
      if (((BMessageStatus)message).Complete != 100)
        // some error in parameters
        throw new Exception("Module not initialized.\nError: " + ((BMessageStatus)message).Notice);
    }
    
    /// Send common message to the transaction
    public void SendMessage(BMessage aMessage)
    {
      busManager.Send(aMessage);
      lastMsgTime = DateTime.Now;
    }

    /// switch to sleep status
    ///<param name="aTime">Not implemented yet!</param>
    public void Sleep (int aTime)
    {
      if (status == BEnumStatus.Sleep) return;
      
      // prepare message to module (realy stop running) and send
      BMessage message = BMessage.CreateSimpleCommand("stop.msg");
      busManager.Send(message);
      
      // save statistics
      status = BEnumStatus.Sleep;
      lastMsgTime = DateTime.Now;
    }
    // 
    public void Awake ()
    {
      // only sleeping module can be awake 
      if (status != BEnumStatus.Sleep) return;
      // no message to run => must run new
      if (lastRunMessage == null) return;
      
      // rerun last run message
      // TODO docasna adresa
      busManager.Send(lastRunMessage);
      
      // save statistics
      status = BEnumStatus.Running;
      lastMsgTime = DateTime.Now;
    }
    
    ///<param aProcedure="Name of procedure to run on module. If empty string is given start last started procedure."></param>
    ///<param aParameters="Array of BValueType parameters"></param>
    public void Restart(string aProcedure, ArrayList aParameters)
    {
      BMessage message;

      // if last procedure should be restarted and no message running => can't restart
      if (aProcedure==String.Empty)
      {
        if (lastRunMessage == null) 
          return;
        else
          message = lastRunMessage;
      }
      else
      {
        BValueType[] parameters = new BValueType[aParameters.Count];
        aParameters.CopyTo(parameters);

        message = BMessage.CreateRun(aProcedure, parameters);
      }
      // send run message
      busManager.Send(message);
      
      // save statistics
      lastMsgTime = DateTime.Now;
      status = BEnumStatus.Running;
    }
    
    /// try to get message from the net
    /// <return>Message or null if not present</return>
    public BMessage Receive()
    {
      return busManager.Receive(false);
    }
    
    /// finish the transaction and kill the process
    public void Destroy()
    {
      BMessage message = BMessage.CreateSimpleCommand("end.msg");
      busManager.Send(message);
      
      // wait until acknowledgement arrive
      while (message.Template == "return.msg")
      {
        message = busManager.Receive(true);
      }
      
      // dangerous if this thread is faster then module's
//      if (message.Parameters[2] != "0")
//        throw new Exception(message.Parameters[3]);
//      else
//        // if the message don't stop properly itself
//        moduleProcess.Kill();
    }
    
    /// Stop transaction 
    ~BTransactionItem ()
    {
      Destroy();
    }
  } // class BTransactionItem
} // namespace

