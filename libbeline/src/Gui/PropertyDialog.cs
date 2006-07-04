// created on 5.5.2006 at 23:09
using LibBeline;
using Gtk;
using Glade;
using System;
using System.Collections;
using System.Xml;

namespace LibBeline.Gui
{
  /// Window that shows module property
  public class PropertyDialog: IDisposable
  {
    #region widgets
    [Widget ("PropertiesDialog")] protected Dialog PropertiesDialog;
    [Widget] protected VBox vbox1;
    [Widget] protected Gtk.Notebook nbProperties;
    [Widget] protected Gtk.Button btnHelp;
    [Widget] protected Gtk.Button btnClose;
    protected Gtk.Tooltips tooltips;
    #endregion
    
    // container to pairs (wayToElementInXml, propertyName)
    protected Hashtable properties;
    protected int lastItemNumber;
    
    public PropertyDialog(Gtk.Window parent)// : base("", parent, DialogFlags.DestroyWithParent)
    {
      properties = new Hashtable();
      lastItemNumber = 0;
      tooltips = new Gtk.Tooltips();
      
      // create dialog using glade
      Glade.XML gxml = new Glade.XML(null, "dialogs.glade", "PropertiesDialog", null);
      gxml.Autoconnect(this);
    }
    
    public virtual bool Run ()
		{
			PropertiesDialog.ShowAll ();
			PropertiesDialog.Run();
			return true;
		}
    
    public virtual void Dispose ()
		{
			PropertiesDialog.Destroy ();
			PropertiesDialog.Dispose ();
		}
		
		
		/// Store attribute's XPath to properties hashtable for futher processing
		/// <param name="name">Name of element (key to hashtable)</param>
		/// <param name="wayInXml">Way to the attribute whose element it is (including an attribute name at the end of way)</param>
		protected void StoreToProperties(string name, string wayInXml)
		{
		  string attributeName = wayInXml.Substring(wayInXml.LastIndexOf('@') + 1);
		  attributeName = attributeName.Trim(']');  // trim last ]
		  string wayToElement = wayInXml.Substring(0, wayInXml.LastIndexOf("[@"));
		  
		  string[] propertyStore = {wayToElement, attributeName};
      properties.Add(name, propertyStore);
		}
		
		/// Add new editable entry to the dialog with label and default value
		/// <param name="page">An object that represents dialog (a container of items on the dialog)</param>
		/// <param name="propertyLabel">Label that will be shown before the entry</param>
		/// <param name="propertyValue">Default value that will be preset to the entry</param>
		/// <param name="wayInXml">A XPath path to the element, whose entry is created</param>
		protected void AddStringItem(Gtk.Box page, string propertyLabel, string propertyValue, string toolTip, string wayInXml)
		{
		  // create one entry to property dialog 
      HBox hbox = new HBox(false, 3);
      Label vypln = new Label(); vypln.WidthRequest = 18; hbox.PackStart(vypln, false, false, 1);
      Label label = new Label(propertyLabel);
      Entry entry = new Entry(propertyValue);
      entry.IsEditable = true; entry.Visible = true;
      hbox.PackStart(label, false, false, 1);
      hbox.PackEnd(entry, true, true, 1);
      page.PackStart(hbox, false, false, 1);
      
      // add entry to list of properies (useful during saving)
      entry.Name = "item" + (lastItemNumber++).ToString();
      StoreToProperties(entry.Name, wayInXml);
		}

		/// Add new checkbox to the dialog with label and default value
		/// <param name="page">An object that represents dialog (a container of items on the dialog)</param>
		/// <param name="propertyLabel">Label that will be shown before the checkbox</param>
		/// <param name="propertyValue">Default value that will be preset to the checkbox (should be one of True or False)</param>
		/// <param name="propertyHint">Hint that will be shown when mouse goes over a checkbox</param>
		/// <param name="wayInXml">A XPath path to the element, whose checkbox is created</param> 
		protected void AddBoolItem(Gtk.Box page, string propertyLabel, string propertyValue, string propertyHint, string wayInXml)
		{
		  // create one entry to property dialog 
      HBox hbox = new HBox(false, 3);
      Label vypln = new Label(); vypln.WidthRequest = 18; hbox.PackStart(vypln, false, false, 1);
      CheckButton checkButton = CheckButton.NewWithLabel(propertyLabel);
      hbox.PackStart(checkButton, false, false, 1);
      checkButton.Active = (propertyValue.ToUpper() == "TRUE");
      // add hints to created items
      tooltips.SetTip(checkButton, propertyHint, "");
      page.PackStart(hbox, false, false, 1);

      // add checkbox to list of properies (useful during saving)
      checkButton.Name = "item" + (lastItemNumber++).ToString();
      StoreToProperties(checkButton.Name, wayInXml);
    }
    
