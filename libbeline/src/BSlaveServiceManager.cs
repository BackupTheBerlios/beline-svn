using System;
using System.Collections;

namespace LibBeline {
  /// Class communicating with modules (slave part)
  public class BSlaveServiceManager {

    // Attributes
    /// 
    private static BSlaveServiceManager singletonInstance;
    
    /// Collection of observable classes (the should by announced when message arrive)
    ArrayList observers;
    
    BBusManager busManager;
    private bool messageHandlerFinish;
    
    /// <summary>Object storing module's configuration.</summary>
    public BConfigItem ModuleConfiguration
    {
      get { return moduleConfiguration; }
    }
    private BConfigItem moduleConfiguration;
    
    // 
    public static BSlaveServiceManager GetInstance ()
    {
      // only slave can make instance of BSlaveServiceManager
      if (LibBeline.GetInstance().System != BEnumSystem.slave) return null;

      return singletonInstance;
    }
    
    /// <summary> Initialize new instance of BSlaveServiceManager</summary>
    /// <param name="projectName">This name is used when searching a project's configuration file. Must be same as the name of the configuration file.</param>
    public static void InitializeInstance(string projectName)
    {
      try
      {
        if (singletonInstance != null) return;  // instance exists, nothing to create
        singletonInstance = new BSlaveServiceManager(projectName);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    // 
    public void AttachObserver (BObservable aClass)
    {
      try
      {
        observers.Add(aClass);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    // 
    public void DetachObserver (int aClass)
    {
      try
      {
        if (aClass < 0 || aClass >= observers.Count) return;  // bad number
        
        observers.Remove(aClass);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    // 
    public void CleanObservers ()
    {
      try
      {
        observers.Clear();
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    public BMessage Receive(bool blocking)
    {
      BMessage message = null;
      while (message==null)
      {
        message = busManager.Receive(false);
        if (message == null)
        { // no message arrived, wait 1 second and try again 
          System.Threading.Thread.Sleep(200);
        }
      } // while
      
      return message;
    }
    
    /// This method wait for messages and handle them (ie. call Observers)
    /// If everything goes right finish at the end of the module
    /// <return>Return value that should be returned to the shell</return>
    public int MessageHandler()
    {
      messageHandlerFinish = false;
      
      BMessage message = null;
      while (!messageHandlerFinish)
      {
        message = this.Receive(true);
        ArrivedMessage(message);
      } // while
      
      if (message.Template == "end.msg")
        return 0;
      else
        return 1;
    }
    
    // 
    public void SendCommand (BMessage message)
    {
      try
      {
        busManager.Send(message);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// Create return value and send it to the master
    /// <param aStatus="Return code of procedure, zero means finished OK" />
    /// <param aMessage="Project specific reply in XML format, which will be connected to a return node" />
    public void SendReturn (int aStatus, string aMessage)
    {
      try
      {
        BMessage msg = BMessage.CreateReturn(aStatus, aMessage);
        busManager.Send(msg);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// Create question message and send it to the master
    /// <param name="aInnerQuestion">Question to master, which will be shown to user</param>
    public void SendQuestion (string aInnerQuestion)
    {
      try
      {
        BMessage msg = BMessage.CreateQuestion(aInnerQuestion);
        busManager.Send(msg);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    // 
    public void SendStatus (int aComplete, string aMessage)
    {
      try
      {
        BMessage msg = BMessage.CreateStatus(aComplete, aMessage);
        
        busManager.Send(msg);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// signal from BusManager that some message arrived
    public void ArrivedMessage(BMessage aMessage)
    {   
      try
      { 
        BMessage tmpMessage;  // prepared return message
        switch (aMessage.Template)
        {
          case "alive.msg":
            // tell everybody that the message arrived
            try
            {
              foreach (BObservable observer in observers)
              {
                observer.MessageArrived(aMessage);
              }
            }
            catch (Exception e)
            { // error in parameters
              System.Console.WriteLine("Exception " + e.Message);
              tmpMessage = BMessage.CreateStatus(0, e.Message);
              // send error message
              busManager.Send(tmpMessage);
              throw;  // log it and finish
            }
            
            // send acknowledgement
            tmpMessage = BMessage.CreateStatus(100, "");
            busManager.Send(tmpMessage); 
            break;
          case "run.msg":
          case "getstatus.msg":
          case "stop.msg":
            // tell everybody that the message arrived
            foreach (BObservable observer in observers)
            {
              observer.MessageArrived(aMessage);
            }
            break;
          case "end.msg":
            // stop the module
            messageHandlerFinish = true;
            break;
          default:
            // ignore another messages
            break;
        } // switch 
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }
    
    /// Initialize an instance of BSlaveServiceManager
    /// <param name="projectName">This name is used when searching a project's configuration file. Must be same as the name of the configuration file.</param>
    private BSlaveServiceManager (string projectName)
    {
      try
      {
        observers = new ArrayList();
        
        // intialize configuration of this module (module should read this configuration)
        moduleConfiguration = BConfigManager.GetInstance().LoadModuleConfig(
          System.IO.Path.Combine(BConfigManager.GlobalModulesPath, projectName + ".conf"));
        moduleConfiguration.ImportConfig(
          System.IO.Path.Combine(BConfigManager.LocalModulesPath, projectName + ".conf"));
        
        // don't know far end's PID so send 0 to search for
        busManager = BBusManagerFactory.CreateBusManager(null);
      }
      catch (Exception e)
      { // log all top level errors
        LibBeline.GetInstance().LogManager.Log(e);
        throw;
      }
    }

  } // class BSlaveServiceManager
} // namespace
