using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FindReferences
{
    [MenuItem("Assets/Find References In Project with path", false, 25)]
    private static void FindProjectReferences()
    {
        // 获取当前选中的资源
        Object selectedObject = Selection.activeObject;
        if (selectedObject == null) return;

        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("无法获取选中资源路径");
            return;
        }

        Debug.Log($"正在查找引用了资源 [{selectedObject.name}] 的文件...");

        string targetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        // 兼容 Texture2D -> Sprite 子资源引用的场景
        HashSet<string> targetObjectIds = BuildTargetObjectIdSet(selectedObject, assetPath);

        // 获取项目中所有的 Prefab、Scene、Material 等资源的路径
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        List<ReferenceInfo> foundReferences = new List<ReferenceInfo>();

        int count = 0;
        foreach (string path in allAssetPaths)
        {
            // 过滤掉自身，只检查可序列化资源
            if (path == assetPath)
            {
                continue;
            }

            if (!IsSupportedAsset(path))
            {
                continue;
            }

            if (!MayContainGuid(path, targetGuid))
            {
                continue;
            }

            count++;
            CollectReferencesFromAsset(path, targetObjectIds, foundReferences);
        }

        // 输出结果
        Debug.Log($"--- 查找完成，共扫描了 {count} 个文件，找到 {foundReferences.Count} 处引用 ---");
        if (foundReferences.Count == 0)
        {
            Debug.LogWarning("未找到任何外部引用。");
            return;
        }

        foreach (var refInfo in foundReferences)
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(refInfo.FilePath);
            string displayText = $"[{refInfo.FileType}] {refInfo.FilePath}\n  └─ {refInfo.HierarchyPath}.{refInfo.ComponentName}\n     {refInfo.FieldInfo}";
            Debug.Log(displayText, obj);
        }
    }

    private struct ReferenceInfo
    {
        public string FilePath;
        public string FileType;
        public string HierarchyPath;
        public string ComponentName;
        public string FieldInfo;
    }

    private static bool IsSupportedAsset(string filePath)
    {
        return filePath.EndsWith(".prefab")
            || filePath.EndsWith(".unity")
            || filePath.EndsWith(".mat")
            || filePath.EndsWith(".asset");
    }

    private static bool MayContainGuid(string filePath, string targetGuid)
    {
        if (string.IsNullOrEmpty(targetGuid))
        {
            return false;
        }

        if (!File.Exists(filePath))
        {
            return false;
        }

        string extension = Path.GetExtension(filePath);
        if (!IsTextSerializedExtension(extension))
        {
            return false;
        }

        try
        {
            string content = File.ReadAllText(filePath);
            return content.IndexOf(targetGuid, System.StringComparison.Ordinal) >= 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsTextSerializedExtension(string extension)
    {
        return extension == ".prefab"
            || extension == ".unity"
            || extension == ".mat"
            || extension == ".asset";
    }

    private static HashSet<string> BuildTargetObjectIdSet(Object selectedObject, string assetPath)
    {
        var ids = new HashSet<string>(System.StringComparer.Ordinal);

        AddObjectIdIfAsset(ids, selectedObject);

        // 子资源（例如 Sprite）不会被 Selection.activeObject 总是直接选中，补充进匹配集。
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        for (int i = 0; i < subAssets.Length; i++)
        {
            AddObjectIdIfAsset(ids, subAssets[i]);
        }

        return ids;
    }

    private static void AddObjectIdIfAsset(HashSet<string> ids, Object obj)
    {
        if (obj == null)
        {
            return;
        }

        GlobalObjectId globalId = GlobalObjectId.GetGlobalObjectIdSlow(obj);
        string idString = globalId.ToString();
        if (!string.IsNullOrEmpty(idString))
        {
            ids.Add(idString);
        }
    }

    private static void CollectReferencesFromAsset(string filePath, HashSet<string> targetObjectIds, List<ReferenceInfo> references)
    {
        if (filePath.EndsWith(".prefab"))
        {
            CollectReferencesInPrefab(filePath, targetObjectIds, references);
            return;
        }

        if (filePath.EndsWith(".unity"))
        {
            CollectReferencesInScene(filePath, targetObjectIds, references);
            return;
        }

        CollectReferencesInAsset(filePath, targetObjectIds, references);
    }

    private static void CollectReferencesInPrefab(string prefabPath, HashSet<string> targetObjectIds, List<ReferenceInfo> references)
    {
        GameObject root = null;
        try
        {
            root = PrefabUtility.LoadPrefabContents(prefabPath);
            if (root == null)
            {
                return;
            }

            Component[] components = root.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }

                string hierarchyPath = BuildHierarchyPath(component.transform);
                CollectSerializedObjectReferences(component, targetObjectIds, prefabPath, "Prefab", hierarchyPath, component.GetType().Name, references);
            }
        }
        finally
        {
            if (root != null)
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }

    private static void CollectReferencesInScene(string scenePath, HashSet<string> targetObjectIds, List<ReferenceInfo> references)
    {
        SceneSetup[] originalSetup = EditorSceneManager.GetSceneManagerSetup();
        Scene loadedScene = default;
        bool opened = false;

        try
        {
            loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            opened = loadedScene.IsValid();
            if (!opened)
            {
                return;
            }

            GameObject[] roots = loadedScene.GetRootGameObjects();
            for (int r = 0; r < roots.Length; r++)
            {
                Component[] components = roots[r].GetComponentsInChildren<Component>(true);
                for (int i = 0; i < components.Length; i++)
                {
                    Component component = components[i];
                    if (component == null)
                    {
                        continue;
                    }

                    string hierarchyPath = BuildHierarchyPath(component.transform);
                    CollectSerializedObjectReferences(component, targetObjectIds, scenePath, "Scene", hierarchyPath, component.GetType().Name, references);
                }
            }
        }
        finally
        {
            if (opened)
            {
                EditorSceneManager.CloseScene(loadedScene, true);
            }

            EditorSceneManager.RestoreSceneManagerSetup(originalSetup);
        }
    }

    private static void CollectReferencesInAsset(string assetPath, HashSet<string> targetObjectIds, List<ReferenceInfo> references)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        string fileType = assetPath.EndsWith(".mat") ? "Material" : "Asset";

        for (int i = 0; i < objects.Length; i++)
        {
            Object obj = objects[i];
            if (obj == null)
            {
                continue;
            }

            if (obj is GameObject gameObject)
            {
                Component[] components = gameObject.GetComponentsInChildren<Component>(true);
                for (int c = 0; c < components.Length; c++)
                {
                    Component component = components[c];
                    if (component == null)
                    {
                        continue;
                    }

                    string hierarchyPath = BuildHierarchyPath(component.transform);
                    CollectSerializedObjectReferences(component, targetObjectIds, assetPath, fileType, hierarchyPath, component.GetType().Name, references);
                }

                continue;
            }

            string ownerName = string.IsNullOrEmpty(obj.name) ? "Root" : obj.name;
            CollectSerializedObjectReferences(obj, targetObjectIds, assetPath, fileType, ownerName, obj.GetType().Name, references);
        }
    }

    private static void CollectSerializedObjectReferences(
        Object serializedTarget,
        HashSet<string> targetObjectIds,
        string filePath,
        string fileType,
        string hierarchyPath,
        string componentName,
        List<ReferenceInfo> references)
    {
        SerializedObject serializedObject;
        try
        {
            serializedObject = new SerializedObject(serializedTarget);
        }
        catch
        {
            return;
        }

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.Next(enterChildren))
        {
            enterChildren = false;

            if (iterator.propertyType != SerializedPropertyType.ObjectReference)
            {
                continue;
            }

            Object refObj = iterator.objectReferenceValue;
            if (refObj == null)
            {
                continue;
            }

            string refId = GlobalObjectId.GetGlobalObjectIdSlow(refObj).ToString();
            if (!targetObjectIds.Contains(refId))
            {
                continue;
            }

            references.Add(new ReferenceInfo
            {
                FilePath = filePath,
                FileType = fileType,
                HierarchyPath = hierarchyPath,
                ComponentName = componentName,
                FieldInfo = string.IsNullOrEmpty(iterator.displayName)
                    ? iterator.propertyPath
                    : $"{iterator.displayName} ({iterator.propertyPath})"
            });
        }
    }

    private static string BuildHierarchyPath(Transform transform)
    {
        List<string> names = new List<string>();
        Transform current = transform;
        while (current != null)
        {
            names.Add(current.name);
            current = current.parent;
        }

        names.Reverse();
        return string.Join("/", names);
    }
}