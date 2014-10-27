using System;

namespace Neo.Async {
  /// <summary>
  /// Allows easy interactions with deferred (or repeated) calls
  /// </summary>
  public class Timing : ITiming {
    /// <summary>
    /// Calls the callback after the given seconds
    /// </summary>
    /// <param name="seconds">to wait</param>
    /// <param name="callback">to be called</param>
    /// <returns>A deferred call instance</returns>
    public IDeferred After(int seconds, Action callback) {
      return After((float) seconds, callback);
    }

    /// <summary>
    /// Calls the callback after the given seconds
    /// </summary>
    /// <param name="seconds">to wait</param>
    /// <param name="callback">to be called</param>
    /// <returns>A deferred call instance</returns>
    public IDeferred After(float seconds, Action callback) {
      IDeferred d = new UnityDeferred(seconds, callback);
      d.Start();
      return d;
    }

    /// <summary>
    /// Calls the callback every x seconds. First time in x seconds
    /// </summary>
    /// <param name="seconds">to use as an interval</param>
    /// <param name="callback">to be called</param>
    /// <returns>A deferred call instance</returns>
    public IDeferred Every(int seconds, Action callback) {
      return Every((float) seconds, callback);
    }

    /// <summary>
    /// Calls the callback every x seconds. First time in x seconds
    /// </summary>
    /// <param name="seconds">to use as an interval</param>
    /// <param name="callback">to be called</param>
    /// <returns>A deferred call instance</returns>
    public IDeferred Every(float seconds, Action callback) {
      IDeferred deferred = null;
      deferred = new UnityDeferred(seconds, () => {
        callback();
        if(deferred != null && !deferred.Aborted) deferred.Start();
      });
      deferred.Start();
      return deferred;
    }
  }
}