		/// Add new spinbutton to the dialog with label and default value
		/// <param name="page">An object that represents dialog (a container of items on the dialog)</param>
		/// <param name="propertyLabel">Label that will be shown before the spinbutton</param>
		/// <param name="propertyValue">Default value that will be preset to the spinbutton (should be one of True or False)</param>
		/// <param name="propertyHint">Hint that will be shown when mouse goes over a spinbutton</param>
		/// <param name="wayInXml">A XPath path to the element, whose spinbutton is created</param>
		protected void AddIntItem(Gtk.Box page, string propertyLabel, string propertyValue, string propertyHint, string minimum, string maximum, string step, string wayInXml)
		{
      double dMinimum = Convert.ToDouble(minimum);
      double dMaximum = Convert.ToDouble(maximum);
      double dStep = Convert.ToDouble(step);
      double hodnota = (propertyValue != string.Empty) ? Convert.ToDouble(propertyValue) : 1f;
      if (hodnota < dMinimum) hodnota=dMinimum;
      if (hodnota > dMaximum) hodnota=dMaximum;
      
      // create spinbutton to represent value
      HBox hbox = new HBox(false, 3);
      Label vypln = new Label(); vypln.WidthRequest = 18; hbox.PackStart(vypln, false, false, 1);
      SpinButton spinner = new SpinButton(dMinimum, dMaximum, dStep);
      spinner.Value = hodnota;
      Label labelInt = new Label(propertyLabel);
      
      hbox.PackStart(labelInt, false, false, 1);
      hbox.PackStart(spinner, false, false, 1);
      page.PackStart(hbox, false, false, 1);
      
      // store the way in Xml to this element
      spinner.Name = "item" + (lastItemNumber++).ToString();
      StoreToProperties(spinner.Name, wayInXml);
    }
    
		/// Add new combobox to the dialog with label, filled item and default value
		/// <param name="page">An object that represents dialog (a container of items on the dialog)</param>
		/// <param name="propertyLabel">Label that will be shown before the combobox</param>
		/// <param name="propertySelected">Default value that will be preset to the combobox (should be one of True or False)</param>
		/// <param name="propertyHint">Hint that will be shown when mouse goes over a combobox</param>
		/// <param name="options">An array of strings that represents options of combobox</param>
		/// <param name="wayInXml">A XPath path to the element, whose combobox is created</param>
		protected void AddComboItem(Gtk.Box page, string propertyLabel, string propertySelected, string propertyHint, ArrayList options, string wayInXml)
		{
      int selected = Convert.ToInt32(propertySelected);
      HBox hbox = new HBox(false, 3);
      Label vypln = new Label(); vypln.WidthRequest = 18; hbox.PackStart(vypln, false, false, 1);
      Gtk.Label labelSelect = new Label(propertyLabel);
      Gtk.ComboBox combo = ComboBox.NewText();
      foreach (String option in options)
      {
        combo.AppendText(option);
      }
      // preselect proper item
      if (selected < options.Count && selected >= 0) combo.Active = selected;
      hbox.PackStart(labelSelect, false, false, 1);
      hbox.PackStart(combo, false, false, 1);
      page.PackStart(hbox, false, false, 1);

      // store the way in Xml to this element
      combo.Name = "item" + (lastItemNumber++).ToString();
      StoreToProperties(combo.Name, wayInXml);
    }
		
