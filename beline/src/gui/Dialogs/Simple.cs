// created on 3.4.2006 at 22:44
using Gtk;

namespace Beline.Gui.Dialogs
{
  class Simple
  {
		private static Gtk.Dialog Alert(string primary, string secondary, Image aImage, Gtk.Window parent) 
		{
		  Gtk.Dialog retval = new Gtk.Dialog("", parent, Gtk.DialogFlags.DestroyWithParent);
		  
		  // graphic items
		  Gtk.HBox hbox;
  		Gtk.VBox labelBox;
  		Gtk.Label labelPrimary;
  		Gtk.Label labelSecondary;
  		
			// set-up dialog
			retval.Modal=true;
			retval.BorderWidth=6;
			retval.HasSeparator=false;
			retval.Resizable=false;
		
			retval.VBox.Spacing=12;
		
			hbox=new Gtk.HBox();
			hbox.Spacing=12;
			hbox.BorderWidth=6;
			retval.VBox.Add(hbox);
			
			// set-up image
			aImage.Yalign=0.0F;
			hbox.Add(aImage);
			
			// set-up labels
			labelPrimary=new Gtk.Label();
			labelPrimary.Yalign=0.0F;
			labelPrimary.Xalign=0.0F;
			labelPrimary.UseMarkup=true;
			labelPrimary.Wrap=true;
			
			labelSecondary=new Gtk.Label();
			labelSecondary.Yalign=0.0F;
			labelSecondary.Xalign=0.0F;
			labelSecondary.UseMarkup=true;
			labelSecondary.Wrap=true;
			
			labelPrimary.Markup="<span weight=\"bold\" size=\"larger\">"+primary+"</span>";
			labelSecondary.Markup="\n"+secondary;
			
			labelBox=new VBox();
			labelBox.Add(labelPrimary);
			labelBox.Add(labelSecondary);
			
			hbox.Add(labelBox);
			
			return retval;
		}
		
		public static void ErrorAlert(string primary, string secondary, Gtk.Window parent) 
		{
		  Gtk.Image image = new Gtk.Image();
		  image.SetFromStock(Gtk.Stock.DialogError, Gtk.IconSize.Dialog);
		  Gtk.Dialog dialog = Alert(primary, secondary, image, parent);
		  // prepare error dialog
		  
			dialog.AddButton(Gtk.Stock.Ok, ResponseType.Ok);
			dialog.ShowAll();
			
			// run dialog
			dialog.Run();
			dialog.Destroy();
		}
  } // class Dialogs
} // namespace