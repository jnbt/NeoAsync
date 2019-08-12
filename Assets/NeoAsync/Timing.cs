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

    /// <inheritdoc />
    public IDebounce<T> Debounce<T>(Action<T> func, float wait,
      float? maxWait = null, bool leading = false, bool trailing = true
    ) {
      return new UnityDebounce<T>(this, func, wait, leading, trailing, maxWait);
    }

    /// <inheritdoc />
    public IDebounce<T, V> Debounce<T, V>(Action<T, V> func, float wait,
      float? maxWait = null, bool leading = false, bool trailing = true
    ) {
      return new UnityDebounce<T, V>(this, func, wait, leading, trailing, maxWait);
    }

    /// <inheritdoc />
    public IDebounce<T> Throttle<T>(Action<T> func, float wait,
      bool leading = true, bool trailing = true
    ) {
      return Debounce(func, wait, wait, leading, trailing);
    }

    /// <inheritdoc />
    public IDebounce<T, V> Throttle<T, V>(Action<T, V> func, float wait,
      bool leading = true, bool trailing = true
    ) {
      return Debounce(func, wait, wait, leading, trailing);
    }
  }
}
