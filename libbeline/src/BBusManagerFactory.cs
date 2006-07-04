using System;

namespace LibBeline {
	/// Faktory tvořící instance BBusManageru
	public class BBusManagerFactory {

    ///<summary> Create new instance of BBusManager's child (ie. BFifoManager, BCorbaManager etc.)</summary>
    ///<param name="aTransaction">Instance of a transaction that want to make connection in master or null in slave.</param>
	  // <exception>NotFoundException</exception>
	  public static BBusManager CreateBusManager (BTransactionItem aTransaction)
	  {
	    BBusManager retval;   // return value
	    
	    BConfigItem config = BConfigManager.GetInstance().GlobalConf;
	    string busManager = config["/beline/conf/global/bus[@bustype]"].ToString();
	    
	    switch (busManager)
	    {
	      case "Fifo":
	        if (aTransaction == null)
	          // it is called from slave
	          retval = new BFifoManager(0, "");
	        else
	          // it is called from master
	          retval = new BFifoManager(aTransaction.ModuleProcess.Id, aTransaction.OID);
	      
	        break;
	      default:
	        throw new EntryPointNotFoundException("Not supported Bus Manager.");
	    }
	    
	    return retval;
	  }

	} // class BBusManagerFactory
} // namespace

