// project created on 30.3.2006 at 0:09
using System;
using System.Reflection;
using Gtk;
using Glade;
using LibBeline;
using Beline.Gui.Dialogs;

namespace Beline.Gui {
  public class MainWindow : Window
  {
    #region widgets
    [Widget] Statusbar statusbar1;
    [Widget] ScrolledWindow scrolledwindow3;
    [Widget] ScrolledWindow scrolledwindow4;
    [Widget] HandleBox handlebox1;
    [Widget] TreeView treeview2;
    [Widget] MenuItem mnQuit;
    [Widget] Gtk.Menu moduleMenu;
    
    HTML htmlWidget;
    #endregion
    
    #region private variables
    /// Container representing values which will show in the treeveiw
    private TreeStore store;
    private TreeIter tiBenchmark;
    private TreeIter tiInfo;
    private TreeIter tiSetting;
    #endregion
    
    #region properties
    #endregion
    
  	public static void Main (string[] args)
  	{
  		new MainWindow (args);
  	}
  	
    public MainWindow (string[] args) : base(WindowType.Toplevel)
  	{
  		Application.Init ();

  		Glade.XML gxml = new Glade.XML (null, "beline.glade", "main", null);
  		gxml.Autoconnect (this);
  		
  		// initialize plugins
  		try
  		{
  		  LibBeline.LibBeline.InitializeInstance(BEnumSystem.master, "Beline");
  		}
  		catch (Exception e)
  		{
  		  Simple.ErrorAlert("Nepodařilo se zinicializovat moduly", e.Message, this);
  		  Application.Quit(); // ukonci beh aplikace
  		}
  		
  		// create new HTML window (to show results)
  		htmlWidget=new HTML();
  		scrolledwindow3.Show();
      scrolledwindow3.Add(htmlWidget);   // vlozim okno webove konzole do ramce
      
      // load greeting
      string greeting = "<html> <body><div align=center><b>Hello World!</b></div></body> </html>"; //BTemplates.GetInstance().ReadTemplate("greeting.html");
      htmlWidget.LoadFromString(greeting);
      
      // initialize treeview
      RegenerateTreeView();
      
      // assigning events
      treeview2.ButtonPressEvent += new ButtonPressEventHandler(treeview2_popup_menu);
  		
  		Application.Run ();
  	}

  	///<summary>
  	/// Connect the Signals defined in Glade
  	///</summary>
  	private void OnWindowDeleteEvent (object sender, DeleteEventArgs a) 
  	{
  		Application.Quit ();
  		a.RetVal = true;
  	}
  	
  	#region Main menu handlers
  	///<summary> Handle File -> Save as... command from menu</summary>
  	private void mnSave_activate (object sender, EventArgs a)
  	{
  	  Gtk.FileChooserDialog fs = new Gtk.FileChooserDialog("Save file as...", this, FileChooserAction.Save,
  	    Gtk.Stock.Cancel, ResponseType.Cancel,
  	    Gtk.Stock.Save, ResponseType.Accept);
  	    
  	  bool saved = false;
  	  do {
  	    ResponseType response = (ResponseType)fs.Run();
  	    fs.Hide();
  	    
  	    if (response == ResponseType.Accept)
  	    {
  	      if (System.IO.File.Exists(fs.Filename))
  	      {
  	        Simple.ErrorAlert("Chyba při ukládání souboru", "Soubor " + System.IO.Path.GetFileName(fs.Filename) + " již existuje!", this);
  	      }
  	      else
  	      {
  	        Gtk.HTMLSaveReceiverFn saver = new Gtk.HTMLSaveReceiverFn(Tools.FileTools.SaveHtml);
  	        if (!htmlWidget.Save(saver))
  	        {
  	          Simple.ErrorAlert("Nepodařilo uložit soubor.", "Soubor nebyl uložen", this);
  	        }
  	        saved = true;
  	      }
  	    }
  	    else
  	    {
  	      saved=true;
  	    }
  	  } while (!saved);
  	  
  	  fs.Destroy();
  	}

  	
  	private void mnPrintPreview_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnTisk_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnCopy_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnSelectAll_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnFind_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnFindNext_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnPreferencesModules_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnPreferences_activate (object sender, EventArgs a)
  	{
  	}
  	
  	/// change visibility of main toolbar
  	private void mnShowToolBar_activate (object sender, EventArgs a)
  	{
  	  handlebox1.Visible = !handlebox1.Visible;
  	}
  	
  	/// change visibility of statusbar
  	private void mnShowStatusBar_activate (object sender, EventArgs a)
  	{
  	  statusbar1.Visible = !statusbar1.Visible;
  	}
  	
