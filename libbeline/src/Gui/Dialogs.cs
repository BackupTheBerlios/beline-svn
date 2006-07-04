// created on 27.4.2006 at 19:06
using Gtk;
using System;
using System.Xml;
using LibBeline;

namespace LibBeline.Gui
{
  /// base of all dialog types
  public class BDialog : Gtk.Dialog
  {
    protected Gtk.HBox hbox;
  	protected Gtk.VBox labelBox;
  	protected Gtk.Label label;
  	protected Gtk.Image image;
  	
    public BDialog(string aName, Gtk.Window aParent, string aText) : base (aName, aParent, Gtk.DialogFlags.NoSeparator)
    {
      // setup dialog
      this.Modal = true;
      this.BorderWidth = 6;
      this.HasSeparator = false;
      this.Resizable = false;
      this.VBox.Spacing=12;
      
      // graphic items
		  hbox = new Gtk.HBox();
  		Gtk.VBox labelBox = new VBox();
  		label = new Gtk.Label();
  		image = new Gtk.Image();
		
			hbox.Spacing=12;
			hbox.BorderWidth=6;
			this.VBox.Add(hbox);
			
			// set-up image
			image.Yalign=0.0F;
      hbox.Add(image);
			
			// set-up label
			label.Yalign=0.0F;
			label.Xalign=0.0F;
			label.UseMarkup=true;
			label.Wrap=true;
			label.Markup=aText;
			
			// add to dialog
			labelBox.Add(label);
			hbox.Add(labelBox);
    }
  }
  
  public class BInfoDialog : BDialog
  {
    public BInfoDialog(string aName, Gtk.Window aParent, string aText, string aLevel) 
      : base (aName, aParent, aText)
    {
			// set-up image
			switch (aLevel)
			{
			  case "error":
			    image.SetFromStock(Gtk.Stock.DialogError, Gtk.IconSize.Dialog);
			    break;
			  case "warn":
			    image.SetFromStock(Gtk.Stock.DialogWarning, Gtk.IconSize.Dialog);
			    break;
			  default: // info and mistaken levels
			    image.SetFromStock(Gtk.Stock.DialogInfo, Gtk.IconSize.Dialog);
			    break;
			}

      this.AddButton(Gtk.Stock.Ok, ResponseType.Ok);
    }
  }
  
  /// common question dialog
  public class BQuestionDialog : BDialog
  {
    public BQuestionDialog(string aName, Gtk.Window aParent, string aText) 
      : base (aName, aParent, aText)
    {
  		// set-up image
			image.SetFromStock(Gtk.Stock.DialogQuestion, Gtk.IconSize.Dialog);
      this.AddButton(Gtk.Stock.No, ResponseType.Cancel);
      this.AddButton(Gtk.Stock.Yes, ResponseType.Ok);
    }
  }
  
