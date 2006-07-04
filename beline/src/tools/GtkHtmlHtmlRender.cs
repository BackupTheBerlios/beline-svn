//
//
// GtkHtmlHtmlRender.cs: Implementation of IHtmlRender that uses Gtk.HTML
// Author: Mario Sopena
// Author: Rafael Ferreira <raf@ophion.org>
//
// This code is based on a Mario Sopena's and Rafael Ferreira's GtkHtmlHtmlRender class
// in the monodoc-browser project
using System;
using Gtk;
using Gnome;
using System.IO;
using System.Reflection;
using Beline.Gui.Dialogs;

namespace Beline.Tools {
class GtkHtmlHtmlRender : IHtmlRender {
	
	Gtk.Window parentWindow;
	HTML html_panel;
	
	public Widget HtmlPanel {
		get { return (Widget) html_panel; }
	}

	public event EventHandler OnUrl;
	public event EventHandler UrlClicked;
	public event SubmitHandler OnSubmit;
	
	/// Create new instance of Html Renderer
	public GtkHtmlHtmlRender (Gtk.Window parent) 
	{
	  this.parentWindow = parent;
		html_panel = new HTML();
		html_panel.Show();
		html_panel.Submit += new SubmitHandler(SubmitRequest);
		html_panel.UrlRequested += new UrlRequestedHandler (UrlRequested);
	}
	
  // catch sumit event and send it to observers
  protected void SubmitRequest(object o, SubmitArgs args)
  {
    OnSubmit(o, args);
  }

	public void JumpToAnchor (string anchor)
	{
		html_panel.JumpToAnchor(anchor);
	}

	public void Copy () 
	{
		html_panel.Copy();	
	}

	public void SelectAll () 
	{
		html_panel.SelectAll();	
	}

	public void Render (string html_code) 
	{
		Gtk.HTMLStream stream = html_panel.Begin ("text/html");
		stream.Write(html_code);
		html_panel.End (stream, HTMLStreamStatus.Ok);
	}

	static Stream GetBrowserResourceImage ()
	{
		System.IO.Stream s = File.OpenRead("/etc/libbeline/images/default.png");
		
		return s;
	}

	protected void UrlRequested (object sender, UrlRequestedArgs args)
	{
		Stream s = File.OpenRead(args.Url);
		if (s == null)
		  // nejaky prazdny obrazek to chce
			s = GetBrowserResourceImage ();
		byte [] buffer = new byte [8192];
		int n, m;
		m=0;
		while ((n = s.Read (buffer, 0, 8192)) != 0) {
			args.Handle.Write (buffer, n);
			m += n;
		}
		args.Handle.Close (HTMLStreamStatus.Ok);
	}
	
	/// Save content of html widget to a file
	/// <param name="filename">The destination file</param>
	public bool Save(string filename)
	{
	  Beline.Tools.SaveHtmlTool saver = new Beline.Tools.SaveHtmlTool();
    saver.FileName = filename;
    while (!html_panel.Save(saver.AppendHtml))
    {}
  	        
    return saver.Save();
  }

  public void ZoomIn()
  {
    html_panel.ZoomIn();
  }

  public void ZoomOut()
  {
    html_panel.ZoomOut();
  }
  
  public void ZoomReset()
  {
    html_panel.ZoomReset();
  }

	
	public void Print () {
		
//		if (Html == null || Html == string.Empty) {
//			Simple.ErrorAlert("Chyba při tisku", "Není co tisknout, prázdný dokument", parentWindow);
//			return;
//		}
//
		string Caption = "Beline Printing";

		PrintJob pj = new PrintJob (PrintConfig.Default ());
		PrintDialog dialog = new PrintDialog (pj, Caption, 0);

//		Gtk.HTML gtk_html = new Gtk.HTML (Html);
		html_panel.PrintSetMaster (pj);
			
		PrintContext ctx = pj.Context;
		html_panel.Print (ctx);

		pj.Close ();

		// hello user
		int response = dialog.Run ();
		
		if (response == (int) PrintButtons.Cancel) {
			dialog.Hide ();
			dialog.Dispose ();
			return;
		} else if (response == (int) PrintButtons.Print) {
			pj.Print ();
		} else if (response == (int) PrintButtons.Preview) {
			new PrintJobPreview (pj, Caption).Show ();
		}
		
		ctx.Close ();
		dialog.Hide ();
		dialog.Dispose ();
		return;
	}
}
}
