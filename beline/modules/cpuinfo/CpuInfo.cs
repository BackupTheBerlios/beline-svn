// created on 23.4.2006 at 18:04
// Filename: CpuInfo.cs
/// Contains functions to extract information displayed in CPU category
/// This code based on Nil Gradisnik's Sysinfo project

using System;
using System.IO;
using System.Text;
using LibBeline;

namespace Beline.Modules {
	
	public class CpuInfo : LibBeline.BObservable {
		
		#region private values
		private String cpu_vendor = "unknown";
		private String cpu_name = "unknown";
		private String cpu_frequency = "unknown";
		private String cpu_cache = "unknown";
		private String cpu_bogomips = "unknown";
		private String cpu_numbering = "unknown";
		private String cpu_flags = "unknown";
		#endregion
		
		#region global variables
		public bool HasFpu
		{
		  get { return (cpu_flags.IndexOf("fpu") != -1);}
		}
		public bool HasMmx
		{
		  get { return (cpu_flags.IndexOf("mmx") != -1);}
		}
		public bool HasSse
		{
		  get { return (cpu_flags.IndexOf("sse") != -1);}
		}
		public bool HasSse2
		{
		  get { return (cpu_flags.IndexOf("sse2") != -1);}
		}
		public string CpuVendor
		{
		  get {return cpu_vendor;}
		}
		public string CpuName
		{
		  get {return cpu_name;}
		}
		public string CpuFrequency
		{
		  get {return cpu_frequency;}
		}
		public string CpuCache
		{
		  get {return cpu_cache;}
		}
		public string CpuBogoMips
		{
		  get {return cpu_bogomips;}
		}
		public string CpuNumbering
		{
		  get {return cpu_numbering;}
		}
		public string CpuFlags
		{
		  get {return cpu_flags;}
		}
		#endregion
		
		[STAThread]
		public static int Main(string[] args)
	  {
		  CpuInfo instance = new CpuInfo();
		  return instance.WaitForMessage();
	  }
		
		private CpuInfo()
		{
		  // initialize plugin
  		try
  		{
  		  LibBeline.LibBeline.InitializeInstance(BEnumSystem.slave, "cpuinfo");
  		  BSlaveServiceManager.GetInstance().AttachObserver(this);
  		}
  		catch (Exception e)
  		{
  		  Console.WriteLine("Nepodařilo se zinicializovat modul CpuInfo: {0}", e.Message, this);
  		  return; // quit the module
  		}
  		
		  CpuStaticInfo();
		  CpuDynamicInfo();
		}
		  
		public int WaitForMessage()
		{
      return BSlaveServiceManager.GetInstance().MessageHandler();
		}
		
		/// function needed by BObservable interface
		public void MessageArrived(BMessage aMessage)
		{
		  switch (aMessage.Template)
      {
        case "run.msg":
          // I have no parameters so only return measured values
          BSlaveServiceManager.GetInstance().SendReturn(0, CreateReturnMessage());
          break;
        case "getstatus.msg":
          BSlaveServiceManager.GetInstance().SendStatus(100, "");
          break;
        case "end.msg":
          // should not arrive
          break;
        case "alive.msg":
        case "stop.msg":
        default:
          // nothing to do - ignore
          break;         
      }
		}
		
		//read cpu function
		public void CpuStaticInfo() 
		{	
			String temp;
			Boolean modelB = false;
			Boolean staticB = false;
			
			try 
			{	
				//get cpu information from /proc
				using (TextReader textread = File.OpenText("/proc/cpuinfo")) 
				{
					while ( staticB == false ) {
						
						temp = textread.ReadLine();
						
						//vendor
						if ( temp.StartsWith("vendor_id")) 
						{
							cpu_vendor = temp.Remove(0, 12);
						}
						
						//model name
						if ( temp.StartsWith("model name")) 
						{
							cpu_name =  temp.Remove(0, 13);
						}
										
						//cache
						if ( temp.StartsWith("cache size")) 
						{
							cpu_cache =  temp.Remove(0, 13);
						}
						
						//numbering1 family
						if ( temp.StartsWith("cpu family")) 
						{
							cpu_numbering =  "family(" + temp.Remove(0, 13);
						}
						
						//numbering2 model
						if ( temp.StartsWith("model") && modelB == false) 
						{
							cpu_numbering =  cpu_numbering + ") model(" + temp.Remove(0, 9);
							modelB = true;
						}
						
						//numbering3 stepping
						if ( temp.StartsWith("stepping")) 
						{
							cpu_numbering =  cpu_numbering + ") stepping(" + temp.Remove(0, 11) + ")";
						}
						
						//flags
						if ( temp.StartsWith("flags")) 
						{
							cpu_flags =  temp.Remove(0, 9);
							staticB = true;
						}
					}
				}
			} catch (FileNotFoundException ex) {}
		}
		
		//this function needs to be called more than once, to update info
		public void CpuDynamicInfo() 
		{	
			String temp;
			Boolean dynamicB = false;
			
			try 
			{	
				//get cpu information from /proc
				using (TextReader textread = File.OpenText("/proc/cpuinfo")) 
				{	
					while ( dynamicB == false ) 
					{
						temp = textread.ReadLine();
						
						//frequency
						if ( temp.StartsWith("cpu MHz")) 
						{
							cpu_frequency =  temp.Remove(0, 11);
						}
						
						//bogomips
						if ( temp.StartsWith("bogomips")) 
						{
							cpu_bogomips =  temp.Remove(0, 11);
							dynamicB = true;
						}
					}
				}
			} catch (FileNotFoundException ex) {}
		}
		
		private string CreateReturnMessage()
		{
		  StringBuilder retval = new StringBuilder();
		  string imagesPath = BSlaveServiceManager.GetInstance().ModuleConfiguration["/beline/conf/module/configuration/fold[@id='1']/heading[@id='1']/bcfgitem[@id='1']/string[@value]"].ToString() + "/";

		  retval.Append("<retval label=\"Statické informace\">");
		    retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"VendorId\" description=\"Výrobce\"><string><text lang=\"cz\">" + cpu_vendor + "</text></string></item>");
		    retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Name\" description=\"Název procesoru\"><string><text lang=\"cz\">" + cpu_name + "</text></string></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Cache\" description=\"Procesorová Cache\"><int value=\"" + cpu_cache + "\" /></item>");		 
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Numbering\" description=\"Číslování\"><string><text lang=\"cz\">" + cpu_numbering + "</text></string></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"BogoMIPS\" description=\"BogoMIPS\"><int value=\"" + cpu_bogomips + "\" /></item>");
      retval.Append("</retval>");
      retval.Append("<retval label=\"Dynamické informace\">");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Frekvence\" description=\"Frekvence\" units=\"Hz\" SImultip=\"M\"><int value=\"" + cpu_frequency + "\" /></item>");
      retval.Append("</retval>");
      retval.Append("<retval label=\"Flagy\">");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"FPUPresent\" description=\"Přítomná FPU\"><bool value=\"" + HasFpu.ToString() + "\"></bool></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"MMXPresent\" description=\"Podpora MMX instrukcí\"><bool value=\"" + HasMmx.ToString() + "\"></bool></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"SSEPresent\" description=\"Podpora SSE instrukcí\"><bool value=\"" + HasSse.ToString() + "\"></bool></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"SSE2Present\" description=\"Podpora SSE2 instrukcí\"><bool value=\"" + HasSse2.ToString() + "\"></bool></item>");
      retval.Append("</retval>");
      
      return retval.ToString();
		}
	}
}

//ghaefb