  public class BDialogFactory
  {
    /// this method parse LibBeline Question message, show dialog and create reply message
    /// from users response
    /// <exception>XmlException</exception>
    public static BMessage ShowDialog(BMessage aMessage)
    {
      if (aMessage.Template != "question.msg") 
        // return error message
        return BMessage.CreateException("This is not valid LibBeline Question message");
      
      BMessageQuestion messageQuestion = (BMessageQuestion)aMessage;
      XmlDocument question = new XmlDocument();
      question.LoadXml(messageQuestion.InnerMessage); // load dialog part of message
      string dialogType = question.FirstChild.LocalName;
      Gtk.Dialog dialog;
      
      switch (dialogType.ToLower())
      {
        case "string":
          XmlElement element = (XmlElement)question.SelectSingleNode("//text[@lang='cz']");
          if (element == null) 
          {
            element = (XmlElement)question.SelectSingleNode("//text[@lang='en']");
            // if czech and english string not found quit
            if (element == null) return BMessage.CreateException("Bad format of LibBeline Question message");
          }
                  
          dialog = new BQuestionDialog("Otázka", null,  element.InnerText);
          dialog.ShowAll();
          dialog.Run();
          dialog.Destroy();
          
          // finished good, return
          return BMessage.CreateReturn(0, "");
        case "dialogyesno":
          XmlElement elementyesno = (XmlElement)question.SelectSingleNode("//text[@lang='cz']");
          if (elementyesno == null) 
          {
            elementyesno = (XmlElement)question.SelectSingleNode("//text[@lang='en']");
            // if czech and english string not found quit
            if (elementyesno == null) return BMessage.CreateException("Bad format of LibBeline Question message");
          }
                  
          dialog = new BQuestionDialog("Otázka", null,  elementyesno.InnerText);
          dialog.ShowAll();
          int answer = dialog.Run();
          dialog.Destroy();
          
          // finished good, return
          return BMessage.CreateReturn(0, "<retval><item description=\"answer\"><int value=\"" + answer +
            "\"</int></bretval></retval>");
        case "dialoginfo":
          XmlElement dialogInfo = (XmlElement)question.FirstChild;
          string level = dialogInfo.GetAttribute("severity");
          
          XmlElement elementInfo = (XmlElement)dialogInfo.SelectSingleNode("//text[@lang='cz']");
          if (elementInfo == null) 
          {
            elementInfo = (XmlElement)dialogInfo.SelectSingleNode("//text[@lang='en']");
            // if czech and english string not found quit
            if (elementInfo == null) return BMessage.CreateException("Bad format of LibBeline Question message");
          }
                  
          dialog = new BInfoDialog("Informace", null,  elementInfo.InnerText, level);
          dialog.ShowAll();
          dialog.Run();
          dialog.Destroy();
          
          // finished good, return
          return BMessage.CreateReturn(0, "");
        case "dialog":
          XmlElement xmlDialog = (XmlElement)question.FirstChild;
          string caption = xmlDialog.GetAttribute("caption");
          string text = xmlDialog.GetAttribute("string");
          dialog = new BDialog(caption, null, text);
          int i=0;
          foreach (XmlNode button in xmlDialog.SelectNodes("/button"))
          {
            if (button.NodeType != XmlNodeType.Element) continue; // if it isn't valid element don't bother with him
            XmlElement buttonElement = (XmlElement)button;
            switch (buttonElement.GetAttribute("type").ToLower())
            {
              case "yes":
                dialog.AddButton(Gtk.Stock.Yes, i);
                break;
              case "no":
                dialog.AddButton(Gtk.Stock.No, i);
                break;
              case "ok":
                dialog.AddButton(Gtk.Stock.Ok, i);
                break;
              case "cancel":
                dialog.AddButton(Gtk.Stock.Cancel, i);
                break;
              case "close":
                dialog.AddButton(Gtk.Stock.Close, i);
                break;
              case "delete":
                dialog.AddButton(Gtk.Stock.Delete, i);
                break;
              case "execute":
                dialog.AddButton(Gtk.Stock.Execute, i);
                break;
              case "goback":
                dialog.AddButton(Gtk.Stock.GoBack, i);
                break;
              case "goforward":
                dialog.AddButton(Gtk.Stock.GoForward, i);
                break;
              case "godown":
                dialog.AddButton(Gtk.Stock.GoDown, i);
                break;
              case "goup":
                dialog.AddButton(Gtk.Stock.GoUp, i);
                break;
              case "golast":
                dialog.AddButton(Gtk.Stock.GotoLast, i);
                break;
              case "gofirst":
                dialog.AddButton(Gtk.Stock.GotoFirst, i);
                break;
              case "help":
                dialog.AddButton(Gtk.Stock.Help, i);
                break;
              case "new":
                dialog.AddButton(Gtk.Stock.New, i);
                break;
              default: 
                return BMessage.CreateException("Bad format of LibBeline Question message");
            } // switch
          } // foreach
          
          dialog.ShowAll();
          answer = dialog.Run();
          dialog.Destroy();
          
          // finished good, return
          return BMessage.CreateReturn(0, "<retval><item description=\"answer\"><int value=\"" + answer +
            "\"</int></bretval></retval>");
        default:
          return BMessage.CreateException("This is not valid LibBeline Question message. Dialog type should be one of string, dialog, dialogyesno or dialoginfo.");
      }
    }
  } // class
} // namespace