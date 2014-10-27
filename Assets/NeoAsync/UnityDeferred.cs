using System;
using System.Collections;
using UnityEngine;

namespace Neo.Async {
  /// <summary>
  /// Represents a deferred call using Unity's coroutines
  /// </summary>
  public class UnityDeferred : IDeferred {
    /// <summary>
    /// Seconds to wait as a timeout
    /// </summary>
    public float Seconds { get; private set; }
    /// <summary>
    /// Callback to called in a deferred way
    /// </summary>
    public Action Callback { get; private set; }
    /// <summary>
    /// Is the deferred call already executed?
    /// </summary>
    public bool Finished { get; private set; }
    /// <summary>
    /// Was the deferred call aborted?
    /// </summary>
    public bool Aborted { get; private set; }

    /// <summary>
    /// Creates in instance which describes a deferred call
    /// </summary>
    /// <param name="s">to use as timeout or interval</param>
    /// <param name="cb">to be called</param>
    public UnityDeferred(float s, Action cb) {
      Seconds = s;
      Callback = cb;
      Finished = false;
      Aborted = false;
    }

    /// <summary>
    /// Start the deferred call
    /// </summary>
    public void Start() {
      Finished = false; // reset the state
      if(Seconds > 0f) {
        CoroutineStarter.Instance.Add(waitForTime());
      } else {
        Finished = true;
        callbackFailSafe();
      }
    }

    /// <summary>
    /// Abort the deferred call if not already executed
    /// </summary>
    public void Abort() {
      Finished = true;
      Aborted = true;
    }

    private IEnumerator waitForTime() {
      yield return new WaitForSeconds(Seconds);
      if(!Finished) {
        Finished = true;
        callbackFailSafe();
      }
    }

    private void callbackFailSafe() {
      if(Callback == null) return;
      try {
        Callback();
        // As timing is often used in combination with UI
        // handle the common exception of already destroyed GameObjects
        // to no block the game.
        // All other exceptions are intentionally not catched.
      } catch(MissingReferenceException ex) {
        UnityEngine.Debug.LogWarning(ex.Message);
      }
    }
  }
}
