using System;
using System.IO;

namespace LibBeline {
  /// Zaloguje do souboru
  public sealed class BLogManagerFile : BLogManager {

    // Attributes
    /// Cesta k souboru, do kterého se loguje
    public string Path
    {
      get {return path;}
    }
    private string path;
    
    private StreamWriter file;
    // 
    public BLogManagerFile (string aPath)
    {
      logManagerType = BEnumLogManagerType.File;
      
      // open file to
      FileInfo fi = new FileInfo(aPath);
      if (!fi.Exists) 
        file = File.CreateText(aPath);
      else
        file = File.AppendText(aPath);
       
      file.AutoFlush = true;
    }
    // 
    public override void Log (string aMessage, BEnumLogLevel aLevel)
    {
      // TODO dodelej filtr logu

      string outputLine = String.Format("{0:D} {1}-{2}-{3}", DateTime.Now, 
                                        Enum.GetName(aLevel.GetType(), aLevel), aMessage);
      file.WriteLine(outputLine);
    }
    // 
    public override void Log (Exception aException)
    {
      // TODO dodelej filtr logu
      string outputLine = String.Format("{0:D} {1}-{2}-{3}", DateTime.Now,
                                        Enum.GetName(typeof(BEnumLogLevel), BEnumLogLevel.ErrorEvent),
                                        aException.Message);
      file.WriteLine(outputLine);
    }
    
    // 
    public void Sync ()
    {
      file.Flush();
    }
    
    // 
    ~BLogManagerFile ()
    {
      file.Close();
    }
  } // class BLogManagerFile
} // namespace

