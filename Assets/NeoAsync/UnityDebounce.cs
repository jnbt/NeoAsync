using System;
using UnityEngine;

namespace Neo.Async {
  /// <summary>
  /// Base class to handle debouncing / throttling logic
  /// </summary>
  public abstract class UnityDebounceBase {
    private readonly ITiming timing;
    private readonly float wait;
    private readonly bool leading;
    private readonly bool trailing;
    private readonly float? maxWait;
    private IDeferred deferred;
    private float? lastCallTime;
    private float? lastInvokeTime;
    private bool debounced;

    /// <summary>
    /// Initializes a debounced function call
    /// </summary>
    /// <param name="timing">Timing system to use</param>
    /// <param name="wait">Time to wait</param>
    /// <param name="leading">If set to <c>true</c> invoke on leading flank.</param>
    /// <param name="trailing">If set to <c>true</c> invoke on trailing flank.</param>
    /// <param name="maxWait">Maximum time to wait</param>
    protected UnityDebounceBase(ITiming timing, float wait, bool leading, bool trailing, float? maxWait) {
      this.timing = timing;
      this.wait = Mathf.Max(wait, 0);
      this.leading = leading;
      this.trailing = trailing;
      if (maxWait.HasValue) {
        this.maxWait = Mathf.Max(maxWait.Value, wait);
      }
    }

    /// <inheritdoc />
    public void Abort() {
      if (deferred != null) {
        deferred.Abort();
      }
      lastInvokeTime = 0;
      lastCallTime = null;
      deferred = null;
      DoResetArgs();
    }

    /// <inheritdoc />
    public void Flush() {
      if (deferred == null) {
        return;
      }
      trailingEdge(now());
    }

    /// <summary>
    /// Debounceds the call
    /// </summary>
    protected void DebouncedCall() {
      float time = now();
      bool isInvoking = shouldInvoke(time);
      debounced = true;
      lastCallTime = time;

      if (isInvoking) {
        if (deferred == null) {
          leadingEdge(time);
          return;
        }
        if (maxWait.HasValue) {
          // Handle invocations in a tight loop
          deferred = timing.After(maxWait.Value, timerExpired);
          invokeFunc(time);
          return;
        }
      }
      if (deferred == null) {
        deferred = timing.After(wait, timerExpired);
      }
    }

    /// <summary>
    /// Hook to reset the remembered arguments
    /// </summary>
    protected abstract void DoResetArgs();
    /// <summary>
    /// Hook to actually invoke the debounced function
    /// </summary>
    protected abstract void DoInvokeFunc();

    private static float now() {
      return Time.unscaledTime;
    }

    private void leadingEdge(float time) {
      // Reset any `maxWait` timer
      lastInvokeTime = time;
      // Start the timer for the trailing edge
      deferred = timing.After(wait, timerExpired);
      // Invoke the leading edge
      if (leading) {
        invokeFunc(wait);
      }
    }

    private float remainingWait(float time) {
      float timeSinceLastCall = time - (lastCallTime ?? 0);
      float timeSinceLastInvoke = time - (lastInvokeTime ?? 0);
      float remaining = wait - timeSinceLastCall;
      return maxWait.HasValue ? Mathf.Min(remaining, maxWait.Value - timeSinceLastInvoke) : remaining;
    }

    private bool shouldInvoke(float time) {
      float timeSinceLastCall = time - (lastCallTime ?? 0);
      float timeSinceLastInvoke = time - (lastInvokeTime ?? 0);
      // Either this is the first call, activity has stopped and we're at the
      // trailing edge, the system time has gone backwards and we're treating
      // it as the trailing edge, or we've hit the `maxWait` limit.
      return !lastCallTime.HasValue || timeSinceLastCall >= wait || timeSinceLastCall < 0 ||
             (maxWait.HasValue && timeSinceLastInvoke >= maxWait.Value);
    }

    private void timerExpired() {
      float time = now();
      if (shouldInvoke(time)) {
        trailingEdge(time);
        return;
      }
      // Restart the timer
      deferred = timing.After(remainingWait(time), timerExpired);
    }

    private void trailingEdge(float time) {
      deferred = null;
      // Only invoke if we have `lastArgs` which means `func` has been
      // debounced at least once.
      if (trailing && debounced) {
        invokeFunc(time);
        return;
      }
      DoResetArgs();
      debounced = false;
    }

    private void invokeFunc(float time) {
      lastInvokeTime = time;
      debounced = false;
      DoInvokeFunc();
    }
  }

  /// Debounced function with one argument
  /// <see cref="UnityDebounceBase"/>
  public sealed class UnityDebounce<T> : UnityDebounceBase, IDebounce<T> {
    private readonly Action<T> func;
    private T lastArg;

    /// <summary>
    /// Initializes a debounced function call
    /// </summary>
    /// <param name="timing">Timing system to use</param>
    /// <param name="func">to invoke debounced</param>
    /// <param name="wait">Time to wait</param>
    /// <param name="leading">If set to <c>true</c> invoke on leading flank.</param>
    /// <param name="trailing">If set to <c>true</c> invoke on trailing flank.</param>
    /// <param name="maxWait">Maximum time to wait</param>
    public UnityDebounce(ITiming timing, Action<T> func, float wait, bool leading, bool trailing, float? maxWait)
      : base(timing, wait, leading, trailing, maxWait) {
      this.func = func;
    }

    /// <inheritdoc />
    public void Call(T arg) {
      lastArg = arg;
      DebouncedCall();
    }

    /// <summary>
    /// Resets the argument
    /// </summary>
    protected override void DoResetArgs() {
      lastArg = default(T);
    }

    /// <summary>
    /// Calls the debounced function with the last argument
    /// </summary>
    protected override void DoInvokeFunc() {
      T arg = lastArg;
      DoResetArgs();
      func(arg);
    }
  }

  /// Debounced function with two arguments
  /// <see cref="UnityDebounceBase"/>
  public sealed class UnityDebounce<T, V> : UnityDebounceBase, IDebounce<T, V> {
    private readonly Action<T, V> func;
    private T lastArg0;
    private V lastArg1;

    /// <summary>
    /// Initializes a debounced function call
    /// </summary>
    /// <param name="timing">Timing system to use</param>
    /// <param name="func">to invoke debounced</param>
    /// <param name="wait">Time to wait</param>
    /// <param name="leading">If set to <c>true</c> invoke on leading flank.</param>
    /// <param name="trailing">If set to <c>true</c> invoke on trailing flank.</param>
    /// <param name="maxWait">Maximum time to wait</param>
    public UnityDebounce(ITiming timing, Action<T, V> func, float wait, bool leading, bool trailing, float? maxWait)
      : base(timing, wait, leading, trailing, maxWait) {
      this.func = func;
    }

    /// <inheritdoc />
    public void Call(T arg0, V arg1) {
      lastArg0 = arg0;
      lastArg1 = arg1;
      DebouncedCall();
    }

    /// <summary>
    /// Resets the arguments
    /// </summary>
    protected override void DoResetArgs() {
      lastArg0 = default(T);
      lastArg1 = default(V);
    }

    /// <summary>
    /// Calls the debounced function with the last arguments
    /// </summary>
    protected override void DoInvokeFunc() {
      T arg0 = lastArg0;
      V arg1 = lastArg1;
      DoResetArgs();
      func(arg0, arg1);
    }
  }
}
