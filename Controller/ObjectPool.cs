using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T>
{
    private List<T> poolList = new List<T>();
    private Func<T> func;

    public ObjectPool (Func<T> func, int count)
    {
        this.func = func;
        InstanceObject(count);
    }

    private void InstanceObject(int count)
    {
        for (int i = 0; i < count; i++)
        {
            poolList.Add(func());
        }
    }

    public T GetObject()
    {
        int i = poolList.Count;
        while (i-- > 0)
        {
            T t = poolList[i];
            poolList.RemoveAt(i);
            return t;
        }

        InstanceObject(2);
        return GetObject();
    }

    public void AddObject(T t)
    {
        poolList.Add(t);
    }
}