  	/// change visibility of side bar with list of modules
  	private void mnShowSideBar_activate (object sender, EventArgs a)
  	{
  	  scrolledwindow4.Visible = !scrolledwindow4.Visible;
  	}
  	
  	private void mnShowIcons_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnShowTree_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnFontUp_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnFontDown_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnFontNormal_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnRefresh_activate (object sender, EventArgs a)
  	{
  	}
  	
  	private void mnHelp_activate (object sender, EventArgs a)
  	{
  	}
  	
  	///<summary> Handle Help -> About command from menu</summary>
  	private void mnAbout_activate (object sender, EventArgs a)
  	{
  	  Assembly asmbly = Assembly.GetEntryAssembly();
		  string version = "", name="", description="", copyright="";
		  Attribute attrib = Attribute.GetCustomAttribute(asmbly, typeof(AssemblyTitleAttribute));
		  if (attrib != null)
		    name = ((AssemblyTitleAttribute)attrib).Title;
		    
		  attrib = Attribute.GetCustomAttribute(asmbly, typeof(AssemblyConfigurationAttribute));
	    if (attrib != null)
	      version = ((AssemblyConfigurationAttribute)attrib).Configuration;
	       		
	    attrib = Attribute.GetCustomAttribute(asmbly, typeof(AssemblyDescriptionAttribute));
  		if (attrib != null)
  		  description = ((AssemblyDescriptionAttribute)attrib).Description;
  		  
  		attrib = Attribute.GetCustomAttribute(asmbly, typeof(AssemblyCopyrightAttribute));
  		if (attrib != null)
  		  copyright = ((AssemblyCopyrightAttribute)attrib).Copyright;
  		  
  	  AboutWindow about = new AboutWindow();
  	  about.Show(name, version, description, copyright);
  	}
  	
  	private void mnQuit_activate (object sender, EventArgs a)
  	{
  	  Application.Quit();
  	}
  	#endregion
  	
  	#region treeveiw handlers
  	[GLib.ConnectBefore]
  	private void treeview2_popup_menu(object sender, ButtonPressEventArgs a)
  	{
  	  // get selected node
      TreeIter iter;
      TreeModel model;
      string oid;

      if (a.Event.Button == 3)
      { // right button clicked
        if (treeview2.Selection.GetSelected(out model, out iter))
        {
          oid = (string) model.GetValue (iter, 1);
    	  
    	    // tree roots havn't tooltip menu
    	    if (oid == "BelineInf" || oid == "BelineBench" || oid == "BelineSet") return;
    	    
      	  ContextMenu moduleMenu = new ContextMenu();
      	  try
      	  {
      	   moduleMenu.Popup(oid, this);
      	  }
          catch (Exception e)
          {
            Simple.ErrorAlert("Chyba v modulu", "Nemohu se spojit s modulem.", this);
          }
        }
      }
  	}
  
    private void RegenerateTreeView()
    {
      store = new TreeStore (typeof (string), typeof (string));
      // initialize roots
      tiInfo = store.AppendValues("Informace", "BelineInf");
      tiBenchmark = store.AppendValues("Benchmark (test výkonu)", "BelineBench");
      tiSetting = store.AppendValues("Nastavení parametrů HW", "BelineSet");
      
      // initialize treeview
      treeview2.AppendColumn ("Nazev", new CellRendererText(), "text", 0);
      treeview2.AppendColumn ("Oid", new CellRendererText(), "text", 1);
      treeview2.Columns[1].Visible = false;
      treeview2.Model = store;
      
      // get all modules
      try
      {
        BModuleItem[] modules = BModuleManager.GetInstance().GetAllModules();
        foreach (BModuleItem module in modules)
        {
          string moduleType = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID)["/beline/conf/module/private[type]"].ToString();
          switch (moduleType)
          {
            case "info":
              store.AppendValues(tiInfo, module.Name, module.OID);
              break;
            case "benchmark":
              store.AppendValues(tiBenchmark, module.Name, module.OID);
              break;
            case "setting":
              store.AppendValues(tiSetting, module.Name, module.OID);
              break;
            default:
              throw new Exception("Bad module type. Should be one of \"info\", \"benchmark\", \"setting\".");
    		  }
    		}
      }
      catch (Exception e)
      {
        Simple.ErrorAlert("Nepodařilo se zinicializovat strom modulů", e.Message, this);
  		  Application.Quit(); // ukonci beh aplikace
      }
    }
    #endregion
  }
} // namespace

