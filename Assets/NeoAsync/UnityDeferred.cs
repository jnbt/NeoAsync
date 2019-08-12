using System;
using System.Collections;
using UnityEngine;

namespace Neo.Async {
  /// <summary>
  /// Represents a deferred call using Unity's coroutines
  /// </summary>
  public class UnityDeferred : IDeferred {
    /// <inheritdoc />
    public float Seconds { get; private set; }

    /// <inheritdoc />
    public Action Callback { get; private set; }

    /// <inheritdoc />
    public bool Finished { get; private set; }

    /// <inheritdoc />
    public bool Aborted { get; private set; }

    private ICoroutine runningCoroutine;

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

    /// <inheritdoc />
    public void Start() {
      Finished = false; // reset the state
      if (Seconds > 0f) {
        runningCoroutine = CoroutineStarter.Instance.Add(waitForTime());
      } else {
        Finished = true;
        callbackFailSafe();
      }
    }

    /// <inheritdoc />
    public void Abort() {
      if (!Finished && runningCoroutine != null) {
        CoroutineStarter.Instance.Remove(runningCoroutine);
      }
      Finished = true;
      Aborted = true;
    }

    private IEnumerator waitForTime() {
      yield return new WaitForSeconds(Seconds);
      if (!Finished) {
        Finished = true;
        callbackFailSafe();
      }
    }

    private void callbackFailSafe() {
      if (Callback == null) return;
      try {
        Callback();
        // As timing is often used in combination with UI
        // handle the common exception of already destroyed GameObjects
        // to no block the game.
        // All other exceptions are intentionally not catched.
      } catch (MissingReferenceException ex) {
        Debug.Log(ex.Message);
      }
    }
  }
}
