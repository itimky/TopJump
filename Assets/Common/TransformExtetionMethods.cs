using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensionMethods
{
    public static float GetMaxY(this Transform transform)
    {
        return transform.localPosition.y + transform.localScale.y * 0.5f;
    }

    public static float GetMinY(this Transform transform)
    {
        return transform.localPosition.y - transform.localScale.y * 0.5f;
    }

    public static float GetMaxX(this Transform transform)
    {
        return transform.localPosition.x + transform.localScale.y * 0.5f;
    }

    public static float GetMinX(this Transform transform)
    {
        return transform.localPosition.x - transform.localScale.y * 0.5f;
    }
}

public static class CollectionExtensionMethods
{
    public static void Shuffle<T>(this IList<T> list)
    {  
        int n = list.Count;  
        while (n > 1)
        {  
            n--;  
            int k = Random.Range(0, n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
    }

    public static T Of<T>(this object obj)
    {
        return (T)obj;
    }

    public static T As<T>(this object obj) where T : class
    {
        return obj as T;
    }
}