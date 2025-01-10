using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DataManager : Singleton<DataManager>

/*
 *  전체 외부 데이터를 관리하는 클래스.
 *  DataTypeBase를 상속하는 외부데이터 직렬화 전용 클래스를 제너릭 클래스로 LoadData를 호출하면
 *  그 데이터를 Dictionary<int, T> 형태로 반환해 준다.
 */

{
    // 데이터 종류. 실제 값은 그 데이터의 테이블로 함.
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
        // 종류에 따라 데이터 로더를 로드하기 위한 매개변수가 초기화되지 않았다면 발생
        if (_loaderParameters == null)
            throw new DataManagerNotInitializedException("DataManager Not Initialized");

        var loader = new DataLoader<T>();

        // 해당 데이터 타입이 존재하지 않으면 발생
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