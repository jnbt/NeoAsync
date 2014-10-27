using System;
using Neo.Collections;

namespace Neo.Async{
  // Provides a generic cache for any kind of objects which loading
  // might need some time and can be referenced by a string key
  // Idea: Provide a function to load one object via key. This
  // loader will only be called once per key (until the cache is cleared)
  //
  // This implies that you have to fetch objects from the cache via a callback instead
  // of a simple getter, as the loading might need some time and might be done in the background
  //
  // Usage:
  //   class SomeClass{
  //     private readonly Cache<UnityEngine.GameObject> cache;
  //
  //     public SomeClass(){
  //       cache = new Cache<UnityEngine.GameObject>(resolveGameObject);
  //     }
  //
  //     public void Do(){
  //       cache.Get("MyBigGameObject", (go) => go.transform.position = UnityEngine.Vector3.zero);
  //     }
  //
  //     private GameObject resolveGameObject(string key){
  //       UnityEngine.Resources.Load<GameObject>(key);
  //     }
  //   }
  public class Cache<T> {
    public delegate void LoaderFunction(string key, Action<T> callback);
    public delegate void CallbackFunction(T item);

    private sealed class Entry{
      public string Key{get; set;}
      public CallbackFunction Callback{get; set;}
    }

    private LoaderFunction loader;
    private Dictionary<string,T> storage = new Dictionary<string,T>();
    private List<Entry> queued           = new List<Entry>();

    public Cache(LoaderFunction loader){
      this.loader = loader;
    }

    public void Get(string key, CallbackFunction callback){
      if(storage.ContainsKey(key)) callback(storage[key]);
      else enqueue(key, callback);
    }

    public void Clear(){
      storage.Clear();
    }

    private void enqueue(string key, CallbackFunction callback){
      Entry entry = new Entry(){
        Key      = key,
        Callback = callback
      };

      if(queued.IsEmpty) {
        queued.Add(entry);
        load(entry); // directly invoke the first download
      }else{
        queued.Add(entry); // all other downloads are queued, so max 1 at a time
      }
    }

    private void onLoad(string key, T item){
      storage[key] = item; // cache result
      for(int i=queued.Count-1; i>=0; i--){ //iterate reverse to allow removeAt
        Entry entry = queued[i];
        if(entry.Key == key){
          queued.RemoveAt(i);
          entry.Callback(item);
        }
      }
      if(!queued.IsEmpty){
        load(queued.First);
      }
    }

    private void load(Entry entry){
      loader(entry.Key, (item) => onLoad(entry.Key, item));
    }

  }

}