using System;
using System.Collections;
using UnityEngine;

namespace Neo.Async{
  /// <summary>
  /// Represents a deferred call using Unity's coroutines
  /// </summary>
  public class UnityDeferred : IDeferred{
    public float  Seconds{get; private set;}
    public Action Callback{get; private set;}
    public bool   Finished{get; private set;}
    public bool   Aborted{get; private set;}

    public UnityDeferred(float s, Action cb){
      Seconds  = s;
      Callback = cb;
      Finished = false;
      Aborted  = false;
    }

    public void Start(){
      Finished = false; // reset the state
      if(Seconds > 0f){
        CoroutineStarter.Instance.Add(waitForTime());
      }else{
        Finished = true;
        callbackFailSafe();
      }
    }

    public void Abort(){
      Finished = true;
      Aborted  = true;
    }

    private IEnumerator waitForTime(){
      yield return new WaitForSeconds(Seconds);
      if(!Finished){
        Finished = true;
        callbackFailSafe();
      }
    }

    private void callbackFailSafe(){
      if(Callback == null) return;
      try{
        Callback();
      // As timing is often used in combination with UI
      // handle the common exception of already destroyed GameObjects
      // to no block the game.
      // All other exceptions are intentionally not catched.
      }catch(MissingReferenceException ex){
        UnityEngine.Debug.LogWarning(ex.Message);
      }
    }
  }
}