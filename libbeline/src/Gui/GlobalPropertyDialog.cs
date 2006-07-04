// created on 5.5.2006 at 23:05
using LibBeline;
using Gtk;
using Glade;
using System;
using System.Collections;
using System.Xml;

namespace LibBeline.Gui
{
  /// Window that shows module property
  public sealed class GlobalPropertyDialog: PropertyDialog
  {
    
    public GlobalPropertyDialog(Gtk.Window parent)
      : base(parent)
    {
      // initialize dialog's widgets
      this.InitializeDialog(BConfigManager.GetInstance().GlobalConf, "/beline/conf/global/configuration");
      
      // connect signals to handlers
      btnClose.Clicked += new EventHandler(btnClose_click);
    }
		
		///<summary>Handler for closing button (try save changes to default local configuration file)</summary>
		private void btnClose_click (object sender, EventArgs a)
		{
      Save(BConfigManager.GetInstance().GlobalConf);
		}

		
		///<summary>Initialize dialog's widgets by private section of the configuration file</summary>
		protected override void InitializeDialog(BConfigItem configItem, string wayInXml)
		{  
      base.InitializeDialog(configItem, wayInXml);
      
      Label tabLabel = new Label("Libbeline");
      // add hint to created fold
      tooltips.SetTip(tabLabel, "Konfigurace knihovny libbeline", "Konfigurace knihovny libbeline");

      VBox vbox = new VBox(false, 4);
 
      //*** subtree limits       
      Label headingLabel = new Label();
      headingLabel.Markup = "<b>Limity</b>";
      headingLabel.Xalign = 0.0f;
      headingLabel.Xpad = 10; headingLabel.Ypad = 3;
      vbox.PackStart(headingLabel, false, false, 6);
      
      string timeoutValue = configItem["/beline/conf/global/limit[@defaulttimeout]"].ToString();
      AddIntItem(vbox, "Timeout [s]", timeoutValue, "Defaultní timeout pro moduly", "60", 
        "600", "10", "/beline/conf/global/limit[@defaulttimeout]");
      
      //*** subtree paths
      headingLabel = new Label();
      headingLabel.Markup = "<b>Cesty</b>";
      headingLabel.Xalign = 0.0f;
      headingLabel.Xpad = 10; headingLabel.Ypad = 3;
      vbox.PackStart(headingLabel, false, false, 6);
      
      // templates     
      string templatesValue = configItem["/beline/conf/global/paths[@templates]"].ToString();
      AddStringItem(vbox, "Šablony", templatesValue, "Cesta k šablonám zpráv", 
        "/beline/conf/global/paths[@templates]");
      // fifos
      string fifosValue = configItem["/beline/conf/global/paths[@fifos]"].ToString();
      AddStringItem(vbox, "Dočasný adresář", fifosValue, 
        "Celá cesta k temp adresáři, ve kterém je možné odkládat dočasné soubory", 
        "/beline/conf/global/paths[@fifos]");

      // add new tab to the notebook
      nbProperties.AppendPage(vbox, tabLabel);
		}
  }
}