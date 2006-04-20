using System;


/// Derived from class Exception
public class BException {

  // Attributes
  /// 
  private string exceptionName = "BException";
  /// 
  private string message;
  /// Vnořená chyba
  private BException inner;

  public BException (string aMessage)
  {
    throw new System.Exception ("Not implemented yet!");
  }
  // .
  public BException (BException aException, string aMessage)
  {
    throw new System.Exception ("Not implemented yet!");
  }

  public string PrintStack ( )
  {
    throw new System.Exception ("Not implemented yet!");
  }

  public string getName ()
  {
    throw new System.Exception ("Not implemented yet!");
  }

  public string getMessage ()
  {
    throw new System.Exception ("Not implemented yet!");
  }

  public BException getInnerException ()
  {
    throw new System.Exception ("Not implemented yet!");
  }

}

