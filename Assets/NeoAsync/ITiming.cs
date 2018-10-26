using System;

namespace Neo.Async {
  /// <summary>
  /// Allows easy time-driven callbacks:
  ///  * After(seconds, callback)
  ///    Invokes a callback after the timeout in seconds.
  ///  * Every(seconds, callback)
  ///    Invokes a callback every x seconds. First time in x seconds.
  /// </summary>
  ///
  /// <example>
  ///   [Inject]
  ///   public ITiming Timing { get; set; }
  ///   ...
  ///   Timing.After(5, () => UnityEngine.Debug.Log("This will be invoked in 5 seconds"));
  ///   Timing.Every(5, () => UnityEnging.Debug.Log("This will be invoked EVERY 5 seconds"));
  /// </example>
  ///
  /// <remarks>
  ///   All calls return a "Deferred" object which allows calling an Abort method
  ///   will which stop the time-driven callback.
  /// </remarks>
  public interface ITiming {
    /// <summary>
    /// Builds and starts a deferral for x seconds. Than the callback is called.
    /// </summary>
    /// <param name="s">to wait</param>
    /// <param name="cb">to call</param>
    /// <returns>the deferral</returns>
    IDeferred After(int s, Action cb);

    /// <summary>
    /// Builds and starts a deferral for x seconds. Than the callback is called.
    /// </summary>
    /// <param name="s">to wait</param>
    /// <param name="cb">to call</param>
    /// <returns>the deferral</returns>
    IDeferred After(float s, Action cb);

    /// <summary>
    /// Builds and starts a deferral for x seconds. The callback is called every x seconds.
    /// </summary>
    /// <param name="s">to use as interval</param>
    /// <param name="cb">to call</param>
    /// <returns>the deferral</returns>
    IDeferred Every(int s, Action cb);

    /// <summary>
    /// Builds and starts a deferral for x seconds. The callback is called every x seconds.
    /// </summary>
    /// <param name="s">to use as interval</param>
    /// <param name="cb">to call</param>
    /// <returns>the deferral</returns>
    IDeferred Every(float s, Action cb);
  }
}
