using System;

namespace LibBeline {
	/// Typ k uložení verze
	public sealed class BVersion : BValueType {

    #region attributes
	  /// <summary>Major version number</summary>
	  public int Major
	  {  
	    set { major  = value; }
	    get { return major  ; }
	  }
	  private int major;
	  /// <summary>Minor version number</summary>
	  public int Minor  
	  {
	    set { minor  = value; }
	    get { return minor  ; }
	  }
	  private int minor;
	  /// <summary>Revision/Repair</summary>
	  public int Revision  
	  {
	    set { revision  = value; }
	    get { return revision  ; }
	  }
	  private int revision;
	  /// <summary>Build</summary>
	  public int Build  
	  {
	    set { build  = value; }
	    get { return build  ; }
	  }
	  private int build;
    #endregion
	  
    /// <summary>
    /// Overrided. Create instance of class from string value.
    /// </summary>
    /// <param name="aName">The name of the instance</param>
    /// <param name="aVersion">Version using format x.y.z.w</param>
    /// <exception>ArgumentNullException, FormatException, OverflowException</exception>
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
			catch (Exception)
			{
			  throw new Exception("Invalid format of version. Should be in format like this: 1.0.0.0");
			}
	  }
    /// <summary>
    /// Overrided. Create instance of class from string value.
    /// </summary>
    /// <param name="aVersion">Version using format x.y.z.w</param>
    /// <exception>ArgumentNullException, FormatException, OverflowException</exception>
	  public BVersion (string aVersion) : this ("", aVersion) {}
	  
	  /// <summary>
	  /// Overrided. Create instance of class from four int values.
	  /// </summary>
	  /// <param name="aName">The name of the instance</param>
	  /// <param name="aMajor">Major version number</param>
	  /// <param name="aMinor">Minor version number</param>
	  /// <param name="aRevision">Revision number</param>
	  /// <param name="aBuild">Build</param>
	  public BVersion (string aName, int aMajor, int aMinor, int aRevision, int aBuild)
	  			 : base (aName, BEnumType.BVersion)
	  {
	    if (aMajor < 0) major = 0; else major = aMajor;
	    if (aMinor < 0) minor = 0; else minor = aMinor;
	    if (aRevision < 0) revision = 0; else revision = aRevision;
	    if (aBuild < 0) build = 0; else build = aBuild;
	  }
    /// <summary>
    /// Overrided. Create instance of class from four int values.
    /// </summary>
    /// <param name="aMajor">Major version number</param>
    /// <param name="aMinor">Minor version number</param>
    /// <param name="aRevision">Revision number</param>
    /// <param name="aBuild">Build</param>
	  public BVersion (int aMajor, int aMinor, int aRevision, int aBuild)
	  			 : this ("", aMajor, aMinor, aRevision, aBuild) {}
	  
    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>The string representation of inner value</returns>
	  public override string ToString ()
	  {
	    return major.ToString() + '.' +
	    			 minor.ToString() + '.' +
	    			 revision.ToString() + '.' +
	    			 build.ToString();
	  }

	}
}

