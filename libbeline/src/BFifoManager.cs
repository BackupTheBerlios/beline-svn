using System;
using System.IO;
using Mono.Unix;

namespace LibBeline {
  /// Komunikační třída pro komunikaci přes pojmenované roury
  public class BFifoManager : BBusManager {

    StreamWriter swriter;
    StreamReader sreader;
    FileStream fs;
    
    private int farPid;

    ///<summary>Create new instance of Bus Manager communication throught named pipes</summary>
    ///<param aFarPID="Process ID of module on the far end" />
    public BFifoManager (int aFarPid)
    {
      string tmpPath = BConfigManager.GetInstance().GlobalConf["/beline/conf/general/paths[fifos]"]
        + BItem.GenerateID();
        
      // try if fifo exists, if none, create
      FileInfo fi = new FileInfo(tmpPath);
      if (!fi.Exists)
      {
        Syscall.mkfifo(tmpPath, FilePermissions.DEFFILEMODE);
      }
      
      fs = new FileStream(tmpPath, FileMode.Open, FileAccess.ReadWrite);
      swriter = new StreamWriter(fs);
      sreader = new StreamReader(fs);
      
      if (aFarPid == 0)
      { // unknown PID of far module, wait until he send me first informative message
        BMessage message = this.Receive();
        farPid = Convert.ToInt32(message.IdFrom);
      }
      else
      {
        farPid = aFarPid;
        // send first informative message
        BMessage message = BMessage.CreateSimpleCommand("end.msg");
        this.Send(message); 
      }
      
      // register handler for USR1 signal
      Mono.Posix.Syscall.signal(10, new Mono.Posix.Syscall.sighandler_t(SignalCatcher));
    }
    // 
    public override BEnumBusManagerType GetManagerType ()
    {
      return BEnumBusManagerType.Fifo;
    }
    
    ///<summary>Send message to the destination</summary>
    ///<param destination="PID of module's process"></param>
    public override void Send(BMessage aMessage)
    {
      // add information about sender's and receiver's PIDs
      aMessage.IdFrom = Syscall.getpid().ToString();
      aMessage.IdTo = farPid.ToString();
      
      // send signal to next process (acknowledgement of receiving message)
      Syscall.kill(farPid, Signum.SIGUSR1);
      
      // write the message to a pipe
      swriter.Write(aMessage.ToString());
    }
    
    public override BMessage Receive()
    {
      string tmpMessage = sreader.ReadToEnd();
      
      return BMessage.LoadFromXml(tmpMessage);
    }
    
    ~BFifoManager()
    {
      string tmpPath = fs.Name;
      // close streams
      swriter.Close();
      sreader.Close();
      
      // try to remove files
      try
      {
        File.Delete(tmpPath);
      }
      catch (IOException e) {}  // ignore exception if file is in use and not exists
      catch (System.Security.SecurityException) {} // ignore security exception
    }
    
    private void SignalCatcher(int aSignalNumber)
    {
      BMessage msg = Receive();
      if (LibBeline.GetInstance().System == BEnumSystem.master)
        BMasterServiceManager.GetInstance().ArrivedMessage(msg);
      else if (LibBeline.GetInstance().System == BEnumSystem.slave)
        BSlaveServiceManager.GetInstance().ArrivedMessage(msg);
    }

  } // class BFifoManager
} // namespace

