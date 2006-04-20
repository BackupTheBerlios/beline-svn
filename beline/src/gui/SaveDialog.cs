// created on 30.3.2006 at 21:53
using System;
using Gtk;
using GtkSharp;
using Glade;

namespace Beline.Gui
{
	public class SaveDialog : Dialog
	{
		public SaveDialog () : base (GType)
		{
		  Glade.XML gxml = new Glade.XML (null,"beline.glade", "savedialog", null);
      gxml.Autoconnect (this);
      
		}
	}
}
