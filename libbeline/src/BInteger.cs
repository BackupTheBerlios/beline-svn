using System;


namespace LibBeline {
  /// 
  public class BInteger : BValueType {

    // Attributes
    /// 
    private int innerValue;

    /// <summary>
    /// Create instance of class from classic .Net int value
    /// </summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    public BInteger (string name, int aValue) : base(name, BEnumType.BInteger)
    {
      innerValue=aValue;
    }
    
    /// <summary>Create instance from string value.</summary>
    /// <param name="name">The name of the instance</param>
    /// <param name="aValue">The new value</param>
    /// <exception>ArgumentException, FormatException, OverflowException</exception>
    public BInteger (string name, string aValue) : base(name, BEnumType.BInteger)
    {
      innerValue=Convert.ToInt32(aValue);
    }
    
    /// <summary>
    /// Convert the inner value to the ekvivalent .Net representation.
    /// </summary>
    /// <returns>Ekvivalent .Net representation</returns>
    public virtual int ToInt ()
    {
      return innerValue;
    }
    
    /// <summary>
    /// Overrided. Convert the inner valuet to the ekvivalent string representation.
    /// </summary>
    /// <returns>The string representation of integer value</returns>
    public override string ToString ()
    {
      return innerValue.ToString();
    }

    /// <summary>
    /// Overrided. Return BEnumType.BFloat.
    /// </summary>
    /// <returns></returns>
    public override BEnumType GetBType ()
    {
      return type;
    }

  } // class BInteger
} // namespace