		/// Go throught all widgets and save changes made by user back to the config item
		/// and then call SaveConfigToFile to default configuration file
		/// <param name="configItem">The configuration object whose information should be saved</param>
		protected virtual void Save(BConfigItem configItem)
		{
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
	            string [] property = (string[])properties[item.Name];
	            if (item.GetType() == typeof(Gtk.Entry))
	              configItem.SetAttribute(property[0], property[1], ((Gtk.Entry)item).Text);
	            else if (item.GetType() == typeof(Gtk.SpinButton))
	              configItem.SetAttribute(property[0], property[1], ((Gtk.SpinButton)item).Value.ToString());
	            else if (item.GetType() == typeof(Gtk.CheckButton))
	              configItem.SetAttribute(property[0], property[1], ((Gtk.CheckButton)item).Active.ToString());
	            else if (item.GetType() == typeof(Gtk.ComboBox))
	              configItem.SetAttribute(property[0], property[1], ((Gtk.ComboBox)item).Active.ToString());
	          }
	        }
	      }
      }
		  
		  // save to ~/.beline/messages config file
		  configItem.SaveConfigToFile(null);
		}
		
		
		///<summary>Initialize dialog's widgets by private section of the configuration file</summary>
		///<param name="configItem">The peice of configuration (global, module etc.) that should be dispalyed</param>
		///<param name="wayInXml">Way to the root element of custom configuration in configItem</param>
		protected virtual void InitializeDialog(BConfigItem configItem, string wayInXml)
		{
      // create notebook tabs with items
      XmlNode configure = configItem.GetXmlNode(wayInXml);
      
      if (configure == null)
      { // no private configuration => nothing to build
        return;
      }
      
      //Console.WriteLine(configure.ChildNodes.Count);
      // go throught all folds
      foreach (XmlNode fold in configure.ChildNodes)
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
            // this is first iteration of loop, so no heading in string
            wayInXml += "/heading[@text='" + headingText + "']";
          else
            // this is another iteration of loop, so I must cut heading from prevoius iteration and then append new
            wayInXml = wayInXml.Substring(0, wayInXml.LastIndexOf("/heading")) + "/heading[@text='" + headingText + "']";
          
          Label headingLabel = new Label();
          headingLabel.Markup = "<b>" + headingText + "</b>";
          headingLabel.Xalign = 0.0f;
          headingLabel.Xpad = 10; headingLabel.Ypad = 3;
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
              // this is first iteration of loop, so no bcfgitem in string
              wayInXml += "/bcfgitem[@label='" + propertyLabel + "']";
            else
              // this is another iteration of loop, so I must cut bcfgitem from prevoius iteration and then append new
              wayInXml = wayInXml.Substring(0, wayInXml.LastIndexOf("/bcfgitem")) + "/bcfgitem[@label='" + propertyLabel + "']";
            
            XmlElement propertyValue = (XmlElement)property.FirstChild;
            switch (propertyValue.LocalName)
            {
              case "string":
                // store the way in Xml to this element
                wayInXml += "/string[@value]";
                AddStringItem(vbox, propertyLabel, propertyValue.GetAttribute("value"), 
                  propertyHint, wayInXml);
                break;
              case "bool":
                // store the way in Xml to this element
                wayInXml += "/bool[@value]";
                AddBoolItem(vbox, propertyLabel, propertyValue.GetAttribute("value"),
                  propertyHint, wayInXml);
                break;
              case "int":
                // store the way in Xml to this element
                wayInXml += "/int[@value]";
                AddIntItem(vbox, propertyLabel, propertyValue.GetAttribute("value"),
                  propertyHint, propertyValue.GetAttribute("minimum"), propertyValue.GetAttribute("maximum"),
                  propertyValue.GetAttribute("step"), wayInXml);
                break;
              case "select":
                // store the way in Xml to this element
                wayInXml += "/select[@selected]";
                ArrayList options = new ArrayList(propertyValue.ChildNodes.Count);
                foreach (XmlElement option in propertyValue.ChildNodes)
                { // prepair array for an AddComboItem method
                  XmlElement optionElement = (XmlElement)option;
                  options.Add(optionElement.GetAttribute("text"));
                }
                AddComboItem(vbox, propertyLabel, propertyValue.GetAttribute("selected"),
                  propertyHint, options, wayInXml);
                break;
            } // switch
          } // for each properties
        } // for each headings
        
        // add new tab to the notebook
        nbProperties.AppendPage(vbox, tabLabel);
      }
		}
  }
}