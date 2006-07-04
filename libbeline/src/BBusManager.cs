using System;


namespace LibBeline {
	/// <summary>
	/// Abstract class over all different implementations of BUS manager
	/// </summary>
	public abstract class BBusManager {
	  /// <summary>
	  /// Return subtype of the manager. Throught this can the child of BBusManager be identified.
	  /// </summary>
	  /// <returns></returns>
	  public abstract BEnumBusManagerType GetManagerType ();
	  /// <summary>
	  /// Physically send a message.
	  /// </summary>
	  /// <param name="message">Message to send.</param>
	  public abstract void Send (BMessage message);
	  /// <summary>
	  /// Physically receive message and return it.
	  /// </summary>
	  /// <param name="blocking">Should I wait for message if nothing wait? This will wait for 32 seconds and then return null to avoid freezing the application.</param>
	  /// <returns>Received message or null.</returns>
	  public abstract BMessage Receive (bool blocking);
	  /// <summary>
	  /// Sync all buffers and close connection to bus.
	  /// </summary>
	  public abstract void Destroy();

	}
}

