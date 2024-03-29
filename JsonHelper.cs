﻿using System;
/// <summary>
/// used for json array convert
/// </summary>
public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    /**
     * Usage: JsonHelper.ToJson(arr);
     * For example:
     *     int[] arr = new int[100];
     *     string json = JsonHelper.ToJson(arr);
     */
    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper, true);
    }

    [SerializableAttribute]
    private class Wrapper<T> {
        public T[] Items;
    }
}
