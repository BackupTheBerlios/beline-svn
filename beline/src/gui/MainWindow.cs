// project created on 30.3.2006 at 0:09
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml;
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
    [Widget] Gtk.ToolButton tbPrint;
    [Widget] Gtk.ToolButton tbSave;
    [Widget] Gtk.ToolButton tbSettings;
    [Widget] Gtk.ToolButton tbExit;
    [Widget] Gtk.ToolButton tbHelp;
    
    Beline.Tools.IHtmlRender htmlRender;
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
  		scrolledwindow3.Visible = true;
  		htmlRender = new Beline.Tools.GtkHtmlHtmlRender(this);
      scrolledwindow3.Add(htmlRender.HtmlPanel);   // vlozim okno webove konzole do ramce
      
      // load greeting
      string greeting;
      if (!File.Exists("/etc/libbeline/templates/greeting.htm"))
        greeting = "";
      else
      {
        try
        {
          StreamReader fileGreeting = File.OpenText("/etc/libbeline/templates/greeting.htm");
          greeting = fileGreeting.ReadToEnd();
          fileGreeting.Close();
        }
        catch (Exception)
        {
          greeting = "";
        }
      }
      htmlRender.Render(greeting);
      
      // initialize treeview
      RegenerateTreeView();
      
      // assigning events
      treeview2.ButtonReleaseEvent += new ButtonReleaseEventHandler(treeview2_popup_menu);
      treeview2.CursorChanged += new EventHandler(treeview2_cursor_changed);
      tbSave.Clicked += new EventHandler(mnSave_activate);
      tbPrint.Clicked += new EventHandler(mnTisk_activate);
      tbSettings.Clicked += new EventHandler(mnPreferences_activate);
      tbExit.Clicked += new EventHandler(mnQuit_activate);
      htmlRender.OnSubmit += new SubmitHandler(htmlWidget_submit);
  		
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
  	        if (!htmlRender.Save(fs.Filename))
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
  	  htmlRender.Print();
  	}
  	
  	/// <summary>Handle Edit->Copy command from menu</summary>
  	private void mnCopy_activate (object sender, EventArgs a)
  	{
  	  htmlRender.Copy();
  	}
  	
  	/// <summary>Handle Edit->Select All command from menu</summary>
  	private void mnSelectAll_activate (object sender, EventArgs a)
  	{
  	  htmlRender.SelectAll();
  	}
  	
  	/// <summary>Handle Edit->Find command from menu</summary>
  	private void mnFind_activate (object sender, EventArgs a)
  	{
  	}
  	
  	/// <summary>Handle Edit->Find Next command from menu</summary>
  	private void mnFindNext_activate (object sender, EventArgs a)
  	{
  	}
  	
  	/// <summary>Handle Edit->Module preferences command from menu</summary>
  	private void mnPreferencesModules_activate (object sender, EventArgs a)
  	{
  	  TreeIter iter;
      TreeModel model;
      string oid;
      
      if (treeview2.Selection.GetSelected(out model, out iter))
      {
        oid = (string) model.GetValue (iter, 1);
    	  
    	  // tree roots can't be run
    	  if (oid == "BelineInf" || oid == "BelineBench" || oid == "BelineSet") return;
 
    	  BModuleItem module = BModuleManager.GetInstance().GetModule(oid);
    	  BConfigItem config = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID);
    	  
    	  LibBeline.Gui.ModulePropertyDialog dialog = new LibBeline.Gui.ModulePropertyDialog(config, this);
    	  dialog.Run();
    	  dialog.Dispose();
  	  }
    }
  	
  	/// <summary>Handle Edit->Preferences command from menu</summary>
  	private void mnPreferences_activate (object sender, EventArgs a)
    {
  	  LibBeline.Gui.GlobalPropertyDialog dialog = new LibBeline.Gui.GlobalPropertyDialog(this);
  	  dialog.Run();
  	  dialog.Dispose();
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
  	  htmlRender.ZoomIn();
  	}
  	
  	private void mnFontDown_activate (object sender, EventArgs a)
  	{
  	  htmlRender.ZoomOut();
  	}
  	
  	private void mnFontNormal_activate (object sender, EventArgs a)
  	{
  	  htmlRender.ZoomReset();
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
  	/// Handler of event "Change selected row" on a treeview widget
  	private void treeview2_cursor_changed(object sender, EventArgs a)
  	{
  	  // get selected node
      TreeIter iter;
      TreeModel model;
      string oid;
      
      if (treeview2.Selection.GetSelected(out model, out iter))
      {
        oid = (string) model.GetValue (iter, 1);

    	  // tree roots can't be run
    	  if (oid == "BelineInf" || oid == "BelineBench" || oid == "BelineSet") return;
    	  
	      string output = Beline.Tools.XslTransform.CreateProcedureHtml(oid);
	      htmlRender.Render(output);
    	}
  	}
  	
  	//[GLib.ConnectBefore]
  	private void treeview2_popup_menu(object sender, ButtonReleaseEventArgs a)
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
          string moduleType = BConfigManager.GetInstance().GetModuleConfig(module.ConfigOID)["/beline/conf/module/configuration[@type]"].ToString();
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
    
    #region toolbar handlers
    #endregion
    
    #region html widget handlers
    // handler for submiting htmlWidget
    public void htmlWidget_submit(object o, SubmitArgs args)
    {
      // prepare parameters
      // if at least one parameter exists, trim preambule
      string paramLine = (args.Encoding.Length > 8 ? args.Encoding.Substring(8) : "");
      string[] parametry = paramLine.Split('&');
      ArrayList parameters = new ArrayList(parametry.Length);
      
      foreach (string param in parametry)
      {
        // combobox has '+' at the end of selected value, so remove it
        string[] parameter = param.Trim('+').Split('=');
        if (parameter.Length >= 2)
          // if there is no parameter, array have only one (empty) item, so do not add it 
          parameters.Add(new BString(parameter[0], parameter[1]));
      }
      
  	  // execute procedure
      TreeIter iter;
      TreeModel model;
      string oid;
      
      if (treeview2.Selection.GetSelected(out model, out iter))
      {
        oid = (string) model.GetValue (iter, 1);
    	  
    	  // tree roots can't be run
    	  if (oid == "BelineInf" || oid == "BelineBench" || oid == "BelineSet") return;
    	  
    	  BMasterServiceManager manager = BMasterServiceManager.GetInstance();
    	  string transaction = null;
    	  try
    	  {
    	    transaction = manager.StartModule(oid);
    	  }
    	  catch (Exception e)
    	  {
    	    Simple.ErrorAlert("Nepodařilo se inicializovat modul", e.Message, this);
    	    return;
    	  }
    	  
    	  try
    	  {
    	    manager.RunModule(transaction, "Default", parameters);
    	    BMessageReturn message = manager.ReceiveMessage(true) as BMessageReturn;
    	    if (message == null) throw new Exception("Module doesn't communicate.");
    	    
    	    XmlDocument hodnota = new XmlDocument();
    	    string hodnotaHtml;
          try
          {
    	      hodnota.LoadXml(message.Result);
       	    hodnotaHtml = Beline.Tools.XslTransform.Transform(hodnota, "beline.html.xslt");
    	    }
    	    catch (XmlException e)
    	    {
    	      hodnotaHtml = "<html> <body><div align=center style=\"color:red;\"><b>Error!</b></div></body> </html>";
    	    }
    	    //Console.WriteLine(hodnotaHtml);
			    
    	    htmlRender.Render(hodnotaHtml);
    	    //htmlWidget.ImagesRef();
    	  }
    	  catch (Exception e)
    	  {
    	    Simple.ErrorAlert("Nepodařilo se spustit modul", e.Message, this);
    	    return;
    	  }
    	  
   	    try
    	  {
    	    manager.StopModule(transaction);
    	  }
    	  catch (Exception e)
    	  {
    	    Simple.ErrorAlert("Nepodařilo se ukončit modul", e.Message, this);
    	    return;
    	  }
      }
    }
    #endregion
  }
} // namespace

