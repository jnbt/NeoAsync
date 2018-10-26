using System;
using System.Collections;

namespace Neo.Async {
  /// <summary>
  /// Represents a controller instance which is able to start a coroutine
  /// based on any IEnumerator and also cancel the coroutine.
  /// </summary>
  public interface ICoroutineStarter {
    /// <summary>
    /// Adds an coroutine via an IEnumerator
    /// </summary>
    /// <param name="task">to be executed</param>
    ICoroutine Add(IEnumerator task);
    /// <summary>
    /// Remove an already started coroutine
    /// </summary>
    /// <param name="coroutine">to be stopped</param>
    /// <exception cref="ArgumentException"></exception>
    void Remove(ICoroutine coroutine);
  }
}
