using System;
using System.Collections;
using UnityEngine;

namespace Neo.Async {
  /// <summary>
  /// Allows starting a Unity-based coroutine from any instance.
  /// This is needed as only objects which inherit from MonoBehavior can
  /// start a coroutine, but you might not use this inheritance.
  /// </summary>
  /// <example>
  /// <![CDATA[
  ///   class SomeClass {
  ///     public void Do(){
  ///       CoroutineStarter.Instance.Add(doLazy());
  ///     }
  ///     private IEnumerator doLazy(){
  ///       yield return UnityEngine.WaitForSeconds(5f);
  ///       UnityEngine.Debug.Log("This should be invoked as a coroutine");
  ///     }
  ///   }
  /// ]]>
  /// </example>
  public sealed class CoroutineStarter : MonoBehaviour, ICoroutineStarter {
    /// <summary>
    /// Returns the single GameObject-based instance
    /// </summary>
    public static CoroutineStarter Instance {
      get {
        GameObject go = GameObject.Find("_CoroutineStarter");
        if (go == null) {
          go = new GameObject("_CoroutineStarter");
          go.AddComponent<CoroutineStarter>();
          DontDestroyOnLoad(go);
        }
        return go.GetComponent<CoroutineStarter>();
      }
    }

    /// <inheritdoc />
    public ICoroutine Add(IEnumerator task) {
      return new UnityCoroutine(StartCoroutine(task));
    }

    /// <inheritdoc />
    public void Remove(ICoroutine coroutine) {
      UnityCoroutine asUnity = coroutine as UnityCoroutine;
      if (asUnity == null) throw new ArgumentException("coroutine must be a UnityCoroutine");
      StopCoroutine(asUnity.OriginalCoroutine);
    }

    private sealed class UnityCoroutine : ICoroutine {
      public Coroutine OriginalCoroutine { get; private set; }

      public UnityCoroutine(Coroutine originalCoroutine) {
        OriginalCoroutine = originalCoroutine;
      }
    }
  }
}
