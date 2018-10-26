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

    /// <summary>
    /// Creates a debounced function that delays invoking <c>func</c> until after <c>wait</c> seconds
    /// have elapsed since the last time the debounced function was invoked. The debounced function
    /// comes with a <see cref="IDebounce.Abort"/> method to cancel delayed `func` invocations
    /// and a <see cref="IDebounce.Flush"/> method to immediately invoke them.
    ///
    /// Provide options to indicate whether <c>func</c> should be invoked on the leading and/or
    /// trailing edge of the <c>wait</c> timeout.
    ///
    /// The <c>func</c> is invoked with the last arguments provided to the debounced function.
    /// </summary>
    /// <remarks>
    /// If <c>leading</c> and <c>trailing</c> options are true, <c>func</c> is invoked on the
    /// trailing edge of the timeout only if the debounced function is invoked more than once
    /// during the <c>wait</c> timeout.
    ///
    /// See David Corbachos article (https://css-tricks.com/debouncing-throttling-explained-examples/)
    /// for details over the differences between <see cref="Throttle{T}"/> and
    /// <see cref="Debounce{T}"/>.
    /// </remarks>
    /// <param name="func">The function to debounce</param>
    /// <param name="wait">The number of seconds to delay</param>
    /// <param name="maxWait">The maximum seconds <c>func</c> is allowed to be delayed before it's invoked</param>
    /// <param name="leading">Specify invoking on the leading edge of the timeout</param>
    /// <param name="trailing">Specify invoking on the trailing edge of the timeout</param>
    /// <typeparam name="T">Input type of <c>func</c></typeparam>
    /// <returns>The debounced <c>func</c> wrapper</returns>
    IDebounce<T> Debounce<T>(Action<T> func, float wait, float? maxWait = null, bool leading = false,
      bool trailing = true);


    /// <summary>
    /// Same as <see cref="Debounce{T}"/> with multiple arguments
    /// </summary>
    IDebounce<T, V> Debounce<T, V>(Action<T, V> func, float wait,
      float? maxWait = null, bool leading = false, bool trailing = true);

    /// <summary>
    /// Creates a throttled function that only invokes <c>func</c> at most once per every <c>wait</c>
    /// seconds. The throttled function comes with a <see cref="IDebounce.Abort"/> method to cancel
    /// delayed <c>func</c> invocations and a <see cref="IDebounce.Flush"/> method to immediately
    /// invoke them.
    ///
    /// Provide options to indicate whether <c>func</c> should be invoked on the leading and/or
    /// trailing edge of the <c>wait</c> timeout.
    ///
    /// The <c>func</c> is invoked with the last arguments provided to the throttled function.
    /// Subsequent calls to the throttled function return the result of the last <c>func</c> invocation.
    /// </summary>
    /// <remarks>
    /// If <c>leading</c> and <c>trailing</c> options are true (default), <c>func</c> is invoked on the
    /// trailing edge of the timeout only if the throttled function is invoked more than once
    /// during the <c>wait</c> timeout.
    ///
    /// See David Corbachos article (https://css-tricks.com/debouncing-throttling-explained-examples/)
    /// for details over the differences between <see cref="Throttle{T}"/> and
    /// <see cref="Debounce{T}"/>.
    /// </remarks>
    /// <param name="func">The function to throttle</param>
    /// <param name="wait">The number of seconds to throttle invocations to</param>
    /// <param name="leading">Specify invoking on the leading edge of the timeout</param>
    /// <param name="trailing">Specify invoking on the trailing edge of the timeout</param>
    /// <typeparam name="T">Input type of <c>func</c></typeparam>
    /// <returns>The throttled <c>func</c> wrapper</returns>
    IDebounce<T> Throttle<T>(Action<T> func, float wait, bool leading = true, bool trailing = true);

    /// <summary>
    /// Same as <see cref="Throttle{T}"/> with multiple arguments
    /// </summary>
    IDebounce<T, V> Throttle<T, V>(Action<T, V> func, float wait, bool leading = true, bool trailing = true);
  }
}
