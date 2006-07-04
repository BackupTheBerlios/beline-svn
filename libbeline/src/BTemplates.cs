// created on 2.5.2006 at 21:19
using System;
using System.IO;
using System.Collections;

namespace LibBeline
{
  // class read, cache and return templates of messages
  public sealed class BTemplates
  {
    /// it can cach maximum of 20 items
    Hashtable cache;
    internal static BTemplates singletonInstance;
    
    private BTemplates()
    {
      cache = new Hashtable(20);
    }
    
    public static BTemplates GetInstance()
    {
      // if this is first run (singleton not inicialized yet)
      if (singletonInstance == null)
        singletonInstance = new BTemplates();
      
      return singletonInstance;
    }
    
    /// Read template file with given name and return its content as string 
    public string ReadTemplate(string aFileName)
    {
      // first try to read from template
      if (cache[aFileName] != null)
        return cache[aFileName].ToString();
        
      // combine complete path found in a global configuration
      string pathToTemplates = BConfigManager.GetInstance().GlobalConf["/beline/conf/global/paths[@templates]"].ToString();
      pathToTemplates = Path.Combine(pathToTemplates, aFileName);
            
      // read the whole file
      StreamReader reader;
      string retval;
      try
      {
        reader = new StreamReader(pathToTemplates);
      }
      catch (DirectoryNotFoundException)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (FileNotFoundException)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (IOException)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (ArgumentNullException)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      catch (ArgumentException)
      {
        throw new FileNotFoundException("File " + pathToTemplates + " not found.");
      }
      
      try
      {
        retval = reader.ReadToEnd();
      }
      catch (IOException)
      {
        reader.Close();
        throw;
      }
      catch (OutOfMemoryException)
      {
        reader.Close();
        throw;
      }
      
      reader.Close();
      
      // cache template
      cache.Add(aFileName, retval);
      return retval;
    }
  }
}
