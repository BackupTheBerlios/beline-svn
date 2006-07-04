// Filename: MemoryInfo.cs
// Contains functions to extract information displayed in Memory category
/// This code based on Nil Gradisnik's Sysinfo project

using System;
using System.IO;
using System.Text;
using LibBeline;

namespace Beline.Modules {
	
	public class MemoryInfo : BObservable {
		
		#region private values
		public String memory_total = "unknown";
		public String memory_free = "unknown";
		
		public String memory_swaptotal = "unknown";
		public String memory_swapfree = "unknown";
		
		public String memory_cached = "unknown";
		public String memory_active = "unknown";
		public String memory_inactive = "unknown";
	  public String memory_buffers = "unknown";
		#endregion
		
		[STAThread]
		public static int Main(string[] args)
	  {
		  MemoryInfo instance = new MemoryInfo();
		  return instance.WaitForMessage();
	  }
	  
	  private MemoryInfo()
		{
		  // initialize plugin
  		try
  		{
  		  LibBeline.LibBeline.InitializeInstance(BEnumSystem.slave, "memoryinfo");
  		  BSlaveServiceManager.GetInstance().AttachObserver(this);
  		}
  		catch (Exception e)
  		{
  		  Console.WriteLine("Nepodařilo se zinicializovat modul MemoryInfo: {0}", e.Message, this);
  		  return; // quit the module
  		}
  		
		  MemoryStaticInfo();
		  MemoryDynamicInfo();
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
		
		//read memory info
		private void MemoryStaticInfo() {
			String temp;

			try {
				
				//get memory information from /proc
				using (TextReader textread = File.OpenText("/proc/meminfo")) {

					while ( (temp = textread.ReadLine()) != null ) {
						//total
						if ( temp.StartsWith("MemTotal:")) {
							memory_total = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 9).Trim();
						}
						
						//swap total
						if ( temp.StartsWith("SwapTotal:")) {
							memory_swaptotal = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 10).Trim();
							
							if ( memory_swaptotal == "0" )
								memory_swaptotal = "no swap";
//							else
//								memory_swaptotal = ( Int32.Parse(temp) / 1000 ).ToString();
						}
						
					}
				}
			}catch (FileNotFoundException ex) {}
		}
		
		//read memory info
		private void MemoryDynamicInfo() {
			try {
				string temp;
				//get memory information from /proc
				using (TextReader textread = File.OpenText("/proc/meminfo")) {

					while ((temp = textread.ReadLine()) != null ) {
						//free
						if ( temp.StartsWith("MemFree:")) {
							memory_free = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 8).Trim();
						}
						
						//swap free
						if ( temp.StartsWith("SwapFree:")) {
							memory_swapfree = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 9).Trim();
						}
						
						//cached memory
						if ( temp.StartsWith("Cached:")) {
							memory_cached = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 7).Trim();
						}
						
						//active memory
						if ( temp.StartsWith("Active:")) {	
							memory_active = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 7).Trim();
						}
						
						//inactive memory
						if ( temp.StartsWith("Inactive:")) {
							memory_inactive = temp.Remove(temp.IndexOf("kB"), 2).Remove(0, 9).Trim();
						}
						
						// amount of buffers in memory
						if ( temp.StartsWith("Buffers:")) {
						  memory_buffers = temp.Remove(temp.IndexOf("kB"), 2).Remove(0,8).Trim();
						}
					}
				}
			}catch (FileNotFoundException ex) {}
		}
		
		private string CreateReturnMessage()
		{
		  StringBuilder retval = new StringBuilder();
		  string imagesPath = BSlaveServiceManager.GetInstance().ModuleConfiguration["/beline/conf/module/configuration/fold[@id='1']/heading[@id='1']/bcfgitem[@id='1']/string[@value]"].ToString() + "/";

		  retval.Append("<retval label=\"Uživatelská paměť\">");
		    retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Total\" description=\"Celkem paměti\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_total + "\" /></item>");
		    retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Free\" description=\"Volná paměť\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_free + "\" /></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Buffer\" description=\"Z toho vyrovnávací paměť\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_buffers + "\" /></item>");		 
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Cache\" description=\"Z toho disková cache\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_cached + "\" /></item>");
      retval.Append("</retval>");
      retval.Append("<retval label=\"Odkládací prostor\">");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Total\" description=\"Celkem\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_swaptotal + "\" /></item>");
        retval.Append("<item icon='" + imagesPath + "cpu.png' name=\"Free\" description=\"Volná\" units=\"B\" SImultip=\"k\"><int value=\"" + memory_swapfree + "\" /></item>");
      retval.Append("</retval>");
      
      Console.WriteLine(retval.ToString());
      return retval.ToString();
		}
	}
}

//ghaefb
