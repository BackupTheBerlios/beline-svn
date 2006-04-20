// created on 3.4.2006 at 22:37
using System;
using System.IO;

namespace Beline.Tools 
{
  public class FileTools
  {
    public static bool SaveHtml(System.IntPtr aEngine, string aContent)
    {
      if (File.Exists("/home/kowy/Projects/beline/pokus.html")) return false;
      StreamWriter writer = File.CreateText("/home/kowy/Projects/beline/pokus.html");
      
      try
      {
        writer.Write(aContent);
      } 
      catch (Exception e)
      {
        return false;
      }
      finally
      {
        writer.Close();
      }
      
      return true;
    }
  }
} // namesapce