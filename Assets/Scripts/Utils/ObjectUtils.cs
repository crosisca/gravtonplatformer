using System;
using System.Linq;
using UnityEngine;

public static class ObjectUtils
{
    public static bool MatchesType<T> (this object obj)
    {
        Type type = obj.GetType();
        Type comparedType = typeof(T);
        return comparedType == type || type.IsSubclassOf(comparedType) || comparedType.IsSubclassOf(type);
    }

    public static bool MatchesAnyType (this object obj, params Type[] types)
    {
        Type type = obj.GetType();

        foreach (Type comparedType in types)
        {
            if (comparedType == type || type.IsSubclassOf(comparedType) || comparedType.IsSubclassOf(type))
                return true;
        }

        return false;
    }

    public static bool MatchesType<T> (this Type type)
    {
        Type comparedType = typeof(T);
        return comparedType == type || type.IsSubclassOf(comparedType) || comparedType.IsSubclassOf(type);
    }

    public static bool MatchesType (this Type type, Type comparedType)
    {
        return comparedType == type || type.IsSubclassOf(comparedType) || comparedType.IsSubclassOf(type);
    }

    public static bool MatchesType (this object obj, Type comparedType)
    {
        Type type = obj.GetType();
        return comparedType == type || type.IsSubclassOf(comparedType) || comparedType.IsSubclassOf(type);
    }

    public static string ArrayToString (this object[] objectArray)
    {
        if (objectArray == null)
            return string.Empty;

        return string.Join(", ", objectArray.Select(x => x != null ? x.ToString() : "null").ToArray());
    }

    public static T GetOrAddComponentInChildren<T> (this GameObject go) where T : Component
    {
        T ret = go.GetComponentInChildren<T>(true);

        if (ret == null)
            ret = go.AddComponent<T>();

        return ret;
    }

    public static T GetOrAddComponent<T> (this GameObject go) where T : Component
    {
        T ret = go.GetComponent<T>();

        if (ret == null)
            ret = go.AddComponent<T>();

        return ret;
    }
}