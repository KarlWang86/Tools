# Unity Tools Collection

Lightweight utility scripts for Unity game development.

This repository contains reusable runtime helpers and editor tools used in real projects, helping speed up UI adaptation, event dispatching, prefab deep copy, and asset reference lookup.

## Contents

### Runtime Utilities

1. `CanvasScalerSetting.cs`
- Auto sets `CanvasScaler.matchWidthOrHeight` based on current screen ratio and design ratio.
- Useful for quickly adapting UI across different aspect ratios.

2. `CombineMeshGameObject.cs`
- Combines child meshes under one parent object into a single mesh.
- Keeps the first child material on the merged mesh.
- Removes `MeshRenderer` and `MeshFilter` from children after merge.
- Good for reducing draw calls in static environment objects.

3. `CoroutineUtils.cs`
- Small coroutine helpers:
  - `DelaySeconds(Action, float)`
  - `WaitForSeconds(float)`
  - `Do(Action)`
  - `Chain(params IEnumerator[])`
- `Chain` contains a TODO placeholder and should be connected to your own coroutine runner before real use.

4. `EventManager.cs`
- Simple singleton event bus based on enum event names.
- Supports `AddListener`, `RemoveListener`, and `Raise` with `params object[]` payload.

5. `JsonHelper.cs`
- Wrapper around `JsonUtility` to support array serialization/deserialization.
- `FromJson<T>(string)` and `ToJson<T>(T[])`.

6. `UnitySerializedDictionary.cs`
- Base class to make dictionary-like data serializable in Unity via `ISerializationCallbackReceiver`.
- Typical pattern:

```csharp
[Serializable]
public class IntStringDictionary : UnitySerializedDictionary<int, string> { }
```

### Editor Utilities

1. `FindReferences.cs`
- Adds menu: `Assets/Find References In Project with path`.
- Finds external references to the currently selected asset in `.prefab`, `.unity`, `.mat`, and `.asset` files.
- Handles sub-asset scenarios (for example Texture -> Sprite references) by matching `GlobalObjectId`.

2. `Prefab_DeepCopy.cs`
- Adds menu: `Assets/Deep Copy`.
- Deep-copies a selected prefab and its dependencies into a new folder.
- Rewrites GUID references inside copied files to point to copied assets.
- Intended for fast prefab package extraction/reuse.

## Quick Start

1. Copy all scripts into your Unity project (recommended under `Assets/Tools`).
2. Put editor-only scripts in an `Editor` folder:
   - `FindReferences.cs`
   - `Prefab_DeepCopy.cs`
3. Open Unity and let scripts compile.
4. Use runtime scripts by attaching components or calling static helpers.
5. Use editor scripts from the `Assets` menu in Project view.

## Usage

- Detailed usage examples are moved into source code comments for each class.
- Start from these files:
  - `EventManager.cs`
  - `CombineMeshGameObject.cs`
  - `JsonHelper.cs`
  - `CoroutineUtils.cs`

## Requirements

- Unity (tested with UnityEditor APIs used in scripts)
- C# 7.0+ recommended

## Notes and Limitations

- `CombineMeshGameObject` assumes child objects have both `MeshFilter` and `MeshRenderer`.
- For merged meshes, keep Unity vertex limits in mind (historically 65k for 16-bit index meshes).
- `Prefab_DeepCopy` currently ignores `.cs`, `.shader`, `.shadergraph` dependencies by design.
- `CoroutineUtils.Chain` needs project-specific coroutine-runner integration.

## License

See `LICENSE`.
