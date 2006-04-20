using System;

namespace LibBeline {
	/// Typ k uložení verze
	public sealed class BVersion : BValueType {

	  // Attributes
	  /// Major version number
	  public int Major
	  {  
	    set { major  = value; }
	    get { return major  ; }
	  }
	  private int major;
	  /// Minor version number
	  public int Minor  
	  {
	    set { minor  = value; }
	    get { return minor  ; }
	  }
	  private int minor;
	  /// Revision/Repair
	  public int Revision  
	  {
	    set { revision  = value; }
	    get { return revision  ; }
	  }
	  private int revision;
	  /// Build
	  public int Build  
	  {
	    set { build  = value; }
	    get { return build  ; }
	  }
	  private int build;
	  
	  // <exception>ArgumentNullException, FormatException, OverflowException</exception>
	  public BVersion (string aName, string aVersion) : base (aName, BEnumType.BVersion)
	  {
	    string[] lpieces;
			try
			{
			  lpieces = aVersion.Split('.');
			  major = Convert.ToInt32(lpieces[0]);
			  minor = Convert.ToInt32(lpieces[1]);
			  revision = Convert.ToInt32(lpieces[2]);
			  build = Convert.ToInt32(lpieces[3]);
			}
			catch (Exception e)
			{
			  throw new Exception("Invalid format of version. Should be in format like this: 1.0.0.0");
			}
	  }
	  public BVersion (string aVersion) : this ("", aVersion) {}
	  
	  // 
	  public BVersion (string aName, int aMajor, int aMinor, int aRevision, int aBuild)
	  			 : base (aName, BEnumType.BVersion)
	  {
	    if (aMajor < 0) major = 0; else major = aMajor;
	    if (aMinor < 0) minor = 0; else minor = aMinor;
	    if (aRevision < 0) revision = 0; else revision = aRevision;
	    if (aBuild < 0) build = 0; else build = aBuild;
	  }
	  public BVersion (int aMajor, int aMinor, int aRevision, int aBuild)
	  			 : this ("", aMajor, aMinor, aRevision, aBuild) {}
	  
	  // 
	  public override string ToString ()
	  {
	    return major.ToString() + '.' +
	    			 minor.ToString() + '.' +
	    			 revision.ToString() + '.' +
	    			 build.ToString();
	  }

	}
}

