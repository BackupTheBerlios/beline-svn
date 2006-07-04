// created on 3.4.2006 at 22:37
using System;
using System.IO;
using System.Text;

namespace Beline.Tools 
{
  public class SaveHtmlTool
  {
    /// name of a destination file
    public string FileName;
    
    private StringBuilder content;
    
    public SaveHtmlTool()
    {
      content = new StringBuilder();
    }
    
    /// Append part of saved content to internal structure
    /// <param name="aContent">A part of content to be saved</param> 
    public bool AppendHtml(System.IntPtr aEngine, string aContent)
    {
      content.Append(aContent);
      return true;
    }
    
    /// Clear an internal content to prepare class to new storing event
    public void Clear()
    {
      content.Remove(0, content.Length);
    }
    
    /// Physically save an internal content to a file (stored in the FileName property) 
    public bool Save()
    {
//      if (File.Exists(FileName)) return false;
      StreamWriter writer = File.CreateText(FileName);
      
      try
      {
        writer.Write(content);
      } 
      catch (Exception e)
      {
        System.Console.WriteLine(e.Message);
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
