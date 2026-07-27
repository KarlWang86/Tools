# Unity 工具集合

面向 Unity 游戏开发的轻量级常用工具脚本集合。

本仓库包含可复用的运行时工具与编辑器工具，用于加速 UI 适配、事件分发、Prefab 深拷贝、资源引用查找等常见流程。

## 目录

### 运行时工具

1. `CanvasScalerSetting.cs`
- 根据当前屏幕比例与设计分辨率比例，自动设置 `CanvasScaler.matchWidthOrHeight`。
- 适用于不同宽高比设备下的 UI 快速适配。

2. `CombineMeshGameObject.cs`
- 将父节点下的子网格合并为一个网格。
- 合并后使用第一个子节点材质。
- 合并完成后移除子节点上的 `MeshRenderer` 与 `MeshFilter`。
- 适合用于静态场景物件的 Draw Call 优化。

3. `CoroutineUtils.cs`
- 协程辅助工具，包含：
  - `DelaySeconds(Action, float)`
  - `WaitForSeconds(float)`
  - `Do(Action)`
  - `Chain(params IEnumerator[])`
- `Chain` 目前保留了 TODO 占位实现，正式使用前请接入你项目中的协程驱动逻辑。

4. `EventManager.cs`
- 基于枚举事件名的轻量单例事件总线。
- 支持 `AddListener`、`RemoveListener`、`Raise`，并可通过 `params object[]` 传参。

5. `JsonHelper.cs`
- 对 `JsonUtility` 做了数组序列化/反序列化封装。
- 提供 `FromJson<T>(string)` 与 `ToJson<T>(T[])`。

6. `UnitySerializedDictionary.cs`
- 通过 `ISerializationCallbackReceiver` 让字典数据可被 Unity 序列化的基类。
- 常见写法：

```csharp
[Serializable]
public class IntStringDictionary : UnitySerializedDictionary<int, string> { }
```

### 编辑器工具

1. `FindReferences.cs`
- 添加菜单：`Assets/Find References In Project with path`。
- 在 `.prefab`、`.unity`、`.mat`、`.asset` 中查找对当前选中资源的外部引用。
- 通过 `GlobalObjectId` 匹配，兼容子资源引用场景（如 Texture -> Sprite）。

2. `Prefab_DeepCopy.cs`
- 添加菜单：`Assets/Deep Copy`。
- 将选中的 Prefab 及其依赖拷贝到新目录。
- 重写拷贝后文件内 GUID 引用，使其指向新拷贝资源。
- 适合快速做 Prefab 资源包抽取与复用。

## 快速开始

1. 将全部脚本复制到 Unity 项目中（推荐目录：`Assets/Tools`）。
2. 将编辑器脚本放入 `Editor` 目录：
   - `FindReferences.cs`
   - `Prefab_DeepCopy.cs`
3. 打开 Unity，等待脚本编译完成。
4. 运行时脚本通过挂载组件或调用静态方法使用。
5. 编辑器脚本通过 Project 视图中的 `Assets` 菜单触发。

## 使用方式

- 详细使用示例已迁移到各个类的源码注释中。
- 建议从以下文件查看示例：
  - `EventManager.cs`
  - `CombineMeshGameObject.cs`
  - `JsonHelper.cs`
  - `CoroutineUtils.cs`

## 运行要求

- Unity（脚本中使用的 UnityEditor API 已在常规项目环境中验证）
- 推荐 C# 7.0 及以上

## 注意事项与限制

- `CombineMeshGameObject` 默认子节点同时存在 `MeshFilter` 与 `MeshRenderer`。
- 网格合并时请注意顶点数量限制（历史上 16-bit 索引网格约为 65k）。
- `Prefab_DeepCopy` 当前按设计会忽略 `.cs`、`.shader`、`.shadergraph` 依赖。
- `CoroutineUtils.Chain` 需要接入项目自己的协程运行器实现。

## 许可证

详见 `LICENSE`。
# Tools