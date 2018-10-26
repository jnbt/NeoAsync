# NeoAsync: A class library for asynchronous tasks in `Unity3D`

NeoAsync is a growing toolset to manage asynchronous tasks when programming in Unity3D

## Installation

If you don't have access to [Microsoft VisualStudio](http://msdn.microsoft.com/de-de/vstudio) you can just use Unity3D and its compiler.
Or use your VisualStudio installation in combination with [Visual Studio Tools for Unity](http://unityvs.com) to compile a DLL-file, which
can be included into your project.

### Using Unity3D

* Clone the repository
* Copy the files from `Assets\NeoAsync` into your project

### Using VisualStudio

* Clone the repository
* Open the folder as a Unity3D project
* Install the *free* [Unity Testing Tools](https://www.assetstore.unity3d.com/#/content/13802) from the AssetStore
* Install the *free* [Visual Studio Tools for Unity](http://unityvs.com) and import its Unity-package
* Open `UnityVS.NeoAsync.sln`
* [Build a DLL-File](http://forum.unity3d.com/threads/video-tutorial-how-to-use-visual-studio-for-all-your-unity-development.120327)
* Import the DLL and dependencies into your Unity3D project

## Dependencies

* [NeoCollections](https://github.com/jnbt/NeoCollections)

## Usage

### `CoroutineStarter`

Allows starting a Unity-based coroutine from any instance.

```csharp
class SomeClass {
  public void Do(){
    CoroutineStarter.Instance.Add(doLazy());
  }
  private IEnumerator doLazy(){
    yield return UnityEngine.WaitForSeconds(5f);
    UnityEngine.Debug.Log("This should be invoked as a coroutine");
  }
}
```

### Cache\<T\>

Provides a generic cache for any kind of objects which loading might need some time and can be referenced by a string key.

```csharp
class SomeClass{
  private readonly Cache<UnityEngine.GameObject> cache;
  public SomeClass(){
    cache = new Cache<UnityEngine.GameObject>(resolveGameObject);
  }
  public void Do(){
    cache.Get("MyBigGameObject", (go) => go.transform.position = UnityEngine.Vector3.zero);
  }
  private GameObject resolveGameObject(string key){
    UnityEngine.Resources.Load<GameObject>(key);
  }
}
```

### Timing / ITiming

Allows easy time-driven callbacks:

* After(seconds, callback): Invokes a callback after the timeout in seconds.
* Every(seconds, callback): Invokes a callback every x seconds. First time in x seconds.

```csharp
// In the best case inject a Timing instance
[Inject]
public ITiming Timing { get; set; }

// Than in your functions created a deferred call
IDeferred after = Timing.After(5, () => UnityEngine.Debug.Log("This will be invoked in 5 seconds"));

// You can abort the call (if not already executed)
after.Abort();

// Also a interval-based variant is available for convience
Timing.Every(5, () => UnityEnging.Debug.Log("This will be invoked EVERY 5 seconds"));

```

By using an Dependency-Injection pattern / framework (e.g. [Strange](https://github.com/strangeioc/strangeioc)) unit-testing the timing behavior is so easy.

## Testing

Use Unity's embedded Test Runner via `Window -> General -> Test Runner`.
