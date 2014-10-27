using System;

namespace Neo.Async{
  /// <summary>
  /// Represents a deferred invocation of a callback
  /// </summary>
  public interface IDeferred{
    /// <summary>
    /// Starts the coroutine. When completed the callback will be called
    /// </summary>
    void   Start();
    /// <summary>
    /// Starts the coroutine. When completed the callback will be called
    /// </summary>
    void   Abort();
    /// <summary>
    /// True if the invocation already happend since the last Start
    /// </summary>
    bool   Finished{get;}
    /// <summary>
    /// True if the invocation will not happend since the last start
    /// </summary>
    bool   Aborted{get;}
    /// <summary>
    /// Seconds to defer
    /// </summary>
    float  Seconds{get;}
    /// <summary>
    /// Callback to be called on invocation
    /// </summary>
    Action Callback{get;}
  }
}