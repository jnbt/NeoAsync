using System;
using System.Collections.Generic;

namespace Neo.Async {
  /// <summary>
  /// Provides a generic cache for any kind of objects which loading
  /// might need some time and can be referenced by a string key
  /// Idea: Provide a function to load one object via key. This
  /// loader will only be called once per key (until the cache is cleared)
  ///
  /// This implies that you have to fetch objects from the cache via a callback instead
  /// of a simple getter, as the loading might need some time and might be done in the background
  /// </summary>
  /// <example>
  /// <![CDATA[
  ///   class SomeClass{
  ///     private readonly Cache<string, UnityEngine.GameObject> cache;
  ///
  ///     public SomeClass(){
  ///       cache = new Cache<string, UnityEngine.GameObject>(resolveGameObject);
  ///     }
  ///
  ///     public void Do(){
  ///       cache.Get("MyBigGameObject", (go) => go.transform.position = UnityEngine.Vector3.zero);
  ///     }
  ///
  ///     private GameObject resolveGameObject(string key, Action<UnityEngine.GameObject done){
  ///       done(UnityEngine.Resources.Load<GameObject>(key));
  ///     }
  ///   }
  /// ]]>
  /// </example>
  /// <typeparam name="TKey">Type of key to handle cache</typeparam>
  /// <typeparam name="TValue">Type of item value</typeparam>
  public sealed class Cache<TKey, TValue> {
    /// <summary>
    /// Function to be called to load single items into the cache
    /// </summary>
    /// <param name="key">which is looked up</param>
    /// <param name="loader">to call when loaded</param>
    public delegate void LoaderFunction(TKey key, Action<TValue> loader);
    /// <summary>
    /// Function to be called when accessing items on the cache
    /// </summary>
    /// <param name="item">which is loaded from cache or lazy</param>
    public delegate void CallbackFunction(TValue item);

    private readonly LoaderFunction loader;
    private readonly Dictionary<TKey, TValue> storage = new Dictionary<TKey, TValue>();
    private readonly Dictionary<TKey, List<CallbackFunction>> queued = new Dictionary<TKey, List<CallbackFunction>>(); 

    /// <summary>
    /// Initializes a new cache which is bound to a specific loader function
    /// </summary>
    /// <param name="loader">to be called to load items</param>
    public Cache(LoaderFunction loader) {
      this.loader = loader;
    }

    /// <summary>
    /// Receives an item from the cache or loads it
    /// </summary>
    /// <param name="key">to lookup</param>
    /// <param name="callback">to be calles when loaded</param>
    public void Get(TKey key, CallbackFunction callback) {
      if(storage.ContainsKey(key)) callback(storage[key]);
      else enqueue(key, callback);
    }

    /// <summary>
    /// Iterates over all loaded cached items
    /// </summary>
    /// <param name="callback">called for every item</param>
    public void ForEach(CallbackFunction callback) {
      Dictionary<TKey, TValue>.Enumerator enumerator = storage.GetEnumerator();
      while(enumerator.MoveNext()) {
        KeyValuePair<TKey, TValue> pair = enumerator.Current;
        callback(pair.Value);
      }
    }

    /// <summary>
    /// Clears the whole cache
    /// </summary>
    public void Clear() {
      storage.Clear();
    }

    /// <summary>
    /// Checks if there is at least one pending call to the cache
    /// </summary>
    public bool IsPending {
      get { return queued.Count > 0; }
    }

    private void enqueue(TKey key, CallbackFunction callback) {
      if(queued.ContainsKey(key)) {
        queued[key].Add(callback);
      } else {
        queued[key] = new List<CallbackFunction>{ callback };
        load(key); // invoke the first load per key
      }
    }

    private void onLoad(TKey key, TValue item) {
      storage[key] = item; // cache result
      if(!queued.ContainsKey(key)) return;
      // prevent allocations in Unity
      List<CallbackFunction> callbacks = queued[key];
      for (int i = 0, imax = callbacks.Count; i < imax; i++) {
        callbacks[i](item);
      }
      callbacks.Clear();
      queued.Remove(key);
    }

    private void load(TKey key) {
      loader(key, item => onLoad(key, item));
    }
  }
}
