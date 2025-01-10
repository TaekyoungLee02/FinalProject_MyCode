using System;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer<TData> where TData : DataTypeBase
{
    private Dictionary<int, TData> _data;

    public Dictionary<int, TData> Data { get { return _data; } }

    public DataContainer() 
    {
        // DataManager 를 통해 데이터를 받아옴
        var dataManager = DataManager.Instance;
        _data = dataManager.LoadData<TData>();
    }

    /// <summary>
    /// User When Resources.Load Required
    /// </summary>
    /// <returns></returns>
    private bool LoadResource<T>(int pathIndex, out Dictionary<int, T> objects) where T : UnityEngine.Object
    {
        objects = new();

        foreach (var data in _data)
        {
            string path = data.Value.GetPath(pathIndex);

            if(path.Equals(null)) continue;

            try
            {
                var loadedObject = Resources.Load<T>(path);

                objects.Add(data.Key, loadedObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        if(objects.Count == 0) return false;
        else return true;
    }


    /// <summary>
    /// User When Resources.Load Required
    /// </summary>
    /// <returns></returns>
    public bool TryLoadResource<T>(out Dictionary<int, T> objects, int pathIndex = 0) where T : UnityEngine.Object
    {
        bool result = LoadResource<T>(pathIndex, out var _objects);

        if (result)
        {
            objects = _objects;
            return true;
        }
        else
        {
            objects = null;
            return false;
        }
    }
}