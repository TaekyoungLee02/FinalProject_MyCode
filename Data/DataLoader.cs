using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class DataLoader<T> where T : DataTypeBase

/*
*  �ܺο��� �����͸� ���������� �ε��ϴ� Ŭ����.
*  
*  _loadedData�� in �� id, T�� DataTypeBase�� ����ϴ� ����ȭ ���� �������̴�.
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