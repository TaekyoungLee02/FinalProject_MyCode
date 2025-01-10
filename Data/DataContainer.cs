/*
        사실 이 스크립트는 원래 예정에 없었으나
        팀원들이 데이터매니저를 사용하는 것을 조금 어려워하길래
        조금 더 사용하기 쉽게 만든 클래스입니다.

        이 클래스를 만들고 나서 다들 좀 더 쉽게 사용한 것 같아서
        앞으로는 이런 클래스처럼 기능을 만들 때 다른 팀원들이 사용하기 쉽게 만들어야 한다는 걸 깨달았습니다.

        이 부분은 전부 제가 작성했습니다.
*/



using System;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer<TData> where TData : DataTypeBase
{
    private Dictionary<int, TData> _data;

    public Dictionary<int, TData> Data { get { return _data; } }

    public DataContainer() 
    {
        // DataManager ¸¦ ÅëÇØ µ¥ÀÌÅÍ¸¦ ¹Þ¾Æ¿È
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
