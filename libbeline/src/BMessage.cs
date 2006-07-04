using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace LibBeline {
  /// <summary>Třída zapouzdřující zprávu přenášenou mezi klientem a serverem</summary>
  public class BMessage {
    #region attributes
    /// <summary>
    /// 
    /// </summary>
    public BEnumCommands Command  
    {
      get { return command; }
    }
    private BEnumCommands command;
    
    /// <summary>XML dokument s výsledkem resp. zadáním (definice tohoto souboru je specifické)</summary>
    public string Template
    {
      get { return template; }
    }
    private string template;
    
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
    #endregion
    
    // class read, cache and return templates of messages
    //private BTemplates templates;
    
    /// <summary>
    /// Create instance of a message.
    /// </summary>
    /// <param name="template">Message's template</param>
    /// <param name="command">Message's command</param>
    protected BMessage(string template, BEnumCommands command)
    {
      this.template = template;
      this.command = command;
    }
    
    /// Constructor make instance from string value (received from 
    /// <exception>XmlException</exception>
    public static BMessage LoadFromXml (string aXml)
    {
      XmlDocument xml = new XmlDocument();
      string messageFrom, messageTo;
      XmlElement element;   // working variable
      BMessage retval;
      
      // convert string representation to Xml
      xml.LoadXml(aXml);
      
      // show type of message
      element = xml["beline"];
      
      // read basic informations
      if (element == null) throw new XmlException("Bad message format.");
      element = element["message"];
      if (element == null) throw new XmlException("Bad message format.");
      messageFrom = element.GetAttribute("modulefrom");
      messageTo = element.GetAttribute("moduleto");
      
      if (element["masters"] != null)
      { // message from master
        element = element["masters"];
        if (element["alive"] != null)
        { // alive message
          retval = BMessageAlive.LoadFromXml(element["alive"]);
//          element = element["alive"];
//          parameters = new string[1];
//          parameters[0] = "";
//          foreach (XmlNode node in element.ChildNodes)
//            // convert childs of "alive" node to 
//            parameters[0] += node.OuterXml + '\n';
//          template = "alive.msg";
        }
        else if (element["run"] != null)
        { // "run" message
          retval = BMessageRun.LoadFromXml(element["run"]);
//          element = element["run"];
//          attribute = element.Attributes["procedure"];
//          string procedure = (attribute != null ? attribute.Value : "");
//          parameters = new string[2];
//          parameters[0] = procedure;
//          parameters[1] = "";
//          foreach (XmlNode node in element.ChildNodes)
//            // convert childs of "run" node to 
//            parameters[1] += node.OuterXml + '\n';
//          template = "run.msg";
        }
        else if (element["getstatus"] != null)
        { // "getstatus" message
          retval = BMessageSimpleCommand.LoadFromXml(element["getstatus"]);
//          parameters = new string[0];
//          template = "getstatus.msg";
        }
        else if (element["stop"] != null)
        { // "stop" message
          retval = BMessageSimpleCommand.LoadFromXml(element["stop"]);
//          parameters = new string[0];
//          template = "stop.msg";
        }
        else if (element["end"] != null)
        { // "end" message
          retval = BMessageSimpleCommand.LoadFromXml(element["end"]);
//          parameters = new string[0];
//          template = "end.msg";
        }
        else throw new XmlException("Bad message format.");
      }
      else if (element["slaves"] != null)
      { // message form slave       
        element = element["slaves"];
        if (element["status"] != null)
        {
          retval = BMessageStatus.LoadFromXml(element["status"]);
//          element = element["complete"];
//          attribute = element.Attributes["complete"];
//          string complete = (attribute != null ? attribute.Value : "0");
//          element = element["notice"];
//          string notice = (element != null ? element.Value : "");
//          parameters = new string[2];
//          parameters[0] = complete;
//          parameters[1] = notice;
//          template = "status.msg";
        }
        else if (element["return"] != null)
        {
          retval = BMessageReturn.LoadFromXml(element["return"]);
//          element = element["return"];
//          attribute = element.Attributes["status"];
//          string status = (attribute != null ? attribute.Value : "0");
//          parameters = new string[2];
//          parameters[0] = status;
//          parameters[1] = "";
//          foreach (XmlNode node in element.ChildNodes)
//            // convert childs of "run" node to 
//            parameters[1] += node.OuterXml + '\n';
//          template = "return.msg";
        }
        else if (element["question"] != null)
        {
          retval = BMessageQuestion.LoadFromXml(element["question"]);
//          element = element["question"];
//          // has only one children
//          XmlNode child = element.FirstChild;
//          parameters = new string[1];
//          parameters[0] = child.OuterXml;
//
//          template = "question.msg";
        }
        else throw new XmlException("Bad message format.");
      }
      else throw new XmlException("Bad message format.");    
              
      // create return message and return it
      retval.IdFrom = messageFrom;
      retval.IdTo = messageTo;
      return retval;
    }
    
    /// <summary>
    /// Constructor of status message
    /// </summary>
    /// <param name="complete">Percent complete</param>
    /// <param name="notice"></param>
    /// <returns></returns>
    public static BMessage CreateStatus (int complete, string notice)
    {
      return new BMessageStatus(complete, notice);
    }
    
    /// <summary>
    /// Constructor of return message
    /// </summary>
    /// <param name="status">Status of last operation.</param>
    /// <param name="result">Result of last operation.</param>
    /// <returns></returns>
    public static BMessage CreateReturn (int status, string result)
    {
      return new BMessageReturn(status, result);
    }
  
    /// <summary>
    /// Constructor of question message
    /// </summary>
    /// <param name="innerMessage">Question XML message compliant to a DTD libbeline.msg.dtd</param>
    /// <returns></returns>
    public static BMessage CreateQuestion (string innerMessage)
    {
      return new BMessageQuestion(innerMessage);
    }
    
    /// <summary>Constructor of alive message</summary>
    /// <param name="aConfiguration">Configuration items"</param>
    public static BMessage CreateAlive (BValueType[] aConfiguration)
    {
      return new BMessageAlive(aConfiguration);
    }

    /// <summary>
    /// Constructor of run message
    /// </summary>
    /// <param name="procedure">Name of procedure to run.</param>
    /// <param name="parameters">Array of parameters.</param>
    /// <returns></returns>
    public static BMessage CreateRun (string procedure, BValueType[] parameters)
    {
      return new BMessageRun(procedure, parameters);
    }

    /// <summary>
    /// Constructor of getstatus, stop a end message
    /// </summary>
    /// <param name="template">Template of simple message</param>
    /// <returns></returns>
    public static BMessage CreateSimpleCommand (string template)
    {
      if (template != "getstatus.msg" && template != "stop.msg" && template != "end.msg")
        throw new ArgumentException("No message from given template: " + template);
        
      return new BMessageSimpleCommand(template);
    }
    
    /// <summary>Create return value with error message</summary>
    /// <param name="aMessage">Error message for user</param>
    /// <param name="aStatus">Nonzero return status of last program's run</param>
    public static BMessage CreateException(string aMessage, int aStatus)
    {
      if (aStatus == 0) aStatus = 1;
      
      string message = "<retval><item description=\"error\"><string><text lang=\"cz\">" + aMessage + 
        "</text></string></bretval></retval>";
      return CreateReturn(aStatus, message);
    }
    
    /// <summary>Create return value with error message with default status 1</summary>
    /// <param name="aMessage">Error message for user</param>
    public static BMessage CreateException(string aMessage)
    {
      return CreateException(aMessage, 1);
    }
  } // class BMessage
  
  /// <summary>
  /// Class representation of alive message
  /// </summary>
  public sealed class BMessageAlive : BMessage
  {
    // Configuration of module (a content of the alive element)
    public BValueType[] Configuration
    {
      get
      {
        return configuration;
      }
    }
    private BValueType[] configuration;
    
    /// Create new instance of Alive message
    public BMessageAlive(BValueType[] configuration) : base ("alive.msg", BEnumCommands.BCommDefault)
    {
      this.configuration = configuration;
    }
    
    public static BMessage LoadFromXml(XmlNode aliveNode)
    {
      BValueType[] tmpConfiguration = new BValueType[aliveNode.ChildNodes.Count];
      int i = 0;  // iterator
      
      foreach (XmlNode node in aliveNode.ChildNodes)
        // convert childs of "alive" node to
        tmpConfiguration[i++] = BValueType.Deserialize(node);
      
      return new BMessageAlive(tmpConfiguration);
    }
    
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      
      StringBuilder tmpStr = new StringBuilder(1000);
      foreach (BValueType hodnota in configuration)
      {
        tmpStr.Append(BValueType.Serialize(hodnota));
        tmpStr.Append('\n');
      }
      
      retval = String.Format(retval, IdFrom, IdTo, tmpStr.ToString());
      return retval;
    }
  }
  
  /// <summary>
  /// Class representation of question message
  /// </summary>
  public sealed class BMessageQuestion : BMessage
  {
    public string InnerMessage
    {
      get { return innerMessage; }
    }
    private string innerMessage;
    
    public BMessageQuestion(string innerMessage) : base ("question.msg", BEnumCommands.BCommReturnValue)
    {
      this.innerMessage = innerMessage;
    }
    
    public static BMessage LoadFromXml(XmlNode questionNode)
    {
      string tmpInner = "";
      
      // has only one children
      XmlNode child = questionNode.FirstChild;
      tmpInner = child.OuterXml;

      return new BMessageQuestion(tmpInner);
    }
    
    /// <summary>
    /// Overrided. Return string representation of message
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      retval = String.Format(retval, IdFrom, IdTo, innerMessage);
      return retval;
    }
  }
    
  /// <summary>
  /// Class representation of return message
  /// </summary>
  public sealed class BMessageReturn : BMessage
  {
    public int Status
    {
      get { return status; }
    }
    private int status;
    
    public string Result
    {
      get { return result; }
    }
    private string result;
    
    public BMessageReturn(int status, string result) : base ("return.msg", BEnumCommands.BCommReturnValue)
    {
      if (status < 0) status = 0;

      this.status = status;
      this.result = result;
    }
    
    public static BMessage LoadFromXml(XmlNode returnNode)
    {
      XmlElement element;   // working variable
      XmlAttribute attribute; // working variable
      string tmpStatus = "";
      string tmpResult = "";
      
      element = (XmlElement)returnNode;
      attribute = element.Attributes["status"];
      tmpStatus = (attribute != null ? attribute.Value : "0");
      // get the whole return element (to avoid multiple root elements)
      tmpResult = element.OuterXml;
      
      return new BMessageReturn(Convert.ToInt32(tmpStatus), tmpResult);
    }
    
    /// <summary>
    /// Overrided. Return string representation of message
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      retval = String.Format(retval, IdFrom, IdTo, status, result);
      return retval;
    }      
  }
  
  /// <summary>
  /// Class representation of run message
  /// </summary>
  public sealed class BMessageRun : BMessage
  {
    public string Procedure
    {
      get { return procedure; }
    }
    private string procedure;
    
    public BValueType[] Parameters
    {
      get { return parameters; }
    }
    private BValueType[] parameters;
    
    public BMessageRun(string procedure, BValueType[] parameters) : base("run.msg", BEnumCommands.BCommDefault)
    {
      this.procedure = procedure;
      this.parameters = parameters;
    }
    
    public static BMessage LoadFromXml(XmlNode runNode)
    {
      XmlElement element;   // working variable
      XmlAttribute attribute; // working variable
      string tmpProcedure = "";
      BValueType[] tmpParameters = new BValueType[runNode.ChildNodes.Count];
      int i=0;  // iterator
      
      element = (XmlElement)runNode;
      attribute = element.Attributes["procedure"];
      tmpProcedure = (attribute != null ? attribute.Value : "");
      foreach (XmlNode node in element.ChildNodes)
        // convert childs of "run" node to 
        tmpParameters[i++] = BValueType.Deserialize(node);
        
      return new BMessageRun(tmpProcedure, tmpParameters);
    }
      
    /// <summary>
    /// Overrided. Return string representation of message
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      
      StringBuilder tmpStr = new StringBuilder(1000);
      foreach (BValueType hodnota in parameters)
      {
        tmpStr.Append(BValueType.Serialize(hodnota));
        tmpStr.Append('\n');
      }
      
      retval = String.Format(retval, IdFrom, IdTo, procedure, tmpStr.ToString());
      return retval;
    }
  }
  
  /// <summary>
  /// Class representation of simple command message
  /// </summary>
  public sealed class BMessageSimpleCommand : BMessage
  {
    public BMessageSimpleCommand(string template) : base (template, BEnumCommands.BCommDefault)
    {}
    
    public static BMessage LoadFromXml(XmlNode simpleNode)
    {
      string templateName;
      switch (simpleNode.LocalName)
      {
        case "getstatus":
          templateName = "getstatus.msg";
          break;
        case "stop":
          templateName = "stop.msg";
          break;
        default:
          templateName = "end.msg";
          break;
      }
      
      return new BMessageSimpleCommand(templateName);
    }
    
    /// <summary>
    /// Overrided. Return string representation of message
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      
      retval = String.Format(retval, IdFrom, IdTo);
      return retval;
    }
  }
  
  /// <summary>
  /// Class representation of status message
  /// </summary>
  public sealed class BMessageStatus : BMessage
  {
    public int Complete
    {
      get { return complete; }
    }
    private int complete;
    
    public string Notice
    {
      get { return notice; }
    }
    private string notice;
    
    public BMessageStatus(int complete, string notice) : base ("status.msg", BEnumCommands.BCommReturnValue)
    {
      if (complete < 0) complete = 0;
      if (complete > 100) complete = 100;
      
      this.complete = complete;
      this.notice = notice;
    }
    
    public static BMessage LoadFromXml(XmlNode runNode)
    {
      XmlElement element;   // working variable
      XmlAttribute attribute; // working variable
      string tmpComplete = "";
      string tmpNotice = "";
      
      element = (XmlElement)runNode;
      attribute = element.Attributes["percent"];
      tmpComplete = (attribute != null ? attribute.Value : "0");
      element = element["notice"];
      tmpNotice = (element != null ? element.InnerText : "");

      return new BMessageStatus(Convert.ToInt32(tmpComplete), tmpNotice);
    }
      
    /// <summary>
    /// Overrided. Return string representation of message
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string retval = BTemplates.GetInstance().ReadTemplate(Template);
      
      retval = String.Format(retval, IdFrom, IdTo, complete, notice);
      return retval;
    }
  }
} // namespace

