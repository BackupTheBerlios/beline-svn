using System;
using System.Collections;

namespace LibBeline {
	/// <summary>Abstract class covering all Item classes.</summary>
	public abstract class BItem {
    #region
	  /// <summary>
	  /// Identification string of this object.
	  /// </summary>
	  public string OID 
	  { 
	    get {return oid; }
	  }
    /// <summary>
    /// Identification string of this object.
    /// </summary>
    protected string oid;
    #endregion
	  
	  /// Array of OIDs (for greater sure of uniques of OIDs)
    private static ArrayList oidlist;

	  // 
    /// <summary>
    /// Generate an unique random 16char long string ID.
    /// </summary>
    /// <returns>New string ID.</returns>
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

