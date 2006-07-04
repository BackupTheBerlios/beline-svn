using System;
using System.Xml;

namespace LibBeline {
  /// <summary>Factory class that produce instances of log manager. In this class a log manager creating logic is encapsulated. </summary>
  public class BLogManagerFactory {

    /// <summary>
    /// Create instance of new log manager.
    /// </summary>
    /// <returns></returns>
    public static BLogManager CreateLogManager ()
    {
      BConfigItem globalConf = BConfigManager.GetInstance().GlobalConf;
      XmlElement node = (XmlElement)globalConf.GetXmlNode("/beline/conf/global/logging");
      if (node == null) throw new Exception("Error in global configuration. Logging manager not specified.");
      
      // TODO umozni vice logovacich manazeru naraz
      XmlElement loggingElement = (XmlElement)node.FirstChild;
      BEnumLogLevel logLevel;
      switch (loggingElement.GetAttribute("loglevel"))
      {
        case "Nothing":
          logLevel = BEnumLogLevel.Nothing;
          break;
        case "ErrorEvent":
          logLevel = BEnumLogLevel.ErrorEvent;
          break;
        case "WarningEvent":
          logLevel = BEnumLogLevel.WarningEvent;
          break;
        case "InfoEvent":
          logLevel = BEnumLogLevel.InfoEvent;
          break;
        case "Messages":
          logLevel = BEnumLogLevel.Messages;
          break;
        case "AllActions":
          logLevel = BEnumLogLevel.AllActions;
          break;
        case "Debug":
          logLevel = BEnumLogLevel.Debug;
          break;
        default:
          throw new Exception("Error in global configuration. Minimal logging level should be one of:\n ErrorEvent, WarningEvent, InfoEvent, Messages, AllActions, Debug");
      }
      
      switch (loggingElement.LocalName.ToLower())
      {
        case "filelogging":
          return new BLogManagerFile(loggingElement.GetAttribute("filepath"), logLevel);
        default: 
          throw new Exception("Error in global configuration. Bad name of logging manager. Should be one of: File");
      }
    }

  } // class BLogManagerFactory
} // namespace

