using System;

namespace LibBeline {
	/// Faktory tvořící instance BBusManageru
	public static class BBusManagerFactory {

	  // <exception>NotFoundException</exception>
	  public static BBusManager CreateBusManager (BTransactionItem aTransaction)
	  {
	    BBusManager retval;   // return value
	    
	    BModuleItem module = BModuleManager.GetInstance().GetModule(aTransaction.ModuleOID);
	    BConfigItem moduleConfig = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID);
	    string busManager = moduleConfig["/beline/conf/module/bus[bustype]"].ToString();
	    
	    switch (busManager)
	    {
	      case "Fifo":
	        retval = new BFifoManager((aTransaction==null ? 0 : aTransaction.ModuleProcess.Id));
	        break;
	      default:
	        throw new EntryPointNotFoundException("Not supported Bus Manager.");
	    }
	    
	    return retval;
	  }

	} // class BBusManagerFactory
} // namespace

