using System;
using System.Collections;
using UnityEngine;

namespace Neo.Async{
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
  public class CoroutineStarter : MonoBehaviour{
    /// <summary>
    /// Returns the single GameObject-based instance
    /// </summary>
    static public CoroutineStarter Instance{
      get {
        GameObject go = GameObject.Find("_CoroutineStarter");
        if(go == null){
          go = new GameObject("_CoroutineStarter");
          go.AddComponent<CoroutineStarter>();
          GameObject.DontDestroyOnLoad(go);
        }
        return go.GetComponent<CoroutineStarter>();
      }
    }

    /// <summary>
    /// Adds an Unity-base coroutine via an IEnumerator
    /// </summary>
    /// <param name="task"></param>
    public void Add(IEnumerator task){
      StartCoroutine(task);
    }
  }
}
