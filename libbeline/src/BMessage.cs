using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace LibBeline {

  // class read, cache and return templates of messages
  internal sealed class BTemplates
  {
    /// it can cach maximum of 20 items
    Hashtable cache;
    private static BTemplates singletonInstance;
    
    private BTemplates()
    {
      cache = new Hashtable(20);
    }
    
    public static BTemplates GetInstance()
    {
      // if this is first run (singleton not inicialized yet)
      if (singletonInstance == null)
        singletonInstance = new BTemplates();
      
      return singletonInstance;
    }
    
    /// Read template file with given name and return its content as string 
    public string ReadTemplate(string aFileName)
    {
      // first try to read from template
      if (cache[aFileName] != null)
        return cache[aFileName].ToString();
        
      // combine complete path found in a global configuration
      string pathToTemplates = BConfigManager.GetInstance().GlobalConf["/beline/conf/general/paths[templates]"].ToString();
      pathToTemplates = Path.Combine(pathToTemplates, aFileName);
            
      // read the whole file
      StreamReader reader;
      string retval;
      try
      {
        reader = new StreamReader(pathToTemplates);
      }
      catch (DirectoryNotFoundException e)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (FileNotFoundException e)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (IOException e)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (ArgumentNullException e)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (ArgumentException e)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      
      try
      {
        retval = reader.ReadToEnd();
      }
      catch (IOException e)
      {
        reader.Close();
        throw;
      }
      catch (OutOfMemoryException e)
      {
        reader.Close();
        throw;
      }
      
      reader.Close();
      
      // cache template
      cache.Add(aFileName, retval);
      return retval;
    }
  }

  /// Třída zapouzdřující zprávu přenášenou mezi klientem a serverem
  public sealed class BMessage {
    /// 
    public BEnumCommands Command  
    {
      get { return command; }
    }
    private BEnumCommands command;
    
    /// XML dokument s výsledkem resp. zadáním (definice tohoto souboru je specifické)
    public string Template
    {
      get { return template; }
    }
    private string template;
    
    /// Seznam parametrů, v případě v případě příkazu
    public string[] Parameters
    {
      get { return parameters; }
    }
    private string[] parameters;
    
    /// <summary>Id of the sending module (format depends on type of Bus Manager)</summary>
    public string IdFrom
    {
      get { return idFrom; }
      set { idFrom = value; }
    }
    string idFrom;
    
    /// <summary>Id of the receiving module (format depends on type of Bus Manager)</summary>
    public string IdTo
    {
      get { return idTo; }
      set { idTo = value; }
    }
    string idTo;
    
    
    // class read, cache and return templates of messages
    //private BTemplates templates;
    
    // create instance of a message
    private BMessage(string aTemplate, string[] aParameters, BEnumCommands aCommand)
    {
      template = aTemplate;
      parameters = aParameters;
      command = aCommand;
    }
    
    /// Constructor make instance from string value (received from 
    /// <exception>XmlException</exception>
    public static BMessage LoadFromXml (string aXml)
    {
      XmlDocument xml = new XmlDocument();
      string messageFrom, messageTo;
      XmlElement element;   // working variable
      XmlAttribute attribute; // working variable
      string[] parameters;
      string template;
      
      // convert to Xml
      xml.LoadXml(aXml);
      
      // show type of message
      element = xml["beline"];
      
      if (element == null) throw new XmlException("Bad message format.");
      element = element["message"];
      if (element == null) throw new XmlException("Bad message format.");
      attribute = element.Attributes["modulefrom"];
      messageFrom = (attribute != null ? attribute.Value : "");
      attribute = element.Attributes["moduleto"];
      messageTo = (attribute != null ? attribute.Value : "");
      
      if (element["masters"] != null)
      { // message from master
        element = element["masters"];
        if (element["alive"] != null)
        { // alive message
          element = element["alive"];
          parameters = new string[1];
          parameters[0] = "";
          foreach (XmlNode node in element.ChildNodes)
            // convert childs of "alive" node to 
            parameters[0] += node.OuterXml + '\n';
          template = "alive.msg";
        }
        else if (element["run"] != null)
        { // "run" message
          element = element["run"];
          attribute = element.Attributes["procedure"];
          string procedure = (attribute != null ? attribute.Value : "");
          parameters = new string[2];
          parameters[0] = procedure;
          parameters[1] = "";
          foreach (XmlNode node in element.ChildNodes)
            // convert childs of "run" node to 
            parameters[1] += node.OuterXml + '\n';
          template = "run.msg";
        }
        else if (element["getstatus"] != null)
        { // "getstatus" message
          parameters = new string[0];
          template = "getstatus.msg";
        }
        else if (element["stop"] != null)
        { // "stop" message
          parameters = new string[0];
          template = "stop.msg";
        }
        else if (element["end"] != null)
        { // "end" message
          parameters = new string[0];
          template = "end.msg";
        }
        else throw new XmlException("Bad message format.");
        
        // create instance of default master's command
        return new BMessage(template, parameters, BEnumCommands.BCommDefault);
      }
      else if (element["slaves"] != null)
      { // message form slave       
        element = element["slaves"];
        if (element["status"] != null)
        {
          element = element["complete"];
          attribute = element.Attributes["complete"];
          string complete = (attribute != null ? attribute.Value : "0");
          element = element["notice"];
          string notice = (element != null ? element.Value : "");
          parameters = new string[2];
          parameters[0] = complete;
          parameters[1] = notice;
          template = "status.msg";
        }
        else if (element["return"] != null)
        {
          element = element["return"];
          attribute = element.Attributes["status"];
          string status = (attribute != null ? attribute.Value : "0");
          parameters = new string[2];
          parameters[0] = status;
          parameters[1] = "";
          foreach (XmlNode node in element.ChildNodes)
            // convert childs of "run" node to 
            parameters[1] += node.OuterXml + '\n';
          template = "return.msg";
        }
        else if (element["question"] != null)
        {
          element = element["question"];
          // has only one children
          XmlNode child = element.FirstChild;
          parameters = new string[1];
          parameters[0] = child.OuterXml;

          template = "question.msg";
        }
        else throw new XmlException("Bad message format.");
        
        // create return message and return it
        BMessage retval = new BMessage(template, parameters, BEnumCommands.BCommReturnValue);
        retval.IdFrom = messageFrom;
        retval.IdTo = messageTo;
        return retval;
      }
      else throw new XmlException("Bad message format.");
    }
    
    // Constructor of status message
    public static BMessage CreateStatus (int aComplete, string aNotice)
    {
      if (aComplete < 0) aComplete = 0;
      if (aComplete > 100) aComplete = 100;
      
      string[] parameters = new string[2];
      parameters[0] = aComplete.ToString();
      parameters[1] = aNotice;
      
      return new BMessage("status.msg", parameters, BEnumCommands.BCommReturnValue); 
    }
    
    // Constructor of return message
    public static BMessage CreateReturn (int aStatus, string aResult)
    {
      if (aStatus < 0) aStatus = 0;
      
      string[] parameters = new string[2];
      parameters[0] = aStatus.ToString();
      parameters[1] = aResult;
      
      return new BMessage("return.msg", parameters, BEnumCommands.BCommReturnValue);
    }
    
    /// Constructor of question message
    public static BMessage CreateQuestion (string innerMessage)
    {
      string[] parameters = new string[1];
      parameters[0] = innerMessage;
      
      return new BMessage("question.msg", parameters, BEnumCommands.BCommReturnValue);
    }
    
    /// <summary>Constructor of alive message</summary>
    /// <param aModuleFrom="OID of sending module" />
    /// <param aModuleTo="OID of receiving module" />
    /// <param aConfiguration="Configuration items" />  
    public static BMessage CreateAlive (BValueType[] aConfiguration)
    {
      string[] parameters = new string[1];
      StringBuilder tmpStr = new StringBuilder(1000);
      foreach (BValueType hodnota in aConfiguration)
      {
        tmpStr.Append(BValueType.Serialize(hodnota));
        tmpStr.Append('\n');
      }
      parameters[0] = tmpStr.ToString();
      
      return new BMessage("alive.msg", parameters, BEnumCommands.BCommDefault);
    }
    
    // Constructor of run message
    public static BMessage CreateRun (string aProcedure, BValueType[] aParameters)
    {
      string[] parameters = new string[2];
      parameters[0] = aProcedure;
      StringBuilder tmpStr = new StringBuilder(1000);
      foreach (BValueType hodnota in aParameters)
      {
        tmpStr.Append(BValueType.Serialize(hodnota));
        tmpStr.Append('\n');
      }
      parameters[1] = tmpStr.ToString();
      
      return new BMessage("run.msg", parameters, BEnumCommands.BCommDefault);
    }
    
    // Constructor of getstatus, stop a end message
    public static BMessage CreateSimpleCommand (string aTemplate)
    {
      if (aTemplate != "getstatus.msg" && aTemplate != "stop.msg" && aTemplate != "end.msg")
        throw new ArgumentException("No message from given template: " + aTemplate);
        
      string[] parameters = new string[0];
      
      return new BMessage(aTemplate, parameters, BEnumCommands.BCommDefault);
    }
    
    // Return message (as return value as command) as string value   
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(template);
      retval = String.Format(template, idFrom, idTo, parameters);
      return retval;
    }
  } // class BMessage
} // namespace

