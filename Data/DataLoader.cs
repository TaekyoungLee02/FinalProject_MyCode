using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class DataLoader<T> where T : DataTypeBase

/*
*  외부에서 데이터를 직접적으로 로드하는 클래스.
*  
*  _loadedData의 in 는 id, T는 DataTypeBase를 상속하는 직렬화 전용 데이터이다.
*/

{
    private Dictionary<int, T> _loadedData;

    [Serializable]
    private class Data
    {
        public List<T> Items;
    }

    /// <summary>
    /// Loads Data From Json
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Dictionary<int, T> LoadData(string path)
    {
        _loadedData = new();

        try
        {
            string dataText = Resources.Load<TextAsset>(path).text;
            var data = JsonConvert.DeserializeObject<Data>(dataText);
            var itemList = data.Items;

            for (int i = 0; i < itemList.Count; i++)
            {
                _loadedData.Add(itemList[i].key, itemList[i]);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }

        return _loadedData;
    }
}