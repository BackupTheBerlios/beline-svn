using System;


namespace LibBeline {
	/// Abstraktní třída zastřešující různé manažery sběrnice
	public abstract class BBusManager {
	  // 
	  public abstract BEnumBusManagerType GetManagerType ();
	  // 
	  public abstract void Send (BMessage aMessage);
	  // 
	  public abstract BMessage Receive ();

	}
}

