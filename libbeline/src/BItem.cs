using System;
using System.Collections;

namespace LibBeline {
	/// Abstraktní třída zastřešující všechny Item třídy
	public abstract class BItem {

	  // Attributes
	  ///
	  protected string oid; 
	  public string OID 
	  { 
	    get {return oid; }
	  }
	  
	  /// Array of OIDs (for greater sure of uniques of OIDs)
    private static ArrayList oidlist;

	  // this class generate a random 16char long string
	  public static string GenerateID ()
	  {
	    string retval = Guid.NewGuid().ToString();
	    int cycleBlock = 0;  // block cycling
	    
	    if (oidlist == null) oidlist = new ArrayList(128);
	    
	    while (oidlist.IndexOf(retval) != -1)
	    { 
	     if (cycleBlock++ == 1000) throw new Exception("Impossible to generate unique ID.");
       // try to generate unique ID
       retval = Guid.NewGuid().ToString();
       
       oidlist.Add(retval);
      }
      
	    return retval;
	  }
	} // class BItem
} // namespace

