# NeoAsync: A class library for asynchronous tasks in `Unity3D`

NeoAsync is a growing toolset to manage asynchronous tasks when programming in Unity3D

## Installation

You can either use to copy the source files of this project into your Unity3D project or use Visual Studio to compile a DLL-file to be included in your project.

### Using Unity3D

* Clone the repository
* Copy the files from `Assets\NeoNetwork` into your project
  * This folder also includes an Assembly definition file

### Using VisualStudio

* Clone the repository
* Open `NeoAsync.sln` with Visual Studio
* Build the solution using "Build -> Build NeoAsync"
* Import the DLL (`obj/Release/NeoAsync.dll`) into your Unity3D project

Hint: Unity currently always reset the LangVersion to "7.3" which isn't supported by Visual Studio. Therefor you need to manually
set / revert the `LangVersion` to `6` in `NeoAsync.csproj`:

    <LangVersion>6</LangVersion>


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

By using an Dependency-Injection pattern / framework (e.g. [Strange](https://github.com/strangeioc/strangeioc)) unit-testing the timing behavior is easy.

## Testing

Use Unity's embedded Test Runner via `Window -> General -> Test Runner`.
