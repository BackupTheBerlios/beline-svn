// created on 3.4.2006 at 23:26
using Gtk;
using System;
using Glade;

namespace Beline.Gui
{
  ///<summary>A singleton About Dialog</summary>
  public class AboutWindow: Gtk.Window
  {
    [Widget] Gtk.VBox AboutTopBox;
  	[Widget] Gtk.Image imgLogo;
  	[Widget] Gtk.Button btnClose;
  	[Widget] Gtk.Label lblName;
  	[Widget] Gtk.Label lblInfo;
  	[Widget] Gtk.Label lblCopyright;

	  public AboutWindow() : base ("")
	  {
		  Glade.XML gxml = new Glade.XML (null, "beline.glade", "AboutTopBox", null);
		  gxml.Autoconnect (this);

  		this.TypeHint=Gdk.WindowTypeHint.Dialog;
  		this.Add(AboutTopBox);
  		this.ShowAll();
	  }
	
  	private void btnClose_clicked(object o, EventArgs args)
  	{
  		this.Destroy();
  	}
	
  	protected override bool OnDeleteEvent(Gdk.Event e)
    {
  		this.Destroy();
  		return true;
  	}
	
  	///<summary>Show the dialog. Only one About dialog can be shown at a time.</summmary>
  	public void Show(string aName, string aVersion, string aDescription, string aCopyright)
  	{
      lblName.Markup = String.Format("<span size=\"x-large\" weight=\"bold\">{0} {1}</span>", aName, aVersion);
  		lblInfo.Markup = aDescription;
  		lblCopyright.Markup = aCopyright;
  		
  		this.Present();
  	}
}
} // namespace