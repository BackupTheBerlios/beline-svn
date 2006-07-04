using System;

namespace LibBeline {
  /// <summary>Communication class offering communication over Corba.</summary>
  public class BCorbaManager : BBusManager {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aName"></param>
    public BCorbaManager (string aName)
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// Overrided.Return subtype of the manager. Throught this can the child of BBusManager be identified.
    /// </summary>
    /// <returns></returns>
    public override BEnumBusManagerType GetManagerType ()
    {
      throw new System.Exception ("Not implemented yet!");
    }
    /// <summary>
    /// Overrided.Physically send a message.
    /// </summary>
    /// <param name="message">Message to send.</param>
    public override void Send(BMessage message)
    {
      throw new System.Exception("Not implemented yet!");
    }
    /// <summary>
    /// Overrided.Physically receive message and return it.
    /// </summary>
    /// <param name="blocking">Should I wait for message if nothing wait? This will wait for 32 seconds and then return null to avoid freezing the application.</param>
    /// <returns>Received message or null.</returns>
    public override BMessage Receive(bool blocking)
    {
      throw new System.Exception("Not implemented yet!");
    }
    /// <summary>
    /// Overrided.Sync all buffers and close connection to bus.
    /// </summary>
    public override void Destroy()
    {
    }

  } // class BCorbaManager
} // namespace

