using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ScriptManager
{
    public static bool MakeManager<T>(MonoBehaviour script, ref T instance)
        where T : MonoBehaviour
    {
        if (instance == null)
        {
            instance = script as T;
            Object.DontDestroyOnLoad(instance.gameObject);
            return true;
        }
        else if (instance != script)
        {
            Object.Destroy(script.gameObject);
            Debug.LogWarning(typeof(T).Name + " duplicate found, destroying.");
            return false; // Delete Duplicate Manager
        }

        return true;
    }
}

