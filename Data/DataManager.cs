using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataManager : Singleton<DataManager>

/*
 *  ��ü �ܺ� �����͸� �����ϴ� Ŭ����.
 *  DataTypeBase�� ����ϴ� �ܺε����� ����ȭ ���� Ŭ������ ���ʸ� Ŭ������ LoadData�� ȣ���ϸ�
 *  �� �����͸� Dictionary<int, T> ���·� ��ȯ�� �ش�.
 */

{
    // ������ ����. ���� ���� �� �������� ���̺�� ��.
    public enum DATATYPE
    {
        NpcData = 30,
        EventData = 31,
        DialogData = 32,
        MonsterData = 20,
        MonsterReinforcementElementData = 29,
        WeaponData = 40,
        AmuletData = 41,
        TraitData = 42,
        CardData = 43,
        CostActionEffectData = 44,
        CostBuffEffectData = 45,
        CostDrawEffectData = 46,
        ActionJsonData = 60,
        ActionEffecterJsonData = 61,
        BuffData = 70,
        UIData = 81,
        ObjectPoolData = 82,
        BgmData = 90,
        SfxData = 91,
        RoomData = 100
    }

    private Dictionary<string, string> _loaderParameters;

    private new void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        SetLoaderParameters();
    }

    public Dictionary<int, T> LoadData<T>() where T : DataTypeBase
    {
        // ������ ���� ������ �δ��� �ε��ϱ� ���� �Ű������� �ʱ�ȭ���� �ʾҴٸ� �߻�
        if (_loaderParameters == null)
            throw new DataManagerNotInitializedException("DataManager Not Initialized");

        var loader = new DataLoader<T>();

        // �ش� ������ Ÿ���� �������� ������ �߻�
        if (!_loaderParameters.TryGetValue(typeof(T).Name, out string path))
            throw new ArgumentOutOfRangeException($"Loader Parameters Does Not Have {typeof(T)} Data");

        try
        {
            var data = loader.LoadData(path);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }

    private void SetLoaderParameters()
    {
        _loaderParameters = new();
        StringBuilder sb = new();

        foreach (var type in Enum.GetValues(typeof(DATATYPE)))
        {
            string typeName = type.ToString();

            sb.Clear();
            sb.Append("JSON/");
            sb.Append(typeName);

            _loaderParameters.Add(typeName, sb.ToString());
        }
    }
}

[Serializable]
public class DataManagerNotInitializedException : Exception
{
    public DataManagerNotInitializedException() { }
    public DataManagerNotInitializedException(string message) : base(message) { }
    public DataManagerNotInitializedException(string message, Exception inner) : base(message, inner) { }
    protected DataManagerNotInitializedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}