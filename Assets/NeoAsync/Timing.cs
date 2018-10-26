using System;

namespace Neo.Async {
  /// <inheritdoc />
  public sealed class Timing : ITiming {
    /// <inheritdoc />
    public IDeferred After(int seconds, Action callback) {
      return After((float) seconds, callback);
    }

    /// <inheritdoc />
    public IDeferred After(float seconds, Action callback) {
      IDeferred d = new UnityDeferred(seconds, callback);
      d.Start();
      return d;
    }

    /// <inheritdoc />
    public IDeferred Every(int seconds, Action callback) {
      return Every((float) seconds, callback);
    }

    /// <inheritdoc />
    public IDeferred Every(float seconds, Action callback) {
      IDeferred deferred = null;
      deferred = new UnityDeferred(seconds, () => {
        callback();
        if (deferred != null && !deferred.Aborted) deferred.Start();
      });
      deferred.Start();
      return deferred;
    }
  }
}
