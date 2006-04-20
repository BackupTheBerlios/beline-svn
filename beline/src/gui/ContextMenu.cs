// created on 11.4.2006 at 20:50
using Gtk;
using System;
using Glade;
using LibBeline;
using Beline.Gui.Dialogs;

namespace Beline.Gui
{
  public class ContextMenu : Gtk.Menu
  {
    Gtk.MenuItem mnModuleMenuSettings;
    Gtk.MenuItem mnModuleMenuAbout;
    Gtk.Window parent;
    
    /// OID of module above which we show the ContextMenu
    string OID;
    
    public ContextMenu()
    {
      // create menu items
      mnModuleMenuSettings = new MenuItem("Nastavení");
      mnModuleMenuAbout = new MenuItem("Vlastnosti");
      
      // connect events
      mnModuleMenuSettings.Activated += new EventHandler(mnModuleMenuSettings_activate);
      mnModuleMenuAbout.Activated += new EventHandler(mnModuleMenuAbout_activate);
		  
		  this.Add(mnModuleMenuSettings);
		  this.Add(mnModuleMenuAbout);
		  this.ShowAll();
    }
    
    /// show popup menu for module given by its OID
    public void Popup(string aOID, Gtk.Window aParent)
    {
      OID = aOID;
      parent = aParent;
      this.Popup(null, null, null, 0, Gtk.Global.CurrentEventTime);
    }
      	
  	private void mnModuleMenuSettings_activate(object sender, EventArgs a)
  	{
  	  BModuleItem module = BModuleManager.GetInstance().GetModule(OID);
  	  BConfigItem config = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID);
  	  
  	  LibBeline.Gui.ModulePropertyDialog dialog = new LibBeline.Gui.ModulePropertyDialog(config, parent);
  	  dialog.Run();
  	  dialog.Dispose();
  	}
  	
  	private void mnModuleMenuAbout_activate(object sender, EventArgs a)
  	{
  	  BModuleItem module = BModuleManager.GetInstance().GetModule(OID);
  	  BConfigItem config = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID);
  	  
  	  string name="", version="", description="";
  	  BValueType hodnota = config["/beline/conf/module[name]"];
  	  if (hodnota != null) name = hodnota.ToString();
  	  hodnota = config["/beline/conf/module[version]"];
  	  if (hodnota != null) version = hodnota.ToString();
  	  hodnota = config["/beline/conf/module[description]"];
  	  if (hodnota != null) description = hodnota.ToString();
  	  
  	  AboutWindow about = new AboutWindow();
  	  about.Show(name, version, description, "");
  	}
  }
}