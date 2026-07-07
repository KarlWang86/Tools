# Unity Tools Collection | Unity 工具集合

Lightweight utility scripts for Unity game development.  
面向 Unity 游戏开发的轻量级常用工具脚本集合。

This repository contains reusable runtime helpers and editor tools used in real projects, helping speed up UI adaptation, event dispatching, prefab deep copy, and asset reference lookup.  
本仓库包含可复用的运行时工具与编辑器工具，用于加速 UI 适配、事件分发、Prefab 深拷贝、资源引用查找等常见流程。

## Contents | 目录

### Runtime Utilities | 运行时工具

1. `CanvasScalerSetting.cs`
- Auto sets `CanvasScaler.matchWidthOrHeight` based on current screen ratio and design ratio.
- 根据当前屏幕比例与设计分辨率比例，自动设置 `CanvasScaler.matchWidthOrHeight`。
- Useful for quickly adapting UI across different aspect ratios.
- 适用于不同宽高比设备下的 UI 快速适配。

2. `CombineMeshGameObject.cs`
- Combines child meshes under one parent object into a single mesh.
- 将父节点下的子网格合并为一个网格。
- Keeps the first child material on the merged mesh.
- 合并后使用第一个子节点材质。
- Removes `MeshRenderer` and `MeshFilter` from children after merge.
- 合并完成后移除子节点上的 `MeshRenderer` 与 `MeshFilter`。
- Good for reducing draw calls in static environment objects.
- 适合用于静态场景物件的 Draw Call 优化。

3. `CoroutineUtils.cs`
- Small coroutine helpers:
- 协程辅助工具，包含：
  - `DelaySeconds(Action, float)`
  - `WaitForSeconds(float)`
  - `Do(Action)`
  - `Chain(params IEnumerator[])`
- `Chain` contains a TODO placeholder and should be connected to your own coroutine runner before real use.
- `Chain` 目前保留了 TODO 占位实现，正式使用前请接入你项目中的协程驱动逻辑。

4. `EventManager.cs`
- Simple singleton event bus based on enum event names.
- 基于枚举事件名的轻量单例事件总线。
- Supports `AddListener`, `RemoveListener`, and `Raise` with `params object[]` payload.
- 支持 `AddListener`、`RemoveListener`、`Raise`，并可通过 `params object[]` 传参。

5. `JsonHelper.cs`
- Wrapper around `JsonUtility` to support array serialization/deserialization.
- 对 `JsonUtility` 做了数组序列化/反序列化封装。
- `FromJson<T>(string)` and `ToJson<T>(T[])`.
- 提供 `FromJson<T>(string)` 与 `ToJson<T>(T[])`。

6. `UnitySerializedDictionary.cs`
- Base class to make dictionary-like data serializable in Unity via `ISerializationCallbackReceiver`.
- 通过 `ISerializationCallbackReceiver` 让字典数据可被 Unity 序列化的基类。
- Typical pattern / 常见写法：

```csharp
[Serializable]
public class IntStringDictionary : UnitySerializedDictionary<int, string> { }
```

### Editor Utilities | 编辑器工具

1. `FindReferences.cs`
- Adds menu: `Assets/Find References In Project with path`.
- 添加菜单：`Assets/Find References In Project with path`。
- Finds external references to the currently selected asset in `.prefab`, `.unity`, `.mat`, and `.asset` files.
- 在 `.prefab`、`.unity`、`.mat`、`.asset` 中查找对当前选中资源的外部引用。
- Handles sub-asset scenarios (for example Texture -> Sprite references) by matching `GlobalObjectId`.
- 通过 `GlobalObjectId` 匹配，兼容子资源引用场景（如 Texture -> Sprite）。

2. `Prefab_DeepCopy.cs`
- Adds menu: `Assets/Deep Copy`.
- 添加菜单：`Assets/Deep Copy`。
- Deep-copies a selected prefab and its dependencies into a new folder.
- 将选中的 Prefab 及其依赖拷贝到新目录。
- Rewrites GUID references inside copied files to point to copied assets.
- 重写拷贝后文件内 GUID 引用，使其指向新拷贝资源。
- Intended for fast prefab package extraction/reuse.
- 适合快速做 Prefab 资源包抽取与复用。

## Quick Start | 快速开始

1. Copy all scripts into your Unity project (recommended under `Assets/Tools`).  
   将全部脚本复制到 Unity 项目中（推荐目录：`Assets/Tools`）。
2. Put editor-only scripts in an `Editor` folder:  
   将编辑器脚本放入 `Editor` 目录：
   - `FindReferences.cs`
   - `Prefab_DeepCopy.cs`
3. Open Unity and let scripts compile.  
   打开 Unity，等待脚本编译完成。
4. Use runtime scripts by attaching components or calling static helpers.  
   运行时脚本通过挂载组件或调用静态方法使用。
5. Use editor scripts from the `Assets` menu in Project view.  
   编辑器脚本通过 Project 视图中的 `Assets` 菜单触发。

## Usage | 使用方式

- Detailed usage examples are moved into source code comments for each class.
- 详细使用示例已迁移到各个类的源码注释中。
- Start from these files:
- 建议从以下文件查看示例：
  - `EventManager.cs`
  - `CombineMeshGameObject.cs`
  - `JsonHelper.cs`
  - `CoroutineUtils.cs`

## Requirements | 运行要求

- Unity (tested with UnityEditor APIs used in scripts)  
  Unity（脚本中使用的 UnityEditor API 已在常规项目环境中验证）
- C# 7.0+ recommended  
  推荐 C# 7.0 及以上

## Notes and Limitations | 注意事项与限制

- `CombineMeshGameObject` assumes child objects have both `MeshFilter` and `MeshRenderer`.  
  `CombineMeshGameObject` 默认子节点同时存在 `MeshFilter` 与 `MeshRenderer`。
- For merged meshes, keep Unity vertex limits in mind (historically 65k for 16-bit index meshes).  
  网格合并时请注意顶点数量限制（历史上 16-bit 索引网格约为 65k）。
- `Prefab_DeepCopy` currently ignores `.cs`, `.shader`, `.shadergraph` dependencies by design.  
  `Prefab_DeepCopy` 当前按设计会忽略 `.cs`、`.shader`、`.shadergraph` 依赖。
- `CoroutineUtils.Chain` needs project-specific coroutine-runner integration.  
  `CoroutineUtils.Chain` 需要接入项目自己的协程运行器实现。

## License | 许可证

See `LICENSE`.  
详见 `LICENSE`。
# Tools