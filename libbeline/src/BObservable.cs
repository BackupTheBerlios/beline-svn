using System;

namespace LibBeline {
  /// Interface pro všechny observované třídy
  public interface BObservable {

    // 
    void MessageArrived (BMessage aCommand);

  } // class BObservable
} // namespace

