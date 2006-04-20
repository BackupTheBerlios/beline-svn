// created on 10.4.2006 at 22:10
using LibBeline;
using Gtk;
using Glade;
using System;
using System.Collections;
using System.Xml;

namespace LibBeline.Gui
{
  /// Window that shows module property
  public class ModulePropertyDialog: IDisposable
  {
    #region widgets
    [Widget ("PropertiesDialog")] Dialog PropertiesDialog;
    [Widget] VBox vbox1;
    [Widget] Gtk.Notebook nbProperties;
    [Widget] Gtk.Button btnHelp;
    [Widget] Gtk.Button btnClose;
    Gtk.Tooltips tooltips;
    #endregion
    
    private BConfigItem configItem;
    private Hashtable properties;
    private int lastItemNumber;
    
    public ModulePropertyDialog(BConfigItem aConfigItem, Gtk.Window parent)// : base("", parent, DialogFlags.DestroyWithParent)
    {
      // store config item
      configItem = aConfigItem;
      properties = new Hashtable();
      lastItemNumber = 0;
      
      // create dialog using glade
      Glade.XML gxml = new Glade.XML(null, "dialogs.glade", "PropertiesDialog", null);
      gxml.Autoconnect(this);
      
      // initialize dialog's widgets
      InitializeDialog();
      
      // connect signals to handlers
      btnClose.Clicked += new EventHandler(btnClose_click);
    }
    
    public bool Run ()
		{
			PropertiesDialog.ShowAll ();
			PropertiesDialog.Run();
			return true;
		}
    
    public void Dispose ()
		{
			PropertiesDialog.Destroy ();
			PropertiesDialog.Dispose ();
		}
		
		///<summary>Handler for closing button (try save changes to local configuration file)</summary>
		private void btnClose_click (object sender, EventArgs a)
		{
//		  XmlDocument rootDocument = new XmlDocument();
//		  rootDocument.LoadXml("<private/>");
//		  XmlElement root = rootDocument.DocumentElement;
//		  
//		  string tabsName = "";
//		  // create private node
//		  foreach (Widget folderWidget in nbProperties.AllChildren)
//		  { // go through all tabs (there are stored VBoxs with tab contain and Labels with tab name)
//		    Gtk.VBox tmpVBox = folderWidget as Gtk.VBox;
//		    if (tmpVBox == null)
//		    {
//		      Gtk.Label tmpLabel = folderWidget as Gtk.Label;
//		      tabsName = tmpLabel.Text;
//		    }
//		    else
//		    {
//		      XmlElement fold = rootDocument.CreateElement("fold");
//		      root.AppendChild(fold);
//		      fold.SetAttribute("label", tabsName);
//		      
//		      XmlElement heading;
//		      foreach (Widget propertyWidget in tmpVBox.AllChildren)
//		      { // go through all headers
//		        if (propertyWidget.Name == "GtkLabel")
//		        { // heading
//		          Gtk.Label labelElement = (Gtk.Label)propertyWidget;
//		          heading = rootDocument.CreateElement("heading");
//		          fold.AppendChild(heading);
//		          heading.SetAttribute("text", labelElement.Text);
//		        }
//		        else if (propertyWidget.Name == "GtkHBox")
//		        {
//		          Gtk.HBox hboxElement = (Gtk.HBox)propertyWidget;
//		          XmlElement hbox = rootDocument.CreateElement("bcfgitem");
//		          //foreach (Widget itemWidget in propertyWidget.
//		        }
//		      }
//		     Console.WriteLine("ss");
//		    }
//		    Console.WriteLine("zz");
//		  }
//		  
//		  Console.WriteLine("a\n{0}", rootDocument.OuterXml);

      foreach (Widget folderWidget in nbProperties.AllChildren)
      {// go through all tabs (there are stored VBoxs with tab contain and Labels with tab name)
		    Gtk.VBox tmpVBox = folderWidget as Gtk.VBox;
        if (tmpVBox == null) continue;   // skip labels        
        foreach (Widget propertyWidget in tmpVBox.AllChildren)
	      { // go through all headers
	        Gtk.HBox tmpHBox = propertyWidget as Gtk.HBox;
	        if (tmpHBox == null) continue;   // skip headings
	        
	        foreach (Widget item in tmpHBox.AllChildren)
	        {
	          if (item.Name.Substring(0,4) == "item")
	          {
	            Console.WriteLine("{0} = {1}", item.Name, properties[item.Name]);
	          }
	        }
	      }
      }
		  //configItem.SaveConfigToFile(null);
		}
		
