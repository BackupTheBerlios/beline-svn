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
  public sealed class ModulePropertyDialog: PropertyDialog
  {
    private BConfigItem configItem;
    
    /// Create new instance of dialog showing module's properties
    /// <param name="configItem">The object with module's configuration</param>
    /// <param name="parent">Pointer to parent window (this dialog will be closed when window will be destroyed)</param>
    public ModulePropertyDialog(BConfigItem configItem, Gtk.Window parent)
     : base(parent)
    {
      this.configItem = configItem;
      // initialize dialog's widgets
      InitializeDialog(configItem, "/beline/conf/module/configuration");
      
      // connect signals to handlers
      btnClose.Clicked += new EventHandler(btnClose_click);
    }
    
		///<summary>Handler for closing button (try save changes to default local configuration file)</summary>
		private void btnClose_click (object sender, EventArgs a)
		{
      Save(configItem);
		}
  }
}