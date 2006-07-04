using System;
using System.IO;
using System.Diagnostics;
//using Mono.Unix;

namespace LibBeline {
  /// Komunikační třída pro komunikaci přes pojmenované roury
  public class BFifoManager : BBusManager {

//    StreamWriter swriter;
//    StreamReader sreader;
//    FileStream fsIn, fsOut;
    private string pathIn, pathOut;
    /// Transaction owner's Id
    //private string transactionOid;
    
    private string farPid;

    ///<summary>Create new instance of Bus Manager communication throught named pipes</summary>
    ///<param name="farPid">Process ID of module on the far end</param>
    ///<param name="transactionOid">ID of owners transaction</param>
    public BFifoManager (int farPid, string transactionOid)
    {
//      if (aFarProcess == null)
//      { // this is slave (module)
////        swriter = Process.GetCurrentProcess().StandardOutput;
////        sreader = Process.GetCurrentProcess().StandardInput;
//      }
//      else
//      {
//        swriter = aFarProcess.StandardInput;
//        sreader = aFarProcess.StandardOutput;
//      }
//      
//      LibBeline.GetInstance().LogManager.Log("roury otevreny", BEnumLogLevel.Debug);
//      transactionOid = aTransactionOid;
      string tmpPath = BConfigManager.GetInstance().GlobalConf["/beline/conf/global/paths[@fifos]"]
        + "/bfifomanager" + (farPid != 0 ? farPid : System.Diagnostics.Process.GetCurrentProcess().Id);
      pathIn = tmpPath + (farPid == 0?"in":"out"); // name of pipe in direction from master to slave
      pathOut = tmpPath + (farPid == 0?"out":"in"); // name of pipe in direction from slave to master
         
      this.farPid = farPid.ToString();
//      BMessage message;
//      if (aFarPid == 0)
//      { // unknown PID of far module, wait until he send me first informative message
//        message = this.Receive(true);
//        farPid = Convert.ToInt32(message.IdFrom);
//      }
//      else
//      {
//        farPid = aFarPid;
//        // send first informative message
//        message = BMessage.CreateSimpleCommand("end.msg");
//        
//        // send information to module (at first I can't use Send routine, because unhandled signal 14 kills application)
//        message.IdFrom = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
//        message.IdTo = farPid.ToString();
//        StreamWriter swriter = new StreamWriter(pathOut);
//        swriter.Write(message.ToString());
//        swriter.Close();
//        //this.Send(message); 
//      }
    }

    /// <summary>
    /// Override. Return subtype of the manager. Throught this can the child of BBusManager be identified.
    /// </summary>
    /// <returns></returns>
    public override BEnumBusManagerType GetManagerType ()
    {
      return BEnumBusManagerType.Fifo;
    }
    
    ///<summary>Override. Send message to the destination</summary>
    ///<param name="aMessage">Message to send.</param>
    public override void Send(BMessage aMessage)
    {
      // wait until file not exists (no message in use)
      int i=1;
      while (i < 32000 && File.Exists(pathOut))
      {
        System.Threading.Thread.Sleep(i);
        i <<= 1;  // multiply 2
      }
      if (File.Exists(pathOut))
       // after 64 seconds module still don't create pipes
       throw new Exception("Can't send message");

      // add information about sender's and receiver's PIDs
      aMessage.IdFrom = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
      aMessage.IdTo = farPid.ToString();
      
      // write the message to a pipe
      StreamWriter swriter = new StreamWriter(pathOut);
      swriter.Write(aMessage.ToString());
      swriter.Close();
 
      // send signal to next process (acknowledgement of receiving message)
      //Syscall.kill(farPid, Signum.SIGUSR1);
    }
    
    ///<summary>Override. Receive message</summary>
    ///<param name="blocking">Wait for the message if not present now</param>
    public override BMessage Receive(bool blocking)
    {
      // wait until message was created
      int i=2000;
      while (i < 32000 && !File.Exists(pathIn) && blocking)
      {
        System.Threading.Thread.Sleep(i);
        i <<= 1;  // multiply 2
      }
      if (!File.Exists(pathIn))
        // after 64 seconds module still don't create pipes
        //throw new Exception("Can't receive message");
        return null;

      StreamReader sreader = new StreamReader(pathIn);
      string tmpMessage = sreader.ReadToEnd();
      sreader.Close();

      File.Delete(pathIn);
      BMessage receivedMessage = BMessage.LoadFromXml(tmpMessage);
      // message received => delete file with message
      try
      {
        farPid = receivedMessage.IdFrom;
      }
      catch {} // ignore errors in conversion (if number not given, don't mind)
      return receivedMessage;
    }
    
    /// <summary>
    /// Override. Sync all buffers and close connection to bus.
    /// </summary>
    public override void Destroy()
    {
      // try to remove files
      try
      {
        File.Delete(pathIn);
        File.Delete(pathOut);
      }
      catch (IOException) {}  // ignore exception if file is in use and not exists
      catch (System.Security.SecurityException) {} // ignore security exception
    }
    
    /// <summary>
    /// Sync all buffers and close connection to bus.
    /// </summary>
    ~BFifoManager()
    { 
      Destroy();
    }
//    
//    private void SignalCatcher(int aSignalNumber)
//    {
//      Console.WriteLine("Signal catched");
//      BMessage msg = Receive();
//      if (LibBeline.GetInstance().System == BEnumSystem.master)
//        BMasterServiceManager.GetInstance().ArrivedMessage(transactionOid, msg);
//      else if (LibBeline.GetInstance().System == BEnumSystem.slave)
//        BSlaveServiceManager.GetInstance().ArrivedMessage(msg);
//    }

  } // class BFifoManager
} // namespace

