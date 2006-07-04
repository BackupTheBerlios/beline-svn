using System;
using System.IO;

namespace LibBeline {
  /// <summary>Store messages to a file.</summary>
  public sealed class BLogManagerFile : BLogManager {

    /// Cesta k souboru, do kterého se loguje
    public string Path
    {
      get {return path;}
    }
    private string path;
    
    private StreamWriter file;
    /// <summary>
    /// Create new instance of log manager.
    /// </summary>
    /// <param name="aPath">The whole path to file storing messages.</param>
    /// <param name="aMinimalLogLevel">Minimal level of logging messages (included) which will be logged.</param>
    public BLogManagerFile (string aPath, BEnumLogLevel aMinimalLogLevel)
    {
      logManagerType = BEnumLogManagerType.File;
      minimalLogLevel = aMinimalLogLevel;
      path = aPath;
    }
    /// <summary>
    /// Override. Overloaded. Write message to log.
    /// </summary>
    /// <param name="message">Message to write.</param>
    /// <param name="level">Level of message.</param>
    public override void Log (string message, BEnumLogLevel level)
    {
      // if message's level is less then minimal wanted do not log it
      if (level > minimalLogLevel) return;

      try
      {
        string outputLine = String.Format("{0:G} {1}-{2}", DateTime.Now, 
                                          Enum.GetName(level.GetType(), level), message);

        FileInfo fi = new FileInfo(path);
        if (!fi.Exists) 
          file = File.CreateText(path);
        else
          file = File.AppendText(path);

        file.WriteLine(outputLine);
        file.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine("Can't write to log file: {0}", e.Message);
      }
    }
    /// <summary>
    /// Override. Overloaded. Write message to log as error event.
    /// </summary>
    /// <param name="exception">Exception to write.</param>
    public override void Log (Exception exception)
    {
      // if message's level is less then minimal wanted do not log it
      if (minimalLogLevel > BEnumLogLevel.ErrorEvent) return;

      try
      {
        FileInfo fi = new FileInfo(path);
        if (!fi.Exists) 
          file = File.CreateText(path);
        else
          file = File.AppendText(path);
        
        string outputLine = String.Format("{0:G} {1}-{2}", DateTime.Now,
                                          Enum.GetName(typeof(BEnumLogLevel), BEnumLogLevel.ErrorEvent),
                                          exception.Message);
        file.WriteLine(outputLine);
        file.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine("Can't write to log file: {0}", e.Message);
      }
    }
  } // class BLogManagerFile
} // namespace