		///<summary>Initialize dialog's widgets by private section of the configuration file</summary>
		private void InitializeDialog()
		{
      // create notebook tabs with items
      XmlNode folds = configItem.GetXmlNode("/beline/conf/module/private");
      string wayInXml = "/beline/conf/module/private";
      
      tooltips = new Gtk.Tooltips();
      
      // go throught all folds
      foreach (XmlNode fold in folds)
      {
        // skip non-valid elements
        if (fold.NodeType != XmlNodeType.Element || fold.LocalName != "fold") continue;
        XmlElement foldElement = (XmlElement)fold;
        string foldLabel = foldElement.GetAttribute("label");
        string foldHint = foldElement.GetAttribute("hint");
        
        if (wayInXml.LastIndexOf("/fold") == -1)
          wayInXml += "/fold[@text='" + foldLabel + "']";
        else
          wayInXml = wayInXml.Substring(0, wayInXml.LastIndexOf("/fold")) + "/fold[@text='" + foldLabel + "']";
        
        Label tabLabel = new Label(foldLabel);
        VBox vbox = new VBox(false, 4);
        
        // add hint to created fold
        tooltips.SetTip(tabLabel , foldHint, foldHint);
        
        // go throught all chapters in one fold
        foreach (XmlNode heading in fold.ChildNodes)
        { 
          // skip non-valid elements
          if (heading.NodeType != XmlNodeType.Element || heading.LocalName != "heading") continue;
          XmlElement headingElement = (XmlElement)heading;
          string headingText = headingElement.GetAttribute("text");
          if (wayInXml.LastIndexOf("/heading") == -1)
            wayInXml += "/heading[@text='" + headingText + "']";
          else
            wayInXml = wayInXml.Substring(0, wayInXml.LastIndexOf("/heading")) + "/heading[@text='" + headingText + "']";
          
          Label headingLabel = new Label();
          headingLabel.Markup = "<span weight=\"bold\">" + headingText + "</span>";
          vbox.PackStart(headingLabel, false, false, 6);
          // go throught all properties in one section
          foreach (XmlNode property in heading.ChildNodes)
          {
            // skip non-valid elements
            if (property.NodeType != XmlNodeType.Element || property.LocalName != "bcfgitem") continue;
            // to get attributes cast to XmlElement
            XmlElement propertyElement = (XmlElement)property;
            // read info from attributes
            string propertyLabel = propertyElement.GetAttribute("label");
            string propertyHint = propertyElement.GetAttribute("hint");
            if (wayInXml.LastIndexOf("/bcfgitem") == -1)
              wayInXml += "/bcfgitem[@label='" + propertyLabel + "']";
            else
              wayInXml = wayInXml.Substring(0, wayInXml.LastIndexOf("/bcfgitem")) + "/bcfgitem[@label='" + propertyLabel + "']";
            
            // create one entry to property dialog 
            HBox hbox = new HBox(false, 2);
            
            XmlElement propertyValue = (XmlElement)property.FirstChild;
            switch (propertyValue.LocalName)
            {
              case "string":
                Label label = new Label(propertyLabel);
                Entry entry = new Entry(propertyValue.GetAttribute("value"));
                entry.IsEditable = true; entry.Visible = true;
                hbox.PackStart(label, false, false, 1);
                hbox.PackEnd(entry, true, true, 1);
                
                // store the way in Xml to this element
                wayInXml += "/string";
                entry.Name = "item" + (lastItemNumber++).ToString();
                properties.Add(entry.Name, wayInXml);
                break;
              case "bool":
                CheckButton checkButton = CheckButton.NewWithLabel(propertyLabel);
                hbox.PackStart(checkButton, false, false, 1);
                string check = propertyValue.GetAttribute("value");
                checkButton.Active = (check.ToUpper() == "TRUE");
                // add hints to created items
                tooltips.SetTip(checkButton, propertyHint, "");
                
                // store the way in Xml to this element
                wayInXml += "/bool";
                checkButton.Name = "item" + (lastItemNumber++).ToString();
                properties.Add(checkButton.Name, wayInXml);
                break;
              case "int":
                double minimum = Convert.ToDouble(propertyValue.GetAttribute("minimum"));
                double maximum = Convert.ToDouble(propertyValue.GetAttribute("maximum"));
                double step = Convert.ToDouble(propertyValue.GetAttribute("step"));
                string strHodnota = propertyValue.GetAttribute("value");
                double hodnota = (strHodnota != string.Empty) ? Convert.ToDouble(strHodnota) : 1f;
                SpinButton spinner = new SpinButton(minimum, maximum, step);
                spinner.Value = hodnota;
                Label labelInt = new Label(propertyLabel);
                
                hbox.PackStart(labelInt, false, false, 1);
                hbox.PackStart(spinner, false, false, 1);
                
                // store the way in Xml to this element
                wayInXml += "/int";
                spinner.Name = "item" + (lastItemNumber++).ToString();
                properties.Add(spinner.Name, wayInXml);
                break;
              case "select":
                int selected = Convert.ToInt32(propertyValue.GetAttribute("selected"));
                Gtk.Label labelSelect = new Label(propertyLabel);
                Gtk.ComboBox combo = ComboBox.NewText();
                foreach (XmlElement option in propertyValue.ChildNodes)
                {
                  // skip non-valid elements
                  if (option.NodeType != XmlNodeType.Element || option.LocalName != "option") continue;
                  XmlElement optionElement = (XmlElement)option;
                  combo.AppendText(optionElement.GetAttribute("text"));
                }
                combo.Active = selected;
                hbox.PackStart(labelSelect, false, false, 1);
                hbox.PackStart(combo, false, false, 1);
                
                // store the way in Xml to this element
                wayInXml += "/select";
                combo.Name = "item" + (lastItemNumber++).ToString();
                properties.Add(combo.Name, wayInXml);
                break;
            } // switch

            // add hints to created items
            tooltips.SetTip(hbox, propertyHint, "");
            
            // add line to hbox
            vbox.PackStart(hbox, false, false, 6);
          } // for each properties
        } // for each headings
        
        // add new tab to the notebook
        nbProperties.AppendPage(vbox, tabLabel);
      }
		}
  }
}