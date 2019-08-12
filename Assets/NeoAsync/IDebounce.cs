namespace Neo.Async {
  /// <summary>
  /// Represents a debounced function 
  /// </summary>
  public interface IDebounce {
    /// <summary>
    /// Cancel the delayed <c>func</c> invocations
    /// </summary>
    void Abort();
    /// <summary>
    /// Immediatly trigger the delayed <c>func</c> invocations
    /// </summary>
    void Flush();
  }

  /// <summary>
  /// Represents a debounced function 
  /// </summary>
  public interface IDebounce<T> : IDebounce {
    /// <summary>
    /// Triggers a debounced <c>func</c> invocation
    /// </summary>
    /// <param name="arg">to pass to <c>func</c></param>
    void Call(T arg);
  }

  /// <summary>
  /// Represents a debounced function 
  /// </summary>
  public interface IDebounce<T, V> : IDebounce {
    /// <summary>
    /// Triggers a debounced <c>func</c> invocation
    /// </summary>
    /// <param name="arg0">to pass to <c>func</c></param>
    /// <param name="arg1">to pass to <c>func</c></param>
    void Call(T arg0, V arg1);
  }
}
