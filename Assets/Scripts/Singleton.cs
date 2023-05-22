using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    public virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
    }
}
